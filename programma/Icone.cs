// Le icone del widget.
//
// Le forme vengono da Lucide (github.com/lucide-icons/lucide, licenza ISC, libera
// anche per uso commerciale): 1798 icone sulla stessa griglia 24x24, tratto 2,
// punte tonde. Coerenti fra loro, e se domani ne serve un'altra e' gia' in stile.
//
// Le animazioni le calcola WPF: nessuna libreria, nessun file, peso zero.
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace WhyWidget {
  public static class Icone {
    static readonly Dictionary<string, string> forme = new Dictionary<string, string>() {
      { "riprendi", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Path x:Name=""giro"" Data=""M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8""
          Stroke=""COLORE"" StrokeThickness=""2"" StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round""
          StrokeDashArray=""14 14"" StrokeDashOffset=""14""/>
    <Path Data=""M3 3v5h5"" Stroke=""COLORE"" StrokeThickness=""2""
          StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" StrokeLineJoin=""Round""/>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"" Duration=""0:0:3.2"">
        <DoubleAnimation Storyboard.TargetName=""giro"" Storyboard.TargetProperty=""StrokeDashOffset""
                         From=""14"" To=""0"" Duration=""0:0:1.4""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
      { "sessioni", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Path x:Name=""s1"" Data=""M12.83 2.18a2 2 0 0 0-1.66 0L2.6 6.08a1 1 0 0 0 0 1.83l8.58 3.91a2 2 0 0 0 1.66 0l8.58-3.9a1 1 0 0 0 0-1.83z""
          Stroke=""COLORE"" StrokeThickness=""2"" StrokeLineJoin=""Round"">
      <Path.RenderTransform><TranslateTransform x:Name=""t1"" Y=""0""/></Path.RenderTransform>
    </Path>
    <Path x:Name=""s2"" Data=""M2 12.87a1 1 0 0 0 .59.91l8.58 3.91a2 2 0 0 0 1.66 0l8.58-3.9a1 1 0 0 0 .59-.92""
          Stroke=""COLORE"" StrokeThickness=""2"" StrokeLineJoin=""Round"" StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" Opacity=""0.6"">
      <Path.RenderTransform><TranslateTransform x:Name=""t2"" Y=""0""/></Path.RenderTransform>
    </Path>
    <Path x:Name=""s3"" Data=""M2 17.87a1 1 0 0 0 .59.91l8.58 3.91a2 2 0 0 0 1.66 0l8.58-3.9a1 1 0 0 0 .59-.92""
          Stroke=""COLORE"" StrokeThickness=""2"" StrokeLineJoin=""Round"" StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" Opacity=""0.35"">
      <Path.RenderTransform><TranslateTransform x:Name=""t3"" Y=""0""/></Path.RenderTransform>
    </Path>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"" Duration=""0:0:3.4"">
        <DoubleAnimation Storyboard.TargetName=""t1"" Storyboard.TargetProperty=""Y"" From=""0"" To=""-1.6""
                         Duration=""0:0:0.6"" AutoReverse=""True"" BeginTime=""0:0:0""/>
        <DoubleAnimation Storyboard.TargetName=""t2"" Storyboard.TargetProperty=""Y"" From=""0"" To=""1.2""
                         Duration=""0:0:0.6"" AutoReverse=""True"" BeginTime=""0:0:0.15""/>
        <DoubleAnimation Storyboard.TargetName=""t3"" Storyboard.TargetProperty=""Y"" From=""0"" To=""2.2""
                         Duration=""0:0:0.6"" AutoReverse=""True"" BeginTime=""0:0:0.3""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
      { "agenti", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Path Data=""M12 8V4H8"" Stroke=""COLORE"" StrokeThickness=""2"" StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" StrokeLineJoin=""Round""/>
    <Rectangle Canvas.Left=""4"" Canvas.Top=""8"" Width=""16"" Height=""12"" RadiusX=""2"" RadiusY=""2""
               Stroke=""COLORE"" StrokeThickness=""2""/>
    <Path Data=""M2 14h2 M20 14h2"" Stroke=""COLORE"" StrokeThickness=""2"" StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round""/>
    <Path x:Name=""occhi"" Data=""M9 13v2 M15 13v2"" Stroke=""ACCENTO"" StrokeThickness=""2""
          StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round""/>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"" Duration=""0:0:3"">
        <DoubleAnimation Storyboard.TargetName=""occhi"" Storyboard.TargetProperty=""Opacity""
                         From=""1"" To=""0.15"" Duration=""0:0:0.5"" AutoReverse=""True"" BeginTime=""0:0:0.4""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
      { "dispositivi", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Ellipse Canvas.Left=""10"" Canvas.Top=""7"" Width=""4"" Height=""4"" Fill=""ACCENTO""/>
    <Path Data=""M9.5 18h5 M12 11v11 M6 20l2-4 M18 20l-2-4"" Stroke=""COLORE"" StrokeThickness=""2""
          StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round""/>
    <Path x:Name=""o1"" Data=""M7.8 4.7a6.14 6.14 0 0 0-.8 7.5"" Stroke=""COLORE"" StrokeThickness=""2""
          StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" Opacity=""0.3""/>
    <Path x:Name=""o2"" Data=""M16.2 4.8c2 2 2.26 5.11.8 7.47"" Stroke=""COLORE"" StrokeThickness=""2""
          StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" Opacity=""0.3""/>
    <Path x:Name=""o3"" Data=""M4.9 16.1C1 12.2 1 5.8 4.9 1.9"" Stroke=""COLORE"" StrokeThickness=""2""
          StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" Opacity=""0.3""/>
    <Path x:Name=""o4"" Data=""M19.1 1.9a9.96 9.96 0 0 1 0 14.1"" Stroke=""COLORE"" StrokeThickness=""2""
          StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" Opacity=""0.3""/>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"" Duration=""0:0:2.6"">
        <DoubleAnimation Storyboard.TargetName=""o1"" Storyboard.TargetProperty=""Opacity"" From=""0.3"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0""/>
        <DoubleAnimation Storyboard.TargetName=""o2"" Storyboard.TargetProperty=""Opacity"" From=""0.3"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0""/>
        <DoubleAnimation Storyboard.TargetName=""o3"" Storyboard.TargetProperty=""Opacity"" From=""0.3"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0.25""/>
        <DoubleAnimation Storyboard.TargetName=""o4"" Storyboard.TargetProperty=""Opacity"" From=""0.3"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0.25""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
      { "iphone", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Rectangle Canvas.Left=""5"" Canvas.Top=""2"" Width=""14"" Height=""20"" RadiusX=""2"" RadiusY=""2""
               Stroke=""COLORE"" StrokeThickness=""2""/>
    <Path Data=""M12 18h.01"" Stroke=""COLORE"" StrokeThickness=""2"" StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round""/>
    <Path x:Name=""su"" Data=""M12 14V7 M9 10l3-3 3 3"" Stroke=""ACCENTO"" StrokeThickness=""2""
          StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" StrokeLineJoin=""Round"" Opacity=""0"">
      <Path.RenderTransform><TranslateTransform x:Name=""tsu"" Y=""4""/></Path.RenderTransform>
    </Path>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"" Duration=""0:0:2.8"">
        <DoubleAnimation Storyboard.TargetName=""tsu"" Storyboard.TargetProperty=""Y"" From=""4"" To=""-1"" Duration=""0:0:1.5""/>
        <DoubleAnimation Storyboard.TargetName=""su"" Storyboard.TargetProperty=""Opacity""
                         From=""0"" To=""1"" Duration=""0:0:0.55"" AutoReverse=""True"" BeginTime=""0:0:0.2""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
      { "lavori", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Path Data=""M21 10.656V19a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h12.344""
          Stroke=""COLORE"" StrokeThickness=""2"" StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" StrokeLineJoin=""Round""/>
    <Path x:Name=""spunta"" Data=""m9 11 3 3L22 4"" Stroke=""ACCENTO"" StrokeThickness=""2""
          StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"" StrokeLineJoin=""Round""
          StrokeDashArray=""11 11"" StrokeDashOffset=""11""/>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"" Duration=""0:0:3.6"">
        <DoubleAnimation Storyboard.TargetName=""spunta"" Storyboard.TargetProperty=""StrokeDashOffset""
                         From=""11"" To=""0"" Duration=""0:0:0.9"" BeginTime=""0:0:0.3""/>
        <DoubleAnimation Storyboard.TargetName=""spunta"" Storyboard.TargetProperty=""StrokeDashOffset""
                         From=""0"" To=""-11"" Duration=""0:0:0.7"" BeginTime=""0:0:2.5""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
      { "impostazioni", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Path Data=""M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z""
          Stroke=""COLORE"" StrokeThickness=""2"" StrokeLineJoin=""Round"">
      <Path.RenderTransform><RotateTransform x:Name=""ruota"" CenterX=""12"" CenterY=""12""/></Path.RenderTransform>
    </Path>
    <Ellipse Canvas.Left=""9"" Canvas.Top=""9"" Width=""6"" Height=""6"" Stroke=""COLORE"" StrokeThickness=""2""/>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"">
        <DoubleAnimation Storyboard.TargetName=""ruota"" Storyboard.TargetProperty=""Angle""
                         From=""0"" To=""360"" Duration=""0:0:24""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
      { "ora", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Ellipse Canvas.Left=""2"" Canvas.Top=""2"" Width=""20"" Height=""20"" Stroke=""COLORE"" StrokeThickness=""2""/>
    <Path Data=""M12 7v5"" Stroke=""COLORE"" StrokeThickness=""2"" StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round""/>
    <Path Data=""M12 12h3.5"" Stroke=""ACCENTO"" StrokeThickness=""2"" StrokeStartLineCap=""Round"" StrokeEndLineCap=""Round"">
      <Path.RenderTransform><RotateTransform x:Name=""lancetta"" CenterX=""12"" CenterY=""12""/></Path.RenderTransform>
    </Path>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"">
        <DoubleAnimation Storyboard.TargetName=""lancetta"" Storyboard.TargetProperty=""Angle""
                         From=""0"" To=""360"" Duration=""0:0:12""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
      // il tasto del verso: tre tessere in fila che si accendono una dopo l'altra.
      // Mostra come DIVENTA il widget se lo premi, non com'e' adesso
      { "orizzontale", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Rectangle x:Name=""r1"" Canvas.Left=""2""  Canvas.Top=""8"" Width=""5.5"" Height=""8"" RadiusX=""1.8"" RadiusY=""1.8"" Fill=""COLORE"" Opacity=""0.45""/>
    <Rectangle x:Name=""r2"" Canvas.Left=""9.25"" Canvas.Top=""8"" Width=""5.5"" Height=""8"" RadiusX=""1.8"" RadiusY=""1.8"" Fill=""COLORE"" Opacity=""0.45""/>
    <Rectangle x:Name=""r3"" Canvas.Left=""16.5"" Canvas.Top=""8"" Width=""5.5"" Height=""8"" RadiusX=""1.8"" RadiusY=""1.8"" Fill=""COLORE"" Opacity=""0.45""/>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"" Duration=""0:0:2.6"">
        <DoubleAnimation Storyboard.TargetName=""r1"" Storyboard.TargetProperty=""Opacity"" From=""0.45"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0""/>
        <DoubleAnimation Storyboard.TargetName=""r2"" Storyboard.TargetProperty=""Opacity"" From=""0.45"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0.22""/>
        <DoubleAnimation Storyboard.TargetName=""r3"" Storyboard.TargetProperty=""Opacity"" From=""0.45"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0.44""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
      { "verticale", @"<Canvas xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""MISURA"" Height=""MISURA"">
  <Canvas Width=""24"" Height=""24"">
    <Canvas.RenderTransform><ScaleTransform ScaleX=""SCALA"" ScaleY=""SCALA""/></Canvas.RenderTransform>
    <Rectangle x:Name=""r1"" Canvas.Left=""8"" Canvas.Top=""2""    Width=""8"" Height=""5.5"" RadiusX=""1.8"" RadiusY=""1.8"" Fill=""COLORE"" Opacity=""0.45""/>
    <Rectangle x:Name=""r2"" Canvas.Left=""8"" Canvas.Top=""9.25"" Width=""8"" Height=""5.5"" RadiusX=""1.8"" RadiusY=""1.8"" Fill=""COLORE"" Opacity=""0.45""/>
    <Rectangle x:Name=""r3"" Canvas.Left=""8"" Canvas.Top=""16.5"" Width=""8"" Height=""5.5"" RadiusX=""1.8"" RadiusY=""1.8"" Fill=""COLORE"" Opacity=""0.45""/>
  </Canvas>
  <Canvas.Triggers>
    <EventTrigger RoutedEvent=""Canvas.Loaded"">
      <BeginStoryboard><Storyboard RepeatBehavior=""Forever"" Duration=""0:0:2.6"">
        <DoubleAnimation Storyboard.TargetName=""r1"" Storyboard.TargetProperty=""Opacity"" From=""0.45"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0""/>
        <DoubleAnimation Storyboard.TargetName=""r2"" Storyboard.TargetProperty=""Opacity"" From=""0.45"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0.22""/>
        <DoubleAnimation Storyboard.TargetName=""r3"" Storyboard.TargetProperty=""Opacity"" From=""0.45"" To=""1"" Duration=""0:0:0.4"" AutoReverse=""True"" BeginTime=""0:0:0.44""/>
      </Storyboard></BeginStoryboard>
    </EventTrigger>
  </Canvas.Triggers>
</Canvas>" },
    };

    // Tira fuori l'icona gia' viva, alla misura che serve.
    // Il disegno e' sempre lo stesso: cambia come e' vestito e quanto e' grande.
    public static UIElement Prendi(string nome, string colore, double misura, string accento) {
      if (nome == null || !forme.ContainsKey(nome)) return null;
      double scala = Math.Round(misura / 24.0, 4);
      string t = forme[nome]
        .Replace("MISURA", misura.ToString(System.Globalization.CultureInfo.InvariantCulture))
        .Replace("SCALA", scala.ToString(System.Globalization.CultureInfo.InvariantCulture))
        .Replace("COLORE", colore)
        .Replace("ACCENTO", accento);
      try { return (UIElement)XamlReader.Parse(t); } catch { return null; }
    }

    public static UIElement Prendi(string nome, string colore, double misura) {
      return Prendi(nome, colore, misura, Pelle.Rosso);
    }
    public static UIElement Prendi(string nome, string colore) { return Prendi(nome, colore, 16); }
    public static UIElement Prendi(string nome) { return Prendi(nome, Pelle.Testo, 16); }
  }
}