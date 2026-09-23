// La finestra: la griglia di riquadri di vetro, e il pannello per cambiarli.
// Sta appoggiata sul desktop: non copre le finestre, non e' in Alt+Tab, non e'
// nella barra. Il vetro lo fa Windows dietro di lei, non un'immagine finta.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Drawing.Imaging;

namespace WhyWidget {

  public class Finestra : Window {
    Impostazioni imp;
    Grid griglia;            // dove stanno i riquadri
    StackPanel comandi;      // dentro il pannello
    Border pannello;
    ScaleTransform entrata;
    readonly Dictionary<string, long> visto = new Dictionary<string, long>();
    readonly Dictionary<string, ContentControl> caselle = new Dictionary<string, ContentControl>();
    int minuto = -1;
    double altezzaRiga = 0;  // in riga tutte le tessere sono alte uguali; 0 vuol dire ognuna la sua
    Border tastoVerso;       // il tondino che cambia verso
    bool inMovimento = false; // durante il cambio di verso non si riparte
    bool inPosa = false;     // durante il ritratto resta sopra a tutto, senno' la foto viene coperta

    // ---------- vetro liquido, angoli tondi, appoggio sul desktop ----------
    [StructLayout(LayoutKind.Sequential)]
    struct ACCENTPOLICY { public int Stato; public int Flag; public int Colore; public int Anima; }
    [StructLayout(LayoutKind.Sequential)]
    struct WINCOMPATTRDATA { public int Attributo; public IntPtr Dati; public int Misura; }
    [DllImport("user32.dll")] static extern int SetWindowCompositionAttribute(IntPtr h, ref WINCOMPATTRDATA d);
    [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int a, ref int v, int s);
    [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr h, IntPtr dopo, int x, int y, int cx, int cy, uint f);
    [DllImport("user32.dll")] static extern int GetWindowLong(IntPtr h, int i);
    [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr h, int i, int v);

    const int ACRILICO = 4, SFOCATO = 3, ATTR_ACCENT = 19, ANGOLI = 33, TONDO = 2;
    static readonly IntPtr BOTTOM = new IntPtr(1);
    const uint NOSIZE = 0x0001, NOMOVE = 0x0002, NOZORDER = 0x0004, NOACTIVATE = 0x0010, SHOWWINDOW = 0x0040;
    const int EXSTYLE = -20, TOOLWINDOW = 0x00000080, NOACTIVATE_EX = 0x08000000;

    void Vetro(IntPtr h, int tinta) {
      ACCENTPOLICY p = new ACCENTPOLICY();
      p.Stato = ACRILICO; p.Flag = 2; p.Colore = tinta; p.Anima = 0;
      int misura = Marshal.SizeOf(p);
      IntPtr dati = Marshal.AllocHGlobal(misura);
      Marshal.StructureToPtr(p, dati, false);
      WINCOMPATTRDATA d = new WINCOMPATTRDATA();
      d.Attributo = ATTR_ACCENT; d.Dati = dati; d.Misura = misura;
      if (SetWindowCompositionAttribute(h, ref d) == 0) {
        // su qualche macchina l'acrilico non c'e': si ripiega sulla sfocatura
        p.Stato = SFOCATO;
        Marshal.StructureToPtr(p, dati, false);
        SetWindowCompositionAttribute(h, ref d);
      }
      Marshal.FreeHGlobal(dati);
      int tondo = TONDO;
      DwmSetWindowAttribute(h, ANGOLI, ref tondo, sizeof(int));
    }

    void Appoggia() {
      if (inPosa) return;
      IntPtr h = new WindowInteropHelper(this).Handle;
      if (h == IntPtr.Zero) return;
      int s = GetWindowLong(h, EXSTYLE);
      SetWindowLong(h, EXSTYLE, s | TOOLWINDOW | NOACTIVATE_EX);
      SetWindowPos(h, BOTTOM, 0, 0, 0, 0, NOSIZE | NOMOVE | NOACTIVATE | SHOWWINDOW);
    }

