// L'avvio del programma. Non e' piu' uno script che apre una finestra nera:
// e' un eseguibile solo, con la sua icona nella barra vicino all'orologio,
// che si puo' far partire da solo quando accende il PC.
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace WhyWidget {

  public static class Avvio {
    const string CHIAVE = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    const string NOME = "WhyWidget";

    public static void Metti(bool si) {
      try {
        using (var k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(CHIAVE, true)) {
          if (k == null) return;
          if (si) k.SetValue(NOME, "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
          else k.DeleteValue(NOME, false);
        }
      } catch { }
    }

    public static bool Messo() {
      try {
        using (var k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(CHIAVE)) {
          return k != null && k.GetValue(NOME) != null;
        }
      } catch { return false; }
    }
  }

  public static class Programma {
    static Mutex solo;
    static NotifyIcon barra;
    static Finestra finestra;

    [STAThread]
    public static void Main(string[] argomenti) {
      // -foto <file>: fa il ritratto della finestra e chiude. Serve a me per
      // controllare com'e' venuto senza disturbare quello che sta facendo lui.
      string foto = null;
      for (int i = 0; i < argomenti.Length - 1; i++)
        if (argomenti[i] == "-foto") foto = argomenti[i + 1];

      // uno solo alla volta: due widget aperti litigherebbero per la porta del ponte
      bool nuovo;
      solo = new Mutex(true, "WhyWidget-uno-solo", out nuovo);
      if (!nuovo) return;

      Impostazioni imp = Impostazioni.Leggi();
      imp.avvioConWindows = Avvio.Messo();

      // le tessere, nell'ordine in cui arrivano nella colonna
      Nucleo.Registra(new TesseraOra());
      Nucleo.Registra(new TesseraRiprendi());
      Nucleo.Registra(new TesseraSessioni());
      // l'ordine e' quello del widget vero: agenti e lavori affiancati,
      // poi le due aggiunte nuove sulla riga sotto
      Nucleo.Registra(new TesseraAgenti());
      Nucleo.Registra(new TesseraLavori());
      Nucleo.Registra(new TesseraCollegati());
      Nucleo.Registra(new TesseraIphone());

      Nucleo.PrimaInfornata();
      Nucleo.AccendiCucina();
      Ponte.Accendi();

      Application_ app = new Application_();
      finestra = new Finestra(imp);

      if (foto != null) {
        finestra.Ritratto(foto);
        app.Run(finestra);
        return;
      }

      MettiNellaBarra(imp);

      app.Run(finestra);
      if (barra != null) { barra.Visible = false; barra.Dispose(); }
    }

    // l'icona vicino all'orologio, come Focusrite: da li' si comanda tutto
    static void MettiNellaBarra(Impostazioni imp) {
      barra = new NotifyIcon();
      barra.Text = "WhyWidget";
      barra.Icon = IconaBarra();
      barra.Visible = true;

      ContextMenuStrip menu = new ContextMenuStrip();
      menu.Items.Add("Impostazioni", null, delegate { finestra.Dispatcher.Invoke(new Action(finestra.ApriPannello)); });
      menu.Items.Add("Rimetti a posto", null, delegate { finestra.Dispatcher.Invoke(new Action(finestra.MettiAPosto)); });
      menu.Items.Add(new ToolStripSeparator());
      menu.Items.Add("Chiudi", null, delegate {
        barra.Visible = false;
        finestra.Dispatcher.Invoke(new Action(delegate { System.Windows.Application.Current.Shutdown(); }));
      });
      barra.ContextMenuStrip = menu;
      barra.DoubleClick += delegate { finestra.Dispatcher.Invoke(new Action(finestra.ApriPannello)); };
    }

    // l'icona: la banda bianca tagliata su fondo nero, la stessa firma del widget.
    // E' disegnata qui, cosi' il programma e' un file solo e non si porta dietro niente.
    static Icon IconaBarra() {
      try {
        string suo = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "WhyWidget.ico");
        if (File.Exists(suo)) return new Icon(suo);
      } catch { }
      Bitmap b = new Bitmap(32, 32);
      using (Graphics g = Graphics.FromImage(b)) {
        g.Clear(ColorTranslator.FromHtml("#0A0A0B"));
        g.FillRectangle(Brushes.White, 3, 6, 20, 4);
        g.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#E11D2E")), 3, 15, 11, 4);
        g.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#6B6B70")), 3, 24, 16, 3);
      }
      return Icon.FromHandle(b.GetHicon());
    }
  }

  // l'applicazione WPF: chiude quando si chiude la finestra
  public class Application_ : System.Windows.Application {
    public Application_() {
      ShutdownMode = ShutdownMode.OnMainWindowClose;
    }
  }
}
