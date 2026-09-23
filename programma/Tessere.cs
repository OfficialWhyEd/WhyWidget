// Le tessere: una per riquadro del widget. Ognuna sa da sola dove prende i dati,
// ogni quanto scadono e cosa mostrare in ciascuna misura.
//
// La forma e' quella del widget vero: griglia di riquadri di vetro come il Centro
// di Controllo dell'iPhone. Le tessere "meta'" stanno affiancate a due a due,
// quelle "piena" prendono la riga intera.
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WhyWidget {

  // ---------------------------------------------------------------- ORA
  public class TesseraOra : Tessera {
    public override string Nome { get { return "ora"; } }
    public override string Titolo { get { return "ORA"; } }
    public override string Icona { get { return "ora"; } }
    public override string Larghezza { get { return "meta"; } }
    public override double Altezza { get { return 118; } }
    public override string[] Misure { get { return new[] { "piccola", "media" }; } }
    public override int Scadenza { get { return 20; } }
    public override bool Fissa { get { return true; } }
    public override object Dati() { return DateTime.Now; }

    public override UIElement Vista(object dati, string misura) {
      StackPanel sp = new StackPanel();
      sp.VerticalAlignment = VerticalAlignment.Center;

      TextBlock ora = Pelle.Numero(DateTime.Now.ToString("HH:mm"), misura == "piccola" ? 32 : 40);
      ora.Margin = new Thickness(0, 0, 0, -2);
      sp.Children.Add(ora);

      var it = new System.Globalization.CultureInfo("it-IT");
      TextBlock data = Pelle.Etichetta(DateTime.Now.ToString("ddd d MMM", it).ToUpper());
      data.HorizontalAlignment = HorizontalAlignment.Left;
      sp.Children.Add(data);
      return sp;
    }
  }

  // ------------------------------------------------------------ RIPRENDI
  // L'unico riquadro con un colore suo: e' il tasto piu' importante del widget.
  public class TesseraRiprendi : Tessera {
    public override string Nome { get { return "riprendi"; } }
    public override string Titolo { get { return "RIPRENDI"; } }
    public override string Icona { get { return "riprendi"; } }
    public override string Larghezza { get { return "meta"; } }
    public override double Altezza { get { return 118; } }
    public override bool Rossa { get { return true; } }
    public override string[] Misure { get { return new[] { "piccola", "media" }; } }
    public override int Scadenza { get { return 20; } }
    public override object Dati() { return Fonte.Sessioni(1); }

    public override UIElement Vista(object dati, string misura) {
      var lista = dati as List<Sessione>;
      Sessione ultima = (lista != null && lista.Count > 0) ? lista[0] : null;

      StackPanel sp = new StackPanel();
      sp.VerticalAlignment = VerticalAlignment.Center;

      ContentControl box = new ContentControl();
      box.Content = Icone.Prendi("riprendi", Pelle.Testo, 24);
      box.HorizontalAlignment = HorizontalAlignment.Left;
      sp.Children.Add(box);

      TextBlock et = Pelle.Testo_("RIPRENDI", "#FFFFFF", 14);
      et.FontWeight = FontWeights.SemiBold;
      et.Margin = new Thickness(0, 10, 0, 0);
      sp.Children.Add(et);

      TextBlock q = Pelle.Testo_(ultima != null ? ultima.quando : "NESSUNA", "#B3FFFFFF", 10.5);
      q.FontWeight = FontWeights.SemiBold;
      q.Margin = new Thickness(0, 1, 0, 0);
      sp.Children.Add(q);
      return sp;
    }

    public override void Tocco() {
      Fonte.ApriClaude("--continue", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }
  }

  // ------------------------------------------------------------ SESSIONI
  public class TesseraSessioni : Tessera {
    public override string Nome { get { return "sessioni"; } }
    public override string Titolo { get { return "LE ULTIME SESSIONI"; } }
    public override string Larghezza { get { return "piena"; } }   // come nella foto giusta: tutta la riga
    public override string Icona { get { return "sessioni"; } }
    public override int Scadenza { get { return 25; } }
    public override object Dati() { return Fonte.Sessioni(13); }

    public override UIElement Vista(object dati, string misura) {
      var lista = dati as List<Sessione>;
      int quante = Quante(misura, 2, 3, 8);

      StackPanel sp = new StackPanel();
      sp.Children.Add(Pelle.Testa(Icona, Titolo, 10));

      int messe = 0;
      if (lista != null) {
        for (int k = 1; k < lista.Count && messe < quante; k++, messe++) {
          Sessione s = lista[k];
          string id = s.id, dove = s.dove;

          TextBlock t = Pelle.Testo_(s.titolo, Pelle.Testo, 12);
          t.TextTrimming = TextTrimming.CharacterEllipsis;
          TextBlock q = Pelle.Testo_(s.quando, Pelle.TenueC, 10);
          q.Margin = new Thickness(10, 1, 0, 0);

          Grid riga = Pelle.Riga(t, q, 7);
          riga.Cursor = System.Windows.Input.Cursors.Hand;
          riga.Background = Brushes.Transparent;
          riga.MouseLeftButtonUp += delegate { Fonte.ApriClaude("--resume " + id, dove); };
          riga.MouseEnter += delegate { t.Foreground = Pelle.Colore("#FFFFFF"); };
          riga.MouseLeave += delegate { t.Foreground = Pelle.Colore(Pelle.Testo); };
          sp.Children.Add(riga);
        }
      }
      if (messe == 0) sp.Children.Add(Pelle.Testo_("nessuna sessione", Pelle.TenueC, 11));
      return sp;
    }
  }

  // -------------------------------------------------------------- AGENTI
  public class TesseraAgenti : Tessera {
    public override string Nome { get { return "agenti"; } }
    public override string Titolo { get { return "AGENTI"; } }
    public override string Icona { get { return "agenti"; } }
    public override string Larghezza { get { return "meta"; } }
    public override double Altezza { get { return 132; } }
    public override int Scadenza { get { return 300; } }
    public override object Dati() { return Fonte.Agenti(); }

    public override UIElement Vista(object dati, string misura) {
      var lista = dati as List<string>;
      int quanti = Quante(misura, 3, 8, 20);

      StackPanel sp = new StackPanel();
      sp.Children.Add(Pelle.Testa(Icona, Titolo, 9));

      WrapPanel wp = new WrapPanel();
      if (lista != null) {
        for (int i = 0; i < lista.Count && i < quanti; i++) {
          string nome = lista[i];
          wp.Children.Add(Pelle.Pastiglia(nome, delegate {
            Fonte.ApriClaude("\"usa l'agente " + nome + "\"",
                             Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
          }));
        }
      }
      sp.Children.Add(wp);
      return sp;
    }
  }

  // -------------------------------------------------------------- LAVORI
  public class TesseraLavori : Tessera {
    public override string Nome { get { return "lavori"; } }
    public override string Titolo { get { return "DA FARE"; } }
    public override string Icona { get { return "lavori"; } }
    public override string Larghezza { get { return "meta"; } }
    public override double Altezza { get { return 132; } }
    public override int Scadenza { get { return 30; } }
    public override object Dati() { return Fonte.Lavori(); }

    public override UIElement Vista(object dati, string misura) {
      var mappa = dati as Dictionary<string, object>;
      List<string> ora = new List<string>();
      if (mappa != null && mappa.ContainsKey("voci")) {
        var voci = mappa["voci"] as System.Collections.ArrayList;
        if (voci != null)
          foreach (var v in voci) {
            var d = v as Dictionary<string, object>;
            if (d != null && d.ContainsKey("s") && (string)d["s"] == "ora" && d.ContainsKey("t"))
              ora.Add((string)d["t"]);
          }
      }

      StackPanel sp = new StackPanel();
      sp.Children.Add(Pelle.Testa(Icona, mappa != null ? Titolo : "LISTA SPENTA", 0));

      TextBlock conta = Pelle.Numero(mappa != null ? ora.Count.ToString() : "--", 38);
      conta.Margin = new Thickness(0, 2, 0, 2);
      sp.Children.Add(conta);

      int quanti = Quante(misura, 1, 2, 5);
      for (int i = 0; i < ora.Count && i < quanti; i++) {
        TextBlock t = Pelle.Testo_(ora[i], "#8FFFFFFF", 10.5);
        t.TextTrimming = TextTrimming.CharacterEllipsis;
        t.Margin = new Thickness(0, 0, 0, 3);
        sp.Children.Add(t);
      }
      return sp;
    }

    public override void Tocco() {
      try { System.Diagnostics.Process.Start(Fonte.BASE + "/lavori"); } catch { }
    }
  }

  // ----------------------------------------------------------- COLLEGATI
  public class TesseraCollegati : Tessera {
    public override string Nome { get { return "collegati"; } }
    public override string Titolo { get { return "COLLEGATI"; } }
    public override string Icona { get { return "dispositivi"; } }
    public override string Larghezza { get { return "meta"; } }
    public override double Altezza { get { return 118; } }
    public override int Scadenza { get { return 60; } }
    public override object Dati() { return Fonte.Collegati(); }

    public override UIElement Vista(object dati, string misura) {
      var lista = dati as List<Collegato>;
      int quanti = Quante(misura, 2, 3, 6);

      StackPanel sp = new StackPanel();
      sp.Children.Add(Pelle.Testa(Icona, Titolo, 9));

      int messe = 0;
      if (lista != null) {
        for (int i = 0; i < lista.Count && i < quanti; i++, messe++) {
          Collegato d = lista[i];
          TextBlock nome = Pelle.Testo_(d.nome, Pelle.Testo, 10.5);
          nome.TextTrimming = TextTrimming.CharacterEllipsis;
          nome.VerticalAlignment = VerticalAlignment.Center;

          StackPanel destra = new StackPanel();
          destra.Orientation = Orientation.Horizontal;
          destra.VerticalAlignment = VerticalAlignment.Center;
          destra.Margin = new Thickness(8, 0, 0, 0);

          if (d.carica.HasValue) {
            Grid barra = Pelle.Barra(d.carica.Value, 24);
            barra.Margin = new Thickness(0, 0, 6, 0);
            destra.Children.Add(barra);
            TextBlock p = Pelle.Testo_(d.carica.Value + "%", Pelle.Testo, 9.5);
            p.VerticalAlignment = VerticalAlignment.Center;
            destra.Children.Add(p);
          } else {
            destra.Children.Add(Pelle.Pallino(d.acceso, 5));
          }
          sp.Children.Add(Pelle.Riga(nome, destra, 6));
        }
      }
      if (messe == 0) sp.Children.Add(Pelle.Testo_("sto guardando...", Pelle.TenueC, 10.5));
      return sp;
    }
  }

  // -------------------------------------------------------------- IPHONE
  public class TesseraIphone : Tessera {
    public override string Nome { get { return "iphone"; } }
    public override string Titolo { get { return "IPHONE"; } }
    public override string Icona { get { return "iphone"; } }
    public override string Larghezza { get { return "meta"; } }
    public override double Altezza { get { return 118; } }
    public override string[] Misure { get { return new[] { "piccola", "media" }; } }
    public override int Scadenza { get { return 3; } }
    public override object Dati() { return null; }   // lo stato ce l'ha gia' il ponte

    public override UIElement Vista(object dati, string misura) {
      string testo, stato;
      if (Ponte.Acceso) {
        testo = Ponte.Quando.HasValue ? Pelle.Taglia(Ponte.Testo, 40) : "copia sul telefono, si incolla qui";
        stato = Ponte.Quando.HasValue ? "ARRIVATO " + Fonte.Quando(Ponte.Quando.Value) : "PRONTO";
      } else {
        testo = "ponte spento";
        stato = string.IsNullOrEmpty(Ponte.Errore) ? "NON PARTITO" : "PORTA OCCUPATA";
      }

      StackPanel sp = new StackPanel();
      StackPanel testa = Pelle.Testa(Icona, Titolo, 9);
      Ellipse q = Pelle.Pallino(Ponte.Acceso, 5);
      q.Margin = new Thickness(7, 0, 0, 0);
      testa.Children.Add(q);
      sp.Children.Add(testa);

      TextBlock t1 = Pelle.Testo_(testo, "#B3FFFFFF", 10.5);
      t1.TextWrapping = TextWrapping.Wrap;
      t1.LineHeight = 14;
      sp.Children.Add(t1);

      TextBlock t2 = Pelle.Testo_(stato, Pelle.TenueC, 9.5);
      t2.FontWeight = FontWeights.SemiBold;
      t2.Margin = new Thickness(0, 4, 0, 0);
      sp.Children.Add(t2);
      return sp;
    }

    // un tocco rimette negli appunti l'ultima cosa arrivata dal telefono
    public override void Tocco() {
      if (!string.IsNullOrEmpty(Ponte.Testo)) {
        try { Clipboard.SetText(Ponte.Testo); } catch { }
      }
    }
  }
}
