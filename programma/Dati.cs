// Da dove arrivano i dati: i file di Claude Code sul disco, il server della
// lista lavori, e Windows per i dispositivi. Niente di tutto questo gira nel
// filo della finestra: qui ci sono solo le funzioni, le chiama la cucina.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Web.Script.Serialization;

namespace WhyWidget {

  public class Sessione {
    public string id;
    public string titolo;
    public string quando;
    public string dove;      // la cartella dove e' nata: senza, riaprirla non la riprende
  }

  public class Collegato {
    public string nome;
    public string via;
    public int? carica;      // niente carica = Windows non la dice. Qui non si inventa.
    public bool acceso;
  }

  public static class Fonte {
    public static string BASE = "http://localhost:4180";

    static string CartellaClaude {
      get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".claude"); }
    }

    // ---------- quanto tempo fa ----------
    public static string Quando(DateTime d) {
      int m = (int)(DateTime.Now - d).TotalMinutes;
      if (m < 1) return "ADESSO";
      if (m < 60) return m + " MIN FA";
      int o = m / 60;
      if (o < 24) return o + " H FA";
      int g = o / 24;
      return g == 1 ? "IERI" : g + " GIORNI FA";
    }

    // ---------- le sessioni di Claude Code ----------
    static string TitoloSessione(string file) {
      try {
        var s = new JavaScriptSerializer();
        int letti = 0;
        foreach (string riga in LeggiRighe(file, 220)) {
          letti++;
          if (riga.IndexOf("\"type\":\"user\"") < 0) continue;
          try {
            var o = s.Deserialize<Dictionary<string, object>>(riga);
            if (!o.ContainsKey("message")) continue;
            var msg = o["message"] as Dictionary<string, object>;
            if (msg == null || !msg.ContainsKey("content")) continue;
            string t = null;
            if (msg["content"] is string) t = (string)msg["content"];
            else {
              var arr = msg["content"] as System.Collections.ArrayList;
              if (arr != null)
                foreach (var v in arr) {
                  var d = v as Dictionary<string, object>;
                  if (d != null && d.ContainsKey("type") && (string)d["type"] == "text" && d.ContainsKey("text")) {
                    t = (string)d["text"]; break;
                  }
                }
            }
            if (!string.IsNullOrEmpty(t) && t[0] != '<' && !t.StartsWith("Caveat:"))
              return System.Text.RegularExpressions.Regex.Replace(t, "\\s+", " ").Trim();
          } catch { }
        }
      } catch { }
      return "sessione senza titolo";
    }

    static IEnumerable<string> LeggiRighe(string file, int quante) {
      using (StreamReader r = new StreamReader(file)) {
        for (int i = 0; i < quante; i++) {
          string riga = r.ReadLine();
          if (riga == null) yield break;
          yield return riga;
        }
      }
    }

    // la cartella di lavoro sta scritta dentro il diario della sessione
    static string CartellaSessione(string file) {
      try {
        var s = new JavaScriptSerializer();
        foreach (string riga in LeggiRighe(file, 40)) {
          if (riga.IndexOf("\"cwd\"") < 0) continue;
          try {
            var o = s.Deserialize<Dictionary<string, object>>(riga);
            if (o.ContainsKey("cwd")) {
              string c = o["cwd"] as string;
              if (!string.IsNullOrEmpty(c) && Directory.Exists(c)) return c;
            }
          } catch { }
        }
      } catch { }
      return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    public static List<Sessione> Sessioni(int quante) {
      List<Sessione> fuori = new List<Sessione>();
      try {
        string cartella = Path.Combine(CartellaClaude, "projects");
        if (!Directory.Exists(cartella)) return fuori;
        var file = new DirectoryInfo(cartella).GetFiles("*.jsonl", SearchOption.AllDirectories)
                     .OrderByDescending(f => f.LastWriteTime).Take(quante);
        foreach (var f in file) {
          fuori.Add(new Sessione {
            id = Path.GetFileNameWithoutExtension(f.Name),
            titolo = TitoloSessione(f.FullName),
            quando = Quando(f.LastWriteTime),
            dove = CartellaSessione(f.FullName)
          });
        }
      } catch { }
      return fuori;
    }

    // ---------- i suoi agenti ----------
    static readonly string[] MIEI = { "mixy", "vally", "whyrig", "clack", "designo" };

    public static List<string> Agenti() {
      List<string> fuori = new List<string>();
      try {
        string cartella = Path.Combine(CartellaClaude, "agents");
        if (!Directory.Exists(cartella)) return fuori;
        foreach (var f in new DirectoryInfo(cartella).GetFiles("*.md")) {
          string n = Path.GetFileNameWithoutExtension(f.Name);
          if (Array.IndexOf(MIEI, n) >= 0) fuori.Add(n);
        }
      } catch { }
      return fuori;
    }

    // ---------- la lista dei lavori ----------
    // Legge il file della lista direttamente: cosi' funziona anche quando il
    // server di WhyPanel e' spento. Il server e' solo la seconda strada.
    public static string FILE_LAVORI = @"E:\Lavori\dati\lavori.json";

    public static Dictionary<string, object> Lavori() {
      try {
        if (File.Exists(FILE_LAVORI))
          return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(File.ReadAllText(FILE_LAVORI));
      } catch { }
      try {
        var req = (HttpWebRequest)WebRequest.Create(BASE + "/dati");
        req.Timeout = 3000;
        using (var risp = req.GetResponse())
        using (var lettore = new StreamReader(risp.GetResponseStream())) {
          return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(lettore.ReadToEnd());
        }
      } catch { return null; }
    }

    // ---------- cosa e' collegato ----------
    // Windows la percentuale la da' solo per certi dispositivi (quelli che la
    // dichiarano). Dove non la da', qui non viene inventata: si dice solo com'e' attaccato.
    const string CHIAVE_BATTERIA = "{104EA319-6EE2-4701-BD47-8DDBF425BBE5} 2";

    public static List<Collegato> Collegati() {
      List<Collegato> fuori = new List<Collegato>();

      // 1) Bluetooth: solo i dispositivi veri, non i servizi di sistema
      try {
        var cerca = new ManagementObjectSearcher(
          "SELECT Name, DeviceID, Status FROM Win32_PnPEntity WHERE " +
          "DeviceID LIKE 'BTHENUM\\\\DEV_%' OR DeviceID LIKE 'BTHLE\\\\DEV_%' OR DeviceID LIKE 'BTHLEDEVICE\\\\%'");
        foreach (ManagementObject o in cerca.Get()) {
          fuori.Add(new Collegato {
            nome = Pulisci((string)o["Name"]),
            via = "BLUETOOTH",
            carica = Batteria((string)o["DeviceID"]),
            acceso = ((string)o["Status"]) == "OK"
          });
        }
      } catch { }

      // 2) la batteria del PC: sui fissi non c'e', sui portatili si'
      try {
        foreach (ManagementObject o in new ManagementObjectSearcher("SELECT * FROM Win32_Battery").Get()) {
          fuori.Add(new Collegato {
            nome = "BATTERIA PC", via = "INTERNA",
            carica = Convert.ToInt32(o["EstimatedChargeRemaining"]), acceso = true
          });
        }
      } catch { }

      // 3) l'uscita audio in uso: e' sempre roba collegata, e a lui interessa vederla
      try {
        var cerca = new ManagementObjectSearcher(
          "SELECT Name, Status FROM Win32_PnPEntity WHERE PNPClass = 'AudioEndpoint' AND Status = 'OK'");
        foreach (ManagementObject o in cerca.Get()) {
          string n = (string)o["Name"];
          if (n == null) continue;
          if (n.IndexOf("icrofon", StringComparison.OrdinalIgnoreCase) >= 0) continue;
          if (n.IndexOf("Mix", StringComparison.OrdinalIgnoreCase) >= 0) continue;
          fuori.Add(new Collegato { nome = Pulisci(n), via = "AUDIO", carica = null, acceso = true });
          break;
        }
      } catch { }

      // chi ha la percentuale sta in cima
      return fuori.OrderBy(c => c.carica.HasValue ? 0 : 1).ThenBy(c => c.nome).ToList();
    }

    // La percentuale, dove c'e', Windows la tiene nel registro fra le proprieta'
    // del dispositivo. Non esiste una via piu' diretta da codice normale.
    static int? Batteria(string idDispositivo) {
      try {
        string chiave = @"SYSTEM\CurrentControlSet\Enum\" + idDispositivo +
                        @"\Properties\{104EA319-6EE2-4701-BD47-8DDBF425BBE5}\0002";
        using (var k = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(chiave)) {
          if (k == null) return null;
          object v = k.GetValue("(Default)") ?? k.GetValue("");
          if (v == null) return null;
          int carica;
          if (v is byte[]) { byte[] b = (byte[])v; if (b.Length == 0) return null; carica = b[0]; }
          else carica = Convert.ToInt32(v);
          if (carica < 0 || carica > 100) return null;
          return carica;
        }
      } catch { }
      return null;
    }

    // Windows chiama le cose come gli pare: "Altoparlanti (Focusrite USB Audio)".
    // Qui resta solo il nome vero.
    public static string Pulisci(string n) {
      if (string.IsNullOrEmpty(n)) return "";
      n = n.Trim();
      var m = System.Text.RegularExpressions.Regex.Match(n, "\\(([^)]{3,})\\)");
      if (m.Success) n = m.Groups[1].Value;
      n = System.Text.RegularExpressions.Regex.Replace(n,
        "^(Altoparlanti|Speakers|Cuffie|Headphones|Auricolari)\\s*[-:]?\\s*", "");
      return System.Text.RegularExpressions.Regex.Replace(n, "\\s{2,}", " ").Trim();
    }

    // ---------- aprire una sessione ----------
    // Deve dare la stessa identica cosa che ottiene lui a mano: stesso terminale
    // (Windows Terminal, profilo predefinito) e soprattutto STESSA CARTELLA.
    public static void ApriClaude(string argomenti, string cartella) {
      if (string.IsNullOrEmpty(cartella) || !Directory.Exists(cartella))
        cartella = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string riga = string.IsNullOrEmpty(argomenti) ? "claude" : "claude " + argomenti;
      try {
        string wt = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                 "Microsoft\\WindowsApps\\wt.exe");
        if (File.Exists(wt)) {
          Process.Start(new ProcessStartInfo(wt,
            "-d \"" + cartella + "\" powershell.exe -NoExit -Command " + riga) { UseShellExecute = true });
        } else {
          Process.Start(new ProcessStartInfo("powershell.exe",
            "-NoExit -Command \"Set-Location -LiteralPath '" + cartella + "'; " + riga + "\"") { UseShellExecute = true });
        }
      } catch { }
    }
  }
}
