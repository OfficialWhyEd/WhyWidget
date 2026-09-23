// La pelle: tutti i mattoni grafici stanno qui e solo qui.
// I valori sono quelli del widget vero: vetro liquido, angoli molto tondi,
// il desktop che si vede attraverso. Le tessere non scelgono colori ne' misure:
// chiedono un mattone e lo ricevono gia' vestito.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WhyWidget {
  public static class Pelle {
    // --- i colori del vetro: quasi tutti bianchi trasparenti, cosi' il desktop passa ---
    public const string Testo   = "#F2FFFFFF";   // quello che si legge
    public const string Mezzo   = "#9EFFFFFF";   // le etichette
    public const string TenueC  = "#5CFFFFFF";   // i tempi, le cose di contorno
    public const string Vetro   = "#1CFFFFFF";   // la tessera
    public const string VetroSu = "#2EFFFFFF";   // la tessera col mouse sopra, e i bordi
    public const string Pill    = "#14FFFFFF";   // le pastiglie degli agenti
    public const string Rosso   = "#FF5A6B";     // l'unico colore
    public const string Rossa   = "#33FF5A6B";   // la tessera RIPRENDI
    public const string RossaBo = "#4DFF8A96";   // il suo bordo

    public const double Raggio  = 22;   // l'arrotondamento: e' quello che fa il vetro
    public const double Dentro  = 16;   // il margine dentro le tessere
    public const double Buco    = 5;    // lo spazio fra una tessera e l'altra
    public const double Colonna = 164;  // quanto e' larga mezza griglia

    public static readonly FontFamily Grande  = new FontFamily("Segoe UI Variable Display, Segoe UI Light");
    public static readonly FontFamily Normale = new FontFamily("Segoe UI");

    public static Brush Colore(string esa) {
      return (Brush)new BrushConverter().ConvertFromString(esa);
    }

    public static TextBlock Testo_(string testo, string colore, double misura) {
      TextBlock t = new TextBlock();
      t.Text = testo ?? "";
      t.Foreground = Colore(colore);
      t.FontSize = misura;
      t.FontFamily = Normale;
      return t;
    }

    // l'etichetta piccola in cima a una tessera
    public static TextBlock Etichetta(string testo) {
      TextBlock t = Testo_(testo, TenueC, 10.5);
      t.FontWeight = FontWeights.SemiBold;
      t.VerticalAlignment = VerticalAlignment.Center;
      return t;
    }

    // il numero grosso: l'ora, il conto dei lavori
    public static TextBlock Numero(string testo, double misura) {
      TextBlock t = Testo_(testo, "#FFFFFF", misura);
      t.FontFamily = Grande;
      t.FontWeight = FontWeights.Thin;
      return t;
    }

    // La tessera: il mattone di tutta la griglia. Col mouse sopra si stringe appena
    // e si schiarisce: e' la stessa risposta dei tasti dell'iPhone.
    public static Border Tessera(UIElement dentro, double altezza, bool rossa) {
      Border b = new Border();
      b.Background = Colore(rossa ? Rossa : Vetro);
      b.BorderBrush = Colore(rossa ? RossaBo : VetroSu);
      b.BorderThickness = new Thickness(1);
      b.CornerRadius = new CornerRadius(Raggio);
      b.Padding = new Thickness(Dentro);
      b.Margin = new Thickness(Buco);
      if (altezza > 0) b.Height = altezza;
      b.Child = dentro;

      ScaleTransform s = new ScaleTransform(1, 1);
      b.RenderTransformOrigin = new Point(0.5, 0.5);
      b.RenderTransform = s;
      bool r = rossa;
      b.MouseEnter += delegate {
        if (!r) b.Background = Colore(VetroSu);
        s.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.975, new Duration(TimeSpan.FromMilliseconds(140))));
        s.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.975, new Duration(TimeSpan.FromMilliseconds(140))));
      };
      b.MouseLeave += delegate {
        if (!r) b.Background = Colore(Vetro);
        s.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(220))));
        s.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(220))));
      };
      Premuta(b, s, 0.975, 0.95);
      return b;
    }

    // il tocco: si stringe un pelo di piu' col tasto giu', e torna su col rilascio.
    // E' la risposta che manca ai widget finti e che c'e' su tutti i tasti dell'iPhone
    public static void Premuta(UIElement e, ScaleTransform s, double riposo, double giu) {
      e.MouseLeftButtonDown += delegate {
        s.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(giu, new Duration(TimeSpan.FromMilliseconds(80))));
        s.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(giu, new Duration(TimeSpan.FromMilliseconds(80))));
      };
      e.MouseLeftButtonUp += delegate {
        DoubleAnimation su = new DoubleAnimation(riposo, new Duration(TimeSpan.FromMilliseconds(260)));
        BackEase molla = new BackEase();
        molla.EasingMode = EasingMode.EaseOut;
        molla.Amplitude = 0.6;
        su.EasingFunction = molla;
        s.BeginAnimation(ScaleTransform.ScaleXProperty, su);
        s.BeginAnimation(ScaleTransform.ScaleYProperty, su);
      };
    }

    // la testa di una tessera: icona che si muove piu' etichetta
    public static StackPanel Testa(string icona, string etichetta, double sotto) {
      StackPanel r = new StackPanel();
      r.Orientation = Orientation.Horizontal;
      r.Margin = new Thickness(0, 0, 0, sotto);
      UIElement ic = Icone.Prendi(icona, Mezzo, 14);
      if (ic != null) {
        ContentControl box = new ContentControl();
        box.Content = ic;
        box.Margin = new Thickness(0, 0, 7, 0);
        box.VerticalAlignment = VerticalAlignment.Center;
        r.Children.Add(box);
      }
      r.Children.Add(Etichetta(etichetta));
      return r;
    }

    // pastiglia arrotondata, come i nomi degli agenti
    public static Border Pastiglia(string testo, Action azione) {
      Border b = new Border();
      b.Background = Colore(Pill);
      b.BorderBrush = Colore("#26FFFFFF");
      b.BorderThickness = new Thickness(1);
      b.CornerRadius = new CornerRadius(9);
      b.Padding = new Thickness(8, 3, 8, 4);
      b.Margin = new Thickness(0, 0, 5, 5);
      b.Cursor = System.Windows.Input.Cursors.Hand;
      b.Child = Testo_(testo, Testo, 10.5);
      if (azione != null) b.MouseLeftButtonUp += delegate { azione(); };
      b.MouseEnter += delegate { b.Background = Colore("#38FFFFFF"); };
      b.MouseLeave += delegate { b.Background = Colore(Pill); };
      return b;
    }

    // bottone acceso o spento, per il pannello
    public static Border Bottone(string testo, bool acceso, Action azione) {
      Border b = new Border();
      b.Background = Colore(acceso ? "#38FFFFFF" : Pill);
      b.BorderBrush = Colore(acceso ? "#5CFFFFFF" : "#26FFFFFF");
      b.BorderThickness = new Thickness(1);
      b.CornerRadius = new CornerRadius(9);
      b.Padding = new Thickness(9, 3, 9, 4);
      b.Margin = new Thickness(0, 0, 5, 5);
      b.Cursor = System.Windows.Input.Cursors.Hand;
      b.Child = Testo_(testo, acceso ? "#FFFFFF" : Mezzo, 10.5);
      if (azione != null) b.MouseLeftButtonUp += delegate { azione(); };
      return b;
    }

    // riga a due colonne: qualcosa a sinistra, qualcosa a destra
    public static Grid Riga(UIElement sinistra, UIElement destra, double sotto) {
      Grid g = new Grid();
      g.Margin = new Thickness(0, 0, 0, sotto);
      g.ColumnDefinitions.Add(new ColumnDefinition());
      ColumnDefinition c2 = new ColumnDefinition();
      c2.Width = GridLength.Auto;
      g.ColumnDefinitions.Add(c2);
      Grid.SetColumn(sinistra, 0);
      g.Children.Add(sinistra);
      if (destra != null) { Grid.SetColumn(destra, 1); g.Children.Add(destra); }
      return g;
    }

    // pallino di stato: acceso rosso, spento tenue
    public static Ellipse Pallino(bool acceso, double misura) {
      Ellipse q = new Ellipse();
      q.Width = misura; q.Height = misura;
      q.Fill = Colore(acceso ? Rosso : TenueC);
      q.VerticalAlignment = VerticalAlignment.Center;
      return q;
    }

    // barra della carica: bianca, e rossa solo quando e' agli sgoccioli
    public static Grid Barra(int percento, double larghezza) {
      Grid g = new Grid();
      g.Width = larghezza; g.Height = 4;
      g.VerticalAlignment = VerticalAlignment.Center;
      Rectangle vuoto = new Rectangle();
      vuoto.Fill = Colore("#26FFFFFF"); vuoto.RadiusX = 2; vuoto.RadiusY = 2;
      Rectangle pieno = new Rectangle();
      pieno.Fill = Colore(percento <= 20 ? Rosso : Testo);
      pieno.RadiusX = 2; pieno.RadiusY = 2;
      pieno.Width = Math.Max(2, larghezza * percento / 100.0);
      pieno.HorizontalAlignment = HorizontalAlignment.Left;
      g.Children.Add(vuoto); g.Children.Add(pieno);
      return g;
    }

    // taglia sull'ultima parola intera: "sessione di lavo..." e' brutto, "sessione di..." no
    public static string Taglia(string t, int n) {
      if (string.IsNullOrEmpty(t)) return "";
      t = System.Text.RegularExpressions.Regex.Replace(t, "\\s+", " ").Trim();
      if (t.Length <= n) return t;
      string pezzo = t.Substring(0, n);
      int spazio = pezzo.LastIndexOf(' ');
      if (spazio > (int)(n * 0.55)) pezzo = pezzo.Substring(0, spazio);
      return pezzo.TrimEnd(' ', ',', '.', ';', ':') + "…";
    }

    // entra sfumando, con il suo turno: e' quello che da' il ritmo all'apertura
    public static void Entra(UIElement elemento, int ritardo) {
      elemento.Opacity = 0;
      DoubleAnimation a = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300)));
      a.BeginTime = TimeSpan.FromMilliseconds(ritardo);
      elemento.BeginAnimation(UIElement.OpacityProperty, a);
    }
  }
}
