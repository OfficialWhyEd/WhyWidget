// Le impostazioni: stanno in un file JSON fuori dal programma, in AppData.
// Cosi' restano anche se la cartella del progetto finisce su GitHub.
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace WhyWidget {

  public class ContoTessera {
    public bool acceso = true;
    public string misura = "media";
  }

  public class Impostazioni {
    public string lato = "destra";        // destra o sinistra
    public string alto = "sopra";         // sopra o sotto
    // il verso: a colonna come sempre, o una riga sola per lasciare libero il desktop.
    // Resta nello stesso angolo: l'orario non si muove, e' il resto che si srotola
    public string verso = "verticale";    // verticale o orizzontale
    public int largo = 356;
    public string altezza = "contenuto";  // contenuto o colonna
    public double opacita = 1.0;
    public bool avvioConWindows = false;

    // l'ordine conta: e' l'ordine in cui le tessere stanno nella colonna
    public List<string> ordine = new List<string>();
    public Dictionary<string, ContoTessera> tessere = new Dictionary<string, ContoTessera>();

    public static string Cartella {
      get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WhyWidget"); }
    }
    static string File_ { get { return Path.Combine(Cartella, "impostazioni.json"); } }

    public static Impostazioni Leggi() {
      Impostazioni i = new Impostazioni();
      try {
        if (File.Exists(File_)) {
          var s = new JavaScriptSerializer();
          i = s.Deserialize<Impostazioni>(File.ReadAllText(File_)) ?? new Impostazioni();
        }
      } catch { i = new Impostazioni(); }

      // paletti: nessun valore assurdo puo' far sparire il widget dallo schermo
      if (i.largo < 280) i.largo = 280;
      if (i.largo > 620) i.largo = 620;
      if (i.opacita < 0.5) i.opacita = 0.5;
      if (i.opacita > 1) i.opacita = 1;
      if (i.lato != "sinistra") i.lato = "destra";
      if (i.alto != "sotto") i.alto = "sopra";
      if (i.verso != "orizzontale") i.verso = "verticale";
      if (i.altezza != "colonna") i.altezza = "contenuto";
      if (i.ordine == null) i.ordine = new List<string>();
      if (i.tessere == null) i.tessere = new Dictionary<string, ContoTessera>();
      return i;
    }

    // comodo per chi disegna
    [ScriptIgnore] public bool Orizzontale { get { return verso == "orizzontale"; } }

    public void Salva() {
      try {
        Directory.CreateDirectory(Cartella);
        File.WriteAllText(File_, new JavaScriptSerializer().Serialize(this));
      } catch { }
    }

    // se domani aggiungo una tessera nuova, i file gia' salvati continuano a funzionare:
    // quello che manca si aggiunge in coda con i valori di partenza
    public ContoTessera Conto(string nome) {
      if (!tessere.ContainsKey(nome)) tessere[nome] = new ContoTessera();
      if (!ordine.Contains(nome)) ordine.Add(nome);
      return tessere[nome];
    }

    public void Sposta(string nome, int verso) {
      int i = ordine.IndexOf(nome);
      int j = i + verso;
      if (i < 0 || j < 0 || j >= ordine.Count) return;
      ordine[i] = ordine[j];
      ordine[j] = nome;
    }
  }
}
