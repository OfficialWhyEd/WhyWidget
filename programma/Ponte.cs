// Il ponte con l'iPhone: quello che copia sul telefono finisce negli appunti del PC.
// Sta dentro il programma apposta: cosi' non c'e' un secondo affare da tenere acceso.
// Gira in un filo suo, altrimenti bloccherebbe la finestra.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace WhyWidget {
  public static class Ponte {
    public const int PORTA = 8787;

    public static bool Acceso;
    public static string Testo = "";
    public static DateTime? Quando;
    public static string Errore = "";
    public static string Chiave;

    static TcpListener ascolto;

    public static void Accendi() {
      LeggiChiave();
      Thread t = new Thread(Gira);
      t.IsBackground = true;
      t.SetApartmentState(ApartmentState.STA);   // senza STA gli appunti non si toccano
      t.Start();
    }

    // la chiave sta fuori dal progetto: quella cartella un giorno finisce su GitHub
    static void LeggiChiave() {
      try {
        Directory.CreateDirectory(Impostazioni.Cartella);
        string f = Path.Combine(Impostazioni.Cartella, "ponte.txt");
        if (File.Exists(f)) { Chiave = File.ReadAllText(f).Trim(); return; }
        byte[] b = new byte[9];
        System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(b);
        Chiave = Convert.ToBase64String(b).Replace("+", "-").Replace("/", "_").Replace("=", "");
        File.WriteAllText(f, Chiave);
      } catch { Chiave = "whyed"; }
    }

    static void Gira() {
      // se il programma e' appena stato riavviato, la porta puo' essere ancora
      // occupata da quello di prima: aspetta invece di arrendersi
      for (int tentativo = 1; tentativo <= 12 && ascolto == null; tentativo++) {
        try {
          ascolto = new TcpListener(IPAddress.Any, PORTA);
          ascolto.ExclusiveAddressUse = false;
          ascolto.Start();
          Acceso = true;
          Errore = "";
        } catch (Exception e) {
          ascolto = null;
          Errore = e.Message;
          Thread.Sleep(5000);
        }
      }
      if (ascolto == null) return;

      while (true) {
        try {
          using (TcpClient cliente = ascolto.AcceptTcpClient()) {
            cliente.ReceiveTimeout = 5000;
            Servi(cliente);
          }
        } catch { }
      }
    }

    static void Servi(TcpClient cliente) {
      NetworkStream f = cliente.GetStream();

      // la testa della richiesta e' ASCII, il corpo e' UTF-8: vanno tenuti byte
      // per byte, altrimenti gli accenti dell'iPhone arrivano rotti
      List<byte> tutto = new List<byte>();
      byte[] pezzo = new byte[8192];
      int stacco = -1;
      while (stacco < 0) {
        int letti = f.Read(pezzo, 0, pezzo.Length);
        if (letti <= 0) break;
        for (int i = 0; i < letti; i++) tutto.Add(pezzo[i]);
        for (int i = 0; i <= tutto.Count - 4; i++)
          if (tutto[i] == 13 && tutto[i + 1] == 10 && tutto[i + 2] == 13 && tutto[i + 3] == 10) { stacco = i; break; }
        if (tutto.Count > 2000000) break;
      }
      if (stacco < 0) return;

      string testa = Encoding.ASCII.GetString(tutto.ToArray(), 0, stacco);
      string[] righe = testa.Split(new[] { "\r\n" }, StringSplitOptions.None);

      int lunghezza = 0;
      foreach (string r in righe)
        if (r.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
          int.TryParse(r.Substring(15).Trim(), out lunghezza);

      int avuti = tutto.Count - (stacco + 4);
      while (avuti < lunghezza) {
        int letti = f.Read(pezzo, 0, pezzo.Length);
        if (letti <= 0) break;
        for (int i = 0; i < letti; i++) tutto.Add(pezzo[i]);
        avuti = tutto.Count - (stacco + 4);
      }
      string corpo = lunghezza > 0
        ? Encoding.UTF8.GetString(tutto.ToArray(), stacco + 4, Math.Min(lunghezza, avuti))
        : "";

      string[] pezzi = righe[0].Split(' ');
      string rotta = pezzi.Length > 1 ? pezzi[1] : "/";
      string percorso = rotta.Split('?')[0];

      // la chiave puo' stare nel percorso (/CHIAVE) o nella coda (?k=CHIAVE):
      // cosi' valgono anche le istruzioni gia' scritte per i Comandi rapidi
      string coda = rotta.IndexOf('?') >= 0 ? rotta.Substring(rotta.IndexOf('?') + 1) : "";
      string k = null;
      foreach (string parte in coda.Split('&'))
        if (parte.StartsWith("k=")) k = Uri.UnescapeDataString(parte.Substring(2));
      bool chiaveOk = percorso == "/" + Chiave || percorso == "/" + Chiave + "/" || k == Chiave;
      string metodo = pezzi[0].ToUpperInvariant();

      if (chiaveOk && (percorso == "/prendi" || (metodo == "GET" && percorso != "/manda"))) {
        // il PC manda i suoi appunti all'iPhone
        string appunti = "";
        try {
          Application.Current.Dispatcher.Invoke(new Action(delegate {
            try { appunti = Clipboard.ContainsText() ? Clipboard.GetText() : ""; } catch { }
          }));
        } catch { }
        Rispondi(f, appunti);
      } else if (chiaveOk) {
        Testo = corpo;
        Quando = DateTime.Now;
        try {
          Application.Current.Dispatcher.Invoke(new Action(delegate {
            try { Clipboard.SetText(corpo); } catch { }
          }));
        } catch { }
        // se e' un link, si apre anche da solo
        if (corpo.StartsWith("http://") || corpo.StartsWith("https://")) {
          try { System.Diagnostics.Process.Start(corpo.Trim()); } catch { }
        }
        Rispondi(f, "preso");
      } else {
        Rispondi(f, "WhyWidget");
      }
    }

    static void Rispondi(Stream f, string testo) {
      byte[] corpo = Encoding.UTF8.GetBytes(testo);
      byte[] testa = Encoding.ASCII.GetBytes(
        "HTTP/1.1 200 OK\r\nContent-Type: text/plain; charset=utf-8\r\n" +
        "Content-Length: " + corpo.Length + "\r\nConnection: close\r\n\r\n");
      f.Write(testa, 0, testa.Length);
      f.Write(corpo, 0, corpo.Length);
      f.Flush();
    }
  }
}