    public Finestra(Impostazioni i) {
      imp = i;

      WindowStyle = WindowStyle.None;
      AllowsTransparency = false;      // serve falso, altrimenti il vetro non si applica
      Background = Colore("#01000000");
      ShowInTaskbar = false;
      Topmost = false;
      ResizeMode = ResizeMode.NoResize;
      SizeToContent = SizeToContent.WidthAndHeight;
      Opacity = 0;

      Grid tutto = new Grid();

      griglia = new Grid();
      griglia.Margin = new Thickness(6);
      entrata = new ScaleTransform(0.94, 0.94);
      griglia.RenderTransformOrigin = new Point(0.5, 0.5);
      griglia.RenderTransform = entrata;
      griglia.HorizontalAlignment = HorizontalAlignment.Left;   // durante il cambio di verso la finestra
      griglia.VerticalAlignment = VerticalAlignment.Top;        // ha una misura sua: la griglia parte dall angolo
      tutto.Children.Add(griglia);

      pannello = CostruisciPannello();
      tutto.Children.Add(pannello);

      Content = tutto;

      Loaded += delegate {
        DisegnaGriglia();
        UpdateLayout();
        MettiAPosto();

        IntPtr h = new WindowInteropHelper(this).Handle;
        Vetro(h, unchecked((int)0x4D14141A));
        Appoggia();

        BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(320))));
        DoubleAnimation su = new DoubleAnimation(0.94, 1, new Duration(TimeSpan.FromMilliseconds(520)));
        BackEase ease = new BackEase();
        ease.EasingMode = EasingMode.EaseOut;
        ease.Amplitude = 0.35;
        su.EasingFunction = ease;
        entrata.BeginAnimation(ScaleTransform.ScaleXProperty, su);
        entrata.BeginAnimation(ScaleTransform.ScaleYProperty, su);
      };

      // un colpo d'occhio al secondo: costa niente, perche' ridisegna solo
      // i riquadri che hanno davvero roba nuova
      DispatcherTimer timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromSeconds(1);
      timer.Tick += delegate { Giro(); Appoggia(); };
      timer.Start();
    }

    static Brush Colore(string esa) { return Pelle.Colore(esa); }

    static ColumnDefinition Colonna() {
      ColumnDefinition c = new ColumnDefinition();
      c.Width = new GridLength(Pelle.Colonna);
      return c;
    }

    // ================= il ritratto =================
    // Fa la foto della finestra com'e' sullo schermo (vetro compreso) e chiude.
    // La uso io per guardare il risultato con i miei occhi prima di dire fatto.
    public void Ritratto(string file) {
      inPosa = true;
      Topmost = true;
      Loaded += delegate {
        DispatcherTimer t = new DispatcherTimer();
        t.Interval = TimeSpan.FromMilliseconds(1500);   // le animazioni d'entrata finiscono
        t.Tick += delegate {
          t.Stop();
          try {
            int x = (int)Left - 12, y = (int)Top - 12;
            int w = (int)ActualWidth + 24, h = (int)ActualHeight + 24;
            using (System.Drawing.Bitmap b = new System.Drawing.Bitmap(w, h))
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b)) {
              g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(w, h));
              b.Save(file, ImageFormat.Png);
            }
          } catch (Exception e) {
            try { System.IO.File.WriteAllText(file + ".errore.txt", e.ToString()); } catch { }
          }
          System.Windows.Application.Current.Shutdown();
        };
        t.Start();
      };
    }

    public void MettiAPosto() {
      UpdateLayout();
      Rect s = SystemParameters.WorkArea;
      Left = imp.lato == "destra" ? s.Left + s.Width - ActualWidth - 18 : s.Left + 18;
      Top = imp.alto == "sotto" ? s.Top + s.Height - ActualHeight - 18 : s.Top + 18;
    }

    // ================= il tasto del verso =================
    // Piccolo, tondo, di vetro: a colonna o in riga. Sta in coda alla griglia,
    // cosi' non copre nessuna tessera.
    Border TastoVerso() {
      Border b = new Border();
      b.Width = 26; b.Height = 26;
      b.Background = Pelle.Colore(Pelle.Vetro);
      b.BorderBrush = Pelle.Colore(Pelle.VetroSu);
      b.BorderThickness = new Thickness(1);
      b.CornerRadius = new CornerRadius(13);
      b.Cursor = System.Windows.Input.Cursors.Hand;
      b.ToolTip = imp.Orizzontale ? "Rimetti a colonna" : "Metti in riga";
      ContentControl ico = new ContentControl();
      ico.Content = Icone.Prendi(imp.Orizzontale ? "verticale" : "orizzontale", Pelle.Testo, 14);
      ico.HorizontalAlignment = HorizontalAlignment.Center;
      ico.VerticalAlignment = VerticalAlignment.Center;
      b.Child = ico;
      ScaleTransform s = new ScaleTransform(1, 1);
      b.RenderTransformOrigin = new Point(0.5, 0.5);
      b.RenderTransform = s;
      b.MouseEnter += delegate {
        b.Background = Pelle.Colore("#38FFFFFF");
        s.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1.1, new Duration(TimeSpan.FromMilliseconds(140))));
        s.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1.1, new Duration(TimeSpan.FromMilliseconds(140))));
      };
      b.MouseLeave += delegate {
        b.Background = Pelle.Colore(Pelle.Vetro);
        s.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(220))));
        s.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(220))));
      };
      Pelle.Premuta(b, s, 1.1, 0.9);
      b.MouseLeftButtonUp += delegate { CambiaVerso(imp.Orizzontale ? "verticale" : "orizzontale"); };
      tastoVerso = b;
      return b;
    }

    // Il cambio di verso e' UN movimento solo: ogni tessera scivola da dove era a dove
    // va, e la finestra si allarga o si stringe insieme a loro, con lo stesso passo.
    // L'angolo non cambia, quindi l'orario resta fermo e il resto si srotola.
    // Le tessere sono le stesse di prima: niente ridisegno, le icone non ripartono.
    public void CambiaVerso(string nuovo) {
      if (inMovimento || imp.verso == nuovo) return;
      inMovimento = true;

      // 1. dove sta ogni cosa adesso, rispetto all'angolo in alto a sinistra della finestra
      Dictionary<string, Point> prima = new Dictionary<string, Point>();
      foreach (KeyValuePair<string, ContentControl> c in caselle) prima[c.Key] = Dentro(c.Value);
      Point tastoPrima = tastoVerso != null ? Dentro(tastoVerso) : new Point(0, 0);
      double l0 = Left, t0 = Top, w0 = ActualWidth, h0 = ActualHeight;

      // 2. il verso nuovo, con le stesse tessere
      imp.verso = nuovo;
      imp.Salva();
      DisegnaGriglia(true);
      MettiAPosto();
      double l1 = Left, t1 = Top, w1 = ActualWidth, h1 = ActualHeight;

      // 3. la finestra torna com'era, e ogni tessera viene spostata indietro dove stava:
      //    da li' a dove va, mentre la finestra le segue
      SizeToContent = SizeToContent.Manual;
      Width = w0; Height = h0; Left = l0; Top = t0;

      Duration durata = new Duration(TimeSpan.FromMilliseconds(620));
      BackEase passo = new BackEase();
      passo.EasingMode = EasingMode.EaseOut;
      passo.Amplitude = 0.18;

      List<KeyValuePair<UIElement, Point>> mosse = new List<KeyValuePair<UIElement, Point>>();
      foreach (KeyValuePair<string, ContentControl> c in caselle) {
        Point p0;
        if (!prima.TryGetValue(c.Key, out p0)) continue;
        Point p1 = Dentro(c.Value);
        mosse.Add(new KeyValuePair<UIElement, Point>(c.Value, new Point(p0.X - p1.X, p0.Y - p1.Y)));
      }
      if (tastoVerso != null) {
        Point p1 = Dentro(tastoVerso);
        mosse.Add(new KeyValuePair<UIElement, Point>(tastoVerso, new Point(tastoPrima.X - p1.X, tastoPrima.Y - p1.Y)));
      }
      Dictionary<UIElement, Transform> proprie = new Dictionary<UIElement, Transform>();
      foreach (KeyValuePair<UIElement, Point> m in mosse) {
        TranslateTransform tr = new TranslateTransform(m.Value.X, m.Value.Y);
        // la sua trasformazione (il tondino ha la scala del tocco) resta sotto, lo scivolo va sopra
        Transform sua = m.Key.RenderTransform;
        proprie[m.Key] = sua;
        TransformGroup gruppo = new TransformGroup();
        if (sua != null && sua != Transform.Identity) gruppo.Children.Add(sua);
        gruppo.Children.Add(tr);
        m.Key.RenderTransform = gruppo;
        tr.BeginAnimation(TranslateTransform.XProperty, Scivola(m.Value.X, 0, durata, passo));
        tr.BeginAnimation(TranslateTransform.YProperty, Scivola(m.Value.Y, 0, durata, passo));
      }

      // La finestra la muovo io, un fotogramma alla volta, con lo stesso passo delle
      // tessere: animare Width e Height di una Window WPF non funziona quando cresce
      // (la cornice resta piccola e salta alla fine).
      IntPtr h = new WindowInteropHelper(this).Handle;
      double scala = 1;
      try {
        PresentationSource ps = PresentationSource.FromVisual(this);
        if (ps != null && ps.CompositionTarget != null) scala = ps.CompositionTarget.TransformToDevice.M11;
      } catch { }
      System.Diagnostics.Stopwatch orologio = System.Diagnostics.Stopwatch.StartNew();
      double ms = durata.TimeSpan.TotalMilliseconds;
      EventHandler quadro = null;
      quadro = delegate {
        double t = Math.Min(1, orologio.ElapsedMilliseconds / ms);
        double s = passo.Ease(t);
        int x = (int)Math.Round((l0 + (l1 - l0) * s) * scala);
        int y = (int)Math.Round((t0 + (t1 - t0) * s) * scala);
        int w = (int)Math.Round((w0 + (w1 - w0) * s) * scala);
        int a = (int)Math.Round((h0 + (h1 - h0) * s) * scala);
        SetWindowPos(h, IntPtr.Zero, x, y, Math.Max(1, w), Math.Max(1, a), NOZORDER | NOACTIVATE);
        if (t < 1) return;

        CompositionTarget.Rendering -= quadro;
        Width = double.NaN; Height = double.NaN;
        SizeToContent = SizeToContent.WidthAndHeight;
        foreach (KeyValuePair<UIElement, Point> m in mosse) m.Key.RenderTransform = proprie.ContainsKey(m.Key) ? proprie[m.Key] : null;
        MettiAPosto();
        Appoggia();
        inMovimento = false;
        if (pannello.Visibility == Visibility.Visible) DisegnaPannello();
      };
      CompositionTarget.Rendering += quadro;
    }

    static DoubleAnimation Scivola(double da, double a, Duration durata, IEasingFunction passo) {
      DoubleAnimation d = new DoubleAnimation(da, a, durata);
      d.EasingFunction = passo;
      d.FillBehavior = FillBehavior.Stop;
      return d;
    }

    // la posizione di un pezzo dentro la finestra, senza trasformazioni di mezzo
    Point Dentro(UIElement e) {
      try {
        Transform t = e.RenderTransform;
        e.RenderTransform = null;
        Point p = e.TranslatePoint(new Point(0, 0), this);
        e.RenderTransform = t;
        return p;
      } catch { return new Point(0, 0); }
    }

    // ================= la griglia =================

    // le righe della colonna: due "meta'" per riga, una "piena" da sola.
    // E' la forma base: la riga orizzontale e' la stessa cosa srotolata
    List<List<string>> Righe(List<string> accese) {
      List<List<string>> righe = new List<List<string>>();
      List<string> riga = new List<string>();
      foreach (string nome in accese) {
        bool meta = Nucleo.PerNome[nome].Larghezza == "meta";
        if (!meta && riga.Count > 0) { righe.Add(riga); riga = new List<string>(); }
        riga.Add(nome);
        if (!meta || riga.Count == 2) { righe.Add(riga); riga = new List<string>(); }
      }
      if (riga.Count > 0) righe.Add(riga);
      return righe;
    }

    void DisegnaGriglia() { DisegnaGriglia(false); }

    // riusa = true: le tessere restano le stesse, cambiano solo di posto
    void DisegnaGriglia(bool riusa) {
      griglia.Children.Clear();
      griglia.RowDefinitions.Clear();
      griglia.ColumnDefinitions.Clear();
      if (!riusa) { caselle.Clear(); visto.Clear(); }

      List<string> ordine = new List<string>(imp.ordine);
      foreach (Tessera t in Nucleo.Elenco) if (!ordine.Contains(t.Nome)) ordine.Add(t.Nome);

      List<string> accese = new List<string>();
      foreach (string nome in ordine) {
        if (!Nucleo.PerNome.ContainsKey(nome)) continue;
        if (!imp.Conto(nome).acceso && !Nucleo.PerNome[nome].Fissa) continue;
        accese.Add(nome);
      }

      List<List<string>> righe = Righe(accese);
      if (imp.Orizzontale) DisegnaRiga(righe, riusa);
      else DisegnaColonna(righe, riusa);
    }

    ContentControl Casella(string nome, bool riusa, int ritardo) {
      ContentControl c;
      if (riusa && caselle.TryGetValue(nome, out c)) { AggiustaAltezza(nome); return c; }
      c = new ContentControl();
      c.Tag = nome;
      caselle[nome] = c;
      DisegnaTessera(nome);
      Pelle.Entra(c, ritardo);
      return c;
    }

    // a colonna: le righe una sotto l'altra. Il tasto del verso sta sotto, a destra
    void DisegnaColonna(List<List<string>> righe, bool riusa) {
      altezzaRiga = 0;
      griglia.ColumnDefinitions.Add(Colonna());
      griglia.ColumnDefinitions.Add(Colonna());

      int ritardo = 0;
      for (int r = 0; r < righe.Count; r++) {
        AggiungiRiga(r);
        for (int k = 0; k < righe[r].Count; k++) {
          string nome = righe[r][k];
          bool meta = Nucleo.PerNome[nome].Larghezza == "meta";
          ContentControl casella = Casella(nome, riusa, ritardo);
          Grid.SetRow(casella, r);
          Grid.SetColumn(casella, meta ? k : 0);
          Grid.SetColumnSpan(casella, meta ? 1 : 2);
          griglia.Children.Add(casella);
          ritardo += 55;
        }
      }

      AggiungiRiga(righe.Count);
      Border tasto = TastoVerso();
      tasto.Margin = new Thickness(0, 2, Pelle.Buco + 2, Pelle.Buco);
      tasto.HorizontalAlignment = HorizontalAlignment.Right;
      Grid.SetRow(tasto, righe.Count);
      Grid.SetColumn(tasto, 1);
      griglia.Children.Add(tasto);
      if (!riusa) Pelle.Entra(tasto, ritardo);
    }

    // in riga: la colonna srotolata. La prima riga (l'orario) resta nell'angolo,
    // le altre le si mettono accanto verso il centro dello schermo, in ordine.
    // Tutte alte uguali; le "piene" larghe come due. Il tasto chiude la fila
    void DisegnaRiga(List<List<string>> righe, bool riusa) {
      altezzaRiga = 118;
      foreach (List<string> r in righe) foreach (string nome in r)
        altezzaRiga = Math.Max(altezzaRiga, Nucleo.PerNome[nome].Altezza);
      AggiungiRiga(0);

      bool destra = imp.lato == "destra";
      List<string> fila = new List<string>();
      if (destra) { for (int r = righe.Count - 1; r >= 0; r--) fila.AddRange(righe[r]); }
      else { foreach (List<string> r in righe) fila.AddRange(r); }

      ColumnDefinition auto = new ColumnDefinition();
      auto.Width = GridLength.Auto;
      int colonna = 0, ritardo = 0;
      int colonnaTasto = 0;
      if (destra) { griglia.ColumnDefinitions.Add(auto); colonna = 1; }

      foreach (string nome in fila) {
        int quante = Nucleo.PerNome[nome].Larghezza == "meta" ? 1 : 2;
        for (int k = 0; k < quante; k++) griglia.ColumnDefinitions.Add(Colonna());
        ContentControl casella = Casella(nome, riusa, ritardo);
        Grid.SetRow(casella, 0);
        Grid.SetColumn(casella, colonna);
        Grid.SetColumnSpan(casella, quante);
        griglia.Children.Add(casella);
        ritardo += 55;
        colonna += quante;
      }

      if (!destra) { griglia.ColumnDefinitions.Add(auto); colonnaTasto = colonna; }
      Border tasto = TastoVerso();
      tasto.Margin = destra ? new Thickness(Pelle.Buco, 0, 2, 0) : new Thickness(2, 0, Pelle.Buco, 0);
      tasto.VerticalAlignment = VerticalAlignment.Center;
      Grid.SetRow(tasto, 0);
      Grid.SetColumn(tasto, colonnaTasto);
      griglia.Children.Add(tasto);
      if (!riusa) Pelle.Entra(tasto, ritardo);
    }

    void AggiungiRiga(int riga) {
      while (griglia.RowDefinitions.Count <= riga) {
        RowDefinition r = new RowDefinition();
        r.Height = GridLength.Auto;
        griglia.RowDefinitions.Add(r);
      }
    }

    // in riga tutte le tessere sono alte uguali, a colonna ognuna la sua
    void AggiustaAltezza(string nome) {
      ContentControl casella;
      Tessera t;
      if (!caselle.TryGetValue(nome, out casella) || !Nucleo.PerNome.TryGetValue(nome, out t)) return;
      Border b = casella.Content as Border;
      if (b == null) return;
      double h = altezzaRiga > 0 ? altezzaRiga : t.Altezza;
      b.Height = h > 0 ? h : double.NaN;
      b.ClipToBounds = altezzaRiga > 0;
    }

    void DisegnaTessera(string nome) {
      Tessera t;
      if (!Nucleo.PerNome.TryGetValue(nome, out t)) return;
      ContentControl casella;
      if (!caselle.TryGetValue(nome, out casella)) return;

      string misura = imp.Conto(nome).misura ?? "media";
      if (Array.IndexOf(t.Misure, misura) < 0) misura = t.Misure[0];

      try {
        UIElement dentro = t.Vista(Nucleo.Prendi(nome), misura);
        Border riquadro = Pelle.Tessera(dentro, altezzaRiga > 0 ? altezzaRiga : t.Altezza, t.Rossa);
        if (altezzaRiga > 0) riquadro.ClipToBounds = true;   // in riga la lista lunga non deve sbordare
        Tessera quale = t;
        riquadro.Cursor = System.Windows.Input.Cursors.Hand;
        riquadro.MouseLeftButtonUp += delegate { quale.Tocco(); };
        casella.Content = riquadro;
      } catch { }
      visto[nome] = Nucleo.VersioneDi(nome);
    }

    // ogni riquadro si rifa' quando scade LA SUA roba, non tutti insieme:
    // rifare tutto farebbe ripartire tutte le animazioni ogni pochi secondi
    void Giro() {
      bool cambiaMinuto = minuto != DateTime.Now.Minute;
      foreach (KeyValuePair<string, ContentControl> c in new List<KeyValuePair<string, ContentControl>>(caselle)) {
        string nome = c.Key;
        if (!Nucleo.PerNome.ContainsKey(nome)) continue;
        long v = Nucleo.VersioneDi(nome);
        long avuta;
        visto.TryGetValue(nome, out avuta);
        if (v != avuta) DisegnaTessera(nome);
        else if (cambiaMinuto && Nucleo.PerNome[nome].Scadenza <= 3) DisegnaTessera(nome);
      }
      if (cambiaMinuto) {
        minuto = DateTime.Now.Minute;
      }
    }

    // ================= il pannello =================
    // Si cambia da qui e si vede subito: niente file da aprire, niente riavvio.

    Border CostruisciPannello() {
      StackPanel dentro = new StackPanel();

      StackPanel testa = new StackPanel();
      testa.Orientation = Orientation.Horizontal;
      testa.Margin = new Thickness(0, 0, 0, 14);
      ContentControl ico = new ContentControl();
      ico.Content = Icone.Prendi("impostazioni", Pelle.Testo, 18);
      ico.Margin = new Thickness(0, 0, 8, 0);
      ico.VerticalAlignment = VerticalAlignment.Center;
      testa.Children.Add(ico);
      TextBlock tit = Pelle.Testo_("IMPOSTAZIONI", "#FFFFFF", 13);
      tit.FontWeight = FontWeights.SemiBold;
      tit.VerticalAlignment = VerticalAlignment.Center;
      testa.Children.Add(tit);
      dentro.Children.Add(testa);

      comandi = new StackPanel();
      dentro.Children.Add(comandi);

      Border chiudi = new Border();
      chiudi.Background = Pelle.Colore("#26FFFFFF");
      chiudi.BorderBrush = Pelle.Colore("#33FFFFFF");
      chiudi.BorderThickness = new Thickness(1);
      chiudi.CornerRadius = new CornerRadius(12);
      chiudi.Padding = new Thickness(12, 7, 12, 8);
      chiudi.Margin = new Thickness(0, 10, 0, 0);
      chiudi.Cursor = System.Windows.Input.Cursors.Hand;
      TextBlock tc = Pelle.Testo_("CHIUDI", "#FFFFFF", 11);
      tc.FontWeight = FontWeights.SemiBold;
      tc.HorizontalAlignment = HorizontalAlignment.Center;
      chiudi.Child = tc;
      chiudi.MouseLeftButtonUp += delegate { pannello.Visibility = Visibility.Collapsed; };
      dentro.Children.Add(chiudi);

      Border b = new Border();
      b.Background = Pelle.Colore("#F20E0E14");
      b.BorderBrush = Pelle.Colore("#3DFFFFFF");
      b.BorderThickness = new Thickness(1);
      b.CornerRadius = new CornerRadius(Pelle.Raggio);
      b.Padding = new Thickness(18, 16, 18, 16);
      b.Margin = new Thickness(6);
      b.Width = Pelle.Colonna * 2 + Pelle.Buco * 2;   // largo come due tessere, anche quando il widget e in riga
      b.VerticalAlignment = VerticalAlignment.Top;
      b.Visibility = Visibility.Collapsed;
      b.Child = dentro;
      return b;
    }

    public void ApriPannello() {
      DisegnaPannello();
      pannello.HorizontalAlignment = imp.lato == "destra" ? HorizontalAlignment.Right : HorizontalAlignment.Left;
      pannello.Visibility = Visibility.Visible;
      pannello.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(200))));
    }

    void Applica() {
      imp.Salva();
      DisegnaGriglia();
      MettiAPosto();
      DisegnaPannello();
    }

    StackPanel RigaComando(string etichetta, params UIElement[] bottoni) {
      StackPanel sp = new StackPanel();
      sp.Margin = new Thickness(0, 0, 0, 10);
      TextBlock e = Pelle.Etichetta(etichetta);
      e.Margin = new Thickness(0, 0, 0, 6);
      e.HorizontalAlignment = HorizontalAlignment.Left;
      sp.Children.Add(e);
      WrapPanel fila = new WrapPanel();
      foreach (UIElement b in bottoni) fila.Children.Add(b);
      sp.Children.Add(fila);
      return sp;
    }

    void DisegnaPannello() {
      comandi.Children.Clear();

      comandi.Children.Add(RigaComando("COME STA",
        Pelle.Bottone("A COLONNA", !imp.Orizzontale, delegate { CambiaVerso("verticale"); }),
        Pelle.Bottone("IN RIGA",   imp.Orizzontale,  delegate { CambiaVerso("orizzontale"); })));

      comandi.Children.Add(RigaComando("DA CHE PARTE",
        Pelle.Bottone("SINISTRA", imp.lato == "sinistra", delegate { imp.lato = "sinistra"; Applica(); }),
        Pelle.Bottone("DESTRA",   imp.lato == "destra",   delegate { imp.lato = "destra";   Applica(); })));

      comandi.Children.Add(RigaComando("IN ALTO O IN BASSO",
        Pelle.Bottone("IN ALTO",  imp.alto == "sopra", delegate { imp.alto = "sopra"; Applica(); }),
        Pelle.Bottone("IN BASSO", imp.alto == "sotto", delegate { imp.alto = "sotto"; Applica(); })));

      comandi.Children.Add(RigaComando("QUANDO ACCENDI IL PC",
        Pelle.Bottone("PARTE DA SOLO", imp.avvioConWindows, delegate {
          imp.avvioConWindows = !imp.avvioConWindows;
          Avvio.Metti(imp.avvioConWindows);
          Applica();
        })));

      List<string> ordine = new List<string>(imp.ordine);
      foreach (Tessera t in Nucleo.Elenco) if (!ordine.Contains(t.Nome)) ordine.Add(t.Nome);

      List<UIElement> quali = new List<UIElement>();
      foreach (string nome in ordine) {
        if (!Nucleo.PerNome.ContainsKey(nome)) continue;
        Tessera t = Nucleo.PerNome[nome];
        if (t.Fissa) continue;
        string chiave = nome;
        quali.Add(Pelle.Bottone(t.Titolo, imp.Conto(nome).acceso, delegate {
          imp.Conto(chiave).acceso = !imp.Conto(chiave).acceso; Applica();
        }));
      }
      comandi.Children.Add(RigaComando("COSA SI VEDE", quali.ToArray()));

      // la misura: piu' grande non vuol dire caratteri piu' grossi, vuol dire piu' roba
      foreach (string nome in ordine) {
        if (!Nucleo.PerNome.ContainsKey(nome)) continue;
        Tessera t = Nucleo.PerNome[nome];
        if (t.Misure.Length < 3 || !imp.Conto(nome).acceso) continue;
        string chiave = nome;
        List<UIElement> misure = new List<UIElement>();
        foreach (string m in t.Misure) {
          string misura = m;
          misure.Add(Pelle.Bottone(misura.ToUpper(), imp.Conto(nome).misura == misura, delegate {
            imp.Conto(chiave).misura = misura; Applica();
          }));
        }
        comandi.Children.Add(RigaComando("QUANTO MOSTRA   " + t.Titolo, misure.ToArray()));
      }

      TextBlock nota = Pelle.Testo_("Si salva da solo.", Pelle.TenueC, 10);
      comandi.Children.Add(nota);
    }
  }
}
