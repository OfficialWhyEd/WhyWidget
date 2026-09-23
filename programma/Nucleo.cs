// Il motore, preso in prestito da come Apple fa i widget dell'iPhone.
//
//   iPhone                        qui
//   ----------------------------  --------------------------------------------
//   widget                        Tessera: una classe in Tessere.cs
//   TimelineProvider              il metodo Dati() della tessera
//   TimelineEntry                 quello che Dati() restituisce
//   reload policy (.after)        Scadenza: ogni tessera ha la sua, in secondi
//   WidgetFamily small/med/large  Misura: piccola / media / grande
//   la vista SwiftUI              il metodo Vista(): riceve i dati e disegna, e basta
//   galleria widget               il pannello: si accende, si spegne, si sceglie la misura
//   estensione fuori dall'app     la cucina: i dati si preparano in un filo a parte
//
// Tre regole che vengono da li' e qui valgono uguale:
//   1. la Vista non va a cercarsi i dati: li riceve gia' pronti. Non si blocca mai.
//   2. ogni tessera si rinfresca quando scade LA SUA roba, non tutte insieme.
//   3. la misura non e' solo grandezza: e' quanta roba mostrare.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace WhyWidget {

  public abstract class Tessera {
    public abstract string Nome { get; }
    public abstract string Titolo { get; }
    public virtual string Icona { get { return null; } }
    public virtual string Kana { get { return null; } }
    public virtual string[] Misure { get { return new[] { "piccola", "media", "grande" }; } }
    public virtual int Scadenza { get { return 60; } }   // secondi
    public virtual bool Fissa { get { return false; } }  // se vera, non si puo' spegnere

    // come sta nella griglia: "meta" si affianca a un'altra meta', "piena" prende la riga
    public virtual string Larghezza { get { return "meta"; } }
    // altezza fissa del riquadro; 0 vuol dire alto quanto serve
    public virtual double Altezza { get { return 0; } }
    // solo RIPRENDI e' rossa: e' il tasto piu' importante
    public virtual bool Rossa { get { return false; } }
    // cosa succede se ci clicchi sopra. Di suo, niente.
    public virtual void Tocco() { }

    // il fornitore: gira nella cucina, non nella finestra. Puo' metterci secondi.
    public virtual object Dati() { return null; }

    // la vista: riceve i dati gia' pronti e disegna. Non legge niente da sola.
    public abstract UIElement Vista(object dati, string misura);

    // quante righe a seconda della misura. Piu' grande vuol dire piu' roba,
    // non caratteri piu' grossi: e' il punto dei widget dell'iPhone.
    protected static int Quante(string misura, int piccola, int media, int grande) {
      if (misura == "piccola") return piccola;
      if (misura == "grande") return grande;
      return media;
    }
  }

  public static class Nucleo {
    public static readonly List<Tessera> Elenco = new List<Tessera>();
    public static readonly Dictionary<string, Tessera> PerNome = new Dictionary<string, Tessera>();

    // quello che la cucina ha pronto, e quando va rifatto
    public static readonly Dictionary<string, object> Roba = new Dictionary<string, object>();
    public static readonly Dictionary<string, DateTime> Scadenze = new Dictionary<string, DateTime>();
    public static readonly Dictionary<string, long> Versione = new Dictionary<string, long>();
    static readonly object chiave = new object();
    static Thread cucina;

    public static void Registra(Tessera t) {
      Elenco.Add(t);
      PerNome[t.Nome] = t;
    }

    public static object Prendi(string nome) {
      lock (chiave) { return Roba.ContainsKey(nome) ? Roba[nome] : null; }
    }

    public static long VersioneDi(string nome) {
      lock (chiave) { return Versione.ContainsKey(nome) ? Versione[nome] : 0; }
    }

    static void Cucina(Tessera t) {
      object d = t.Dati();
      lock (chiave) {
        Roba[t.Nome] = d;
        Scadenze[t.Nome] = DateTime.Now.AddSeconds(t.Scadenza);
        Versione[t.Nome] = VersioneDi(t.Nome) + 1;
      }
    }

    // la prima infornata, subito: all'apertura il widget deve essere gia' pieno,
    // non vuoto per qualche secondo
    public static void PrimaInfornata() {
      foreach (Tessera t in Elenco) {
        try { Cucina(t); } catch { }
      }
    }

    public static void AccendiCucina() {
      cucina = new Thread(delegate() {
        while (true) {
          foreach (Tessera t in Elenco) {
            try {
              DateTime scade;
              lock (chiave) { Scadenze.TryGetValue(t.Nome, out scade); }
              if (scade > DateTime.Now) continue;
              Cucina(t);
            } catch {
              lock (chiave) { Scadenze[t.Nome] = DateTime.Now.AddSeconds(30); }
            }
          }
          Thread.Sleep(900);
        }
      });
      cucina.IsBackground = true;
      cucina.SetApartmentState(ApartmentState.STA);
      cucina.Start();
    }
  }
}
