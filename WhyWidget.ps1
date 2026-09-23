# WhyWidget: il widget da desktop di WhyEd.
# Griglia di riquadri come il Centro di Controllo dell'iPhone: un tocco, niente menu.
# Vetro liquido: il desktop si vede attraverso. Appoggiato sul desktop, non copre le
# finestre, non e' in Alt+Tab, non e' nella barra.

param(
  [string]$Foto = '',    # se dato: fa il ritratto della finestra e chiude. Serve a me per controllare.
  [switch]$ConPannello   # nel ritratto, mostra il pannello impostazioni aperto
)

Add-Type -AssemblyName PresentationFramework, PresentationCore, WindowsBase, System.Windows.Forms

$RADICE = $PSScriptRoot
. (Join-Path $RADICE 'parti\sessioni.ps1')     # aprire una sessione dove e' nata
. (Join-Path $RADICE 'parti\dispositivi.ps1')  # cosa e' collegato e quanta batteria ha
. (Join-Path $RADICE 'parti\icone.ps1')        # le icone che si muovono
. (Join-Path $RADICE 'parti\impostazioni.ps1') # quello che decide lui dal pannello

$IMP = Leggi-Impostazioni

$BASE = 'http://localhost:4180'
$LATO = 'destra'      # 'destra' o 'sinistra'

[xml]$xaml = @'
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        WindowStyle="None" AllowsTransparency="False" Background="#01000000"
        ShowInTaskbar="False" Topmost="False" ResizeMode="NoResize"
        SizeToContent="WidthAndHeight" Opacity="0">
  <Window.Resources>
    <SolidColorBrush x:Key="Testo"  Color="#F2FFFFFF"/>
    <SolidColorBrush x:Key="Mezzo"  Color="#9EFFFFFF"/>
    <SolidColorBrush x:Key="Tenue"  Color="#5CFFFFFF"/>
    <SolidColorBrush x:Key="Rosso"  Color="#FF5A6B"/>

    <!-- il riquadro: e' sempre lo stesso, e' il mattone di tutta la griglia -->
    <Style x:Key="Tessera" TargetType="Border">
      <Setter Property="Background" Value="#1CFFFFFF"/>
      <Setter Property="BorderBrush" Value="#2EFFFFFF"/>
      <Setter Property="BorderThickness" Value="1"/>
      <Setter Property="CornerRadius" Value="22"/>
      <Setter Property="Padding" Value="16"/>
      <Setter Property="Margin" Value="5"/>
      <Setter Property="RenderTransformOrigin" Value="0.5,0.5"/>
      <Setter Property="RenderTransform"><Setter.Value><ScaleTransform/></Setter.Value></Setter>
      <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
          <Setter Property="Background" Value="#2EFFFFFF"/>
          <Trigger.EnterActions>
            <BeginStoryboard><Storyboard>
              <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleX" To="0.975" Duration="0:0:0.14"/>
              <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleY" To="0.975" Duration="0:0:0.14"/>
            </Storyboard></BeginStoryboard>
          </Trigger.EnterActions>
          <Trigger.ExitActions>
            <BeginStoryboard><Storyboard>
              <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleX" To="1" Duration="0:0:0.22"/>
              <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleY" To="1" Duration="0:0:0.22"/>
            </Storyboard></BeginStoryboard>
          </Trigger.ExitActions>
        </Trigger>
      </Style.Triggers>
    </Style>

    <Style x:Key="Et" TargetType="TextBlock">
      <Setter Property="Foreground" Value="{StaticResource Tenue}"/>
      <Setter Property="FontFamily" Value="Segoe UI"/>
      <Setter Property="FontSize" Value="10.5"/>
      <Setter Property="FontWeight" Value="SemiBold"/>
    </Style>
  </Window.Resources>

  <Grid>
   <Grid x:Name="Radice" Margin="6">
    <Grid.RenderTransform><ScaleTransform x:Name="Entrata" ScaleX="0.94" ScaleY="0.94"/></Grid.RenderTransform>
    <Grid.RowDefinitions>
      <RowDefinition Height="Auto"/>
      <RowDefinition Height="Auto"/>
      <RowDefinition Height="Auto"/>
      <RowDefinition Height="Auto"/>
      <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="164"/>
      <ColumnDefinition Width="164"/>
    </Grid.ColumnDefinitions>

    <!-- ORA -->
    <Border Grid.Row="0" Grid.Column="0" Style="{StaticResource Tessera}" Height="118">
      <StackPanel VerticalAlignment="Center">
        <TextBlock x:Name="Ora" Foreground="White" FontSize="40" FontWeight="Thin"
                   FontFamily="Segoe UI Variable Display, Segoe UI Light" Margin="0,0,0,-2"/>
        <TextBlock x:Name="Data" Style="{StaticResource Et}" TextWrapping="Wrap"/>
      </StackPanel>
    </Border>

    <!-- RIPRENDI -->
    <Border x:Name="CardRiprendi" Grid.Row="0" Grid.Column="1" Style="{StaticResource Tessera}"
            Background="#33FF5A6B" BorderBrush="#4DFF8A96" Height="118" Cursor="Hand">
      <StackPanel VerticalAlignment="Center">
        <Path Fill="{StaticResource Testo}" Width="24" Height="24" Stretch="Uniform" HorizontalAlignment="Left"
              Data="M12 5V1L7 6l5 5V7c3.3 0 6 2.7 6 6s-2.7 6-6 6-6-2.7-6-6H4c0 4.4 3.6 8 8 8s8-3.6 8-8-3.6-8-8-8z"/>
        <TextBlock Text="RIPRENDI" Foreground="White" FontFamily="Segoe UI" FontSize="14"
                   FontWeight="SemiBold" Margin="0,10,0,0"/>
        <TextBlock x:Name="UltimaQ" Style="{StaticResource Et}" Foreground="#B3FFFFFF" Margin="0,1,0,0"/>
      </StackPanel>
    </Border>

    <!-- SESSIONI: largo due colonne -->
    <Border Grid.Row="1" Grid.ColumnSpan="2" Style="{StaticResource Tessera}" Padding="16,14,16,10">
      <StackPanel>
        <StackPanel Orientation="Horizontal" Margin="0,0,0,10">
          <ContentControl x:Name="IcoSessioni" Margin="0,0,7,0" VerticalAlignment="Center"/>
          <TextBlock Text="LE ULTIME SESSIONI" Style="{StaticResource Et}" VerticalAlignment="Center"/>
        </StackPanel>
        <StackPanel x:Name="Sessioni"/>
      </StackPanel>
    </Border>

    <!-- AGENTI -->
    <Border Grid.Row="2" Grid.Column="0" Style="{StaticResource Tessera}" Height="132" Padding="15,14,15,12">
      <StackPanel>
        <StackPanel Orientation="Horizontal" Margin="0,0,0,9">
          <ContentControl x:Name="IcoAgenti" Margin="0,0,7,0" VerticalAlignment="Center"/>
          <TextBlock Text="AGENTI" Style="{StaticResource Et}" VerticalAlignment="Center"/>
        </StackPanel>
        <WrapPanel x:Name="Agenti"/>
      </StackPanel>
    </Border>

    <!-- LAVORI -->
    <Border x:Name="CardLavori" Grid.Row="2" Grid.Column="1" Style="{StaticResource Tessera}"
            Height="132" Cursor="Hand">
      <StackPanel>
        <StackPanel Orientation="Horizontal">
          <ContentControl x:Name="IcoLavori" Margin="0,0,7,0" VerticalAlignment="Center"/>
          <TextBlock Text="DA FARE" Style="{StaticResource Et}" VerticalAlignment="Center"/>
        </StackPanel>
        <TextBlock x:Name="ContaLavori" Foreground="White" FontSize="38" FontWeight="Thin"
                   FontFamily="Segoe UI Variable Display, Segoe UI Light" Margin="0,2,0,2"/>
        <StackPanel x:Name="Lavori"/>
      </StackPanel>
    </Border>

    <!-- COLLEGATI: cosa e' attaccato al PC e quanta batteria gli resta -->
    <Border Grid.Row="3" Grid.Column="0" Style="{StaticResource Tessera}" Height="118" Padding="15,14,15,12">
      <StackPanel>
        <StackPanel Orientation="Horizontal" Margin="0,0,0,9">
          <ContentControl x:Name="IcoCollegati" Margin="0,0,7,0" VerticalAlignment="Center"/>
          <TextBlock Text="COLLEGATI" Style="{StaticResource Et}" VerticalAlignment="Center"/>
        </StackPanel>
        <StackPanel x:Name="Collegati"/>
      </StackPanel>
    </Border>

    <!-- IPHONE: quello che copi sul telefono si incolla qui -->
    <Border x:Name="CardPonte" Grid.Row="3" Grid.Column="1" Style="{StaticResource Tessera}"
            Height="118" Padding="15,14,15,12" Cursor="Hand">
      <StackPanel>
        <StackPanel Orientation="Horizontal" Margin="0,0,0,9">
          <ContentControl x:Name="IcoPonte" Margin="0,0,7,0" VerticalAlignment="Center"/>
          <TextBlock Text="IPHONE" Style="{StaticResource Et}" VerticalAlignment="Center"/>
          <Ellipse x:Name="PonteVivo" Width="5" Height="5" Fill="#5CFFFFFF" Margin="7,0,0,0" VerticalAlignment="Center"/>
        </StackPanel>
        <TextBlock x:Name="PonteT" Foreground="#B3FFFFFF" FontFamily="Segoe UI" FontSize="10.5"
                   TextWrapping="Wrap" LineHeight="14"/>
        <TextBlock x:Name="PonteQ" Style="{StaticResource Et}" FontSize="9.5" Margin="0,4,0,0"/>
      </StackPanel>
    </Border>

    <!-- il tasto delle impostazioni: una striscia bassa, non una tessera -->
    <Border x:Name="CardImpostazioni" Grid.Row="4" Grid.ColumnSpan="2" Style="{StaticResource Tessera}"
            Padding="15,9,15,10" Cursor="Hand">
      <Grid>
        <StackPanel Orientation="Horizontal">
          <ContentControl x:Name="IcoImpostazioni" Margin="0,0,7,0" VerticalAlignment="Center"/>
          <TextBlock Text="IMPOSTAZIONI" Style="{StaticResource Et}" FontSize="9.5" VerticalAlignment="Center"/>
        </StackPanel>
        <TextBlock x:Name="PiedeQ" Style="{StaticResource Et}" FontSize="9.5"
                   HorizontalAlignment="Right" VerticalAlignment="Center"/>
      </Grid>
    </Border>

   </Grid>

   <!-- le impostazioni: si aprono sopra, stessa pelle di vetro -->
   <Border x:Name="Pannello" Style="{StaticResource Tessera}" Margin="6" Padding="18,16,18,16"
           Background="#F2101015" BorderBrush="#3DFFFFFF" Visibility="Collapsed"
           VerticalAlignment="Stretch">
     <StackPanel VerticalAlignment="Top">
       <StackPanel Orientation="Horizontal" Margin="0,0,0,14">
         <ContentControl x:Name="IcoPannello" Margin="0,0,8,0" VerticalAlignment="Center"/>
         <TextBlock Text="IMPOSTAZIONI" Foreground="White" FontFamily="Segoe UI" FontSize="13"
                    FontWeight="SemiBold" VerticalAlignment="Center"/>
       </StackPanel>
       <StackPanel x:Name="Comandi"/>
       <Border x:Name="Chiudi" Background="#26FFFFFF" BorderBrush="#33FFFFFF" BorderThickness="1"
               CornerRadius="12" Padding="12,7,12,8" Margin="0,10,0,0" Cursor="Hand">
         <TextBlock Text="CHIUDI" Foreground="White" FontFamily="Segoe UI" FontSize="11"
                    FontWeight="SemiBold" HorizontalAlignment="Center"/>
       </Border>
     </StackPanel>
   </Border>
  </Grid>
</Window>
'@

$w = [Windows.Markup.XamlReader]::Load((New-Object System.Xml.XmlNodeReader $xaml))
foreach ($n in 'Ora','Data','CardRiprendi','UltimaQ','Sessioni','Agenti','CardLavori','ContaLavori','Lavori','Entrata',
               'Collegati','IcoCollegati','CardPonte','IcoPonte','PonteVivo','PonteT','PonteQ',
               'CardImpostazioni','IcoImpostazioni','PiedeQ','Pannello','IcoPannello','Comandi','Chiudi',
               'IcoSessioni','IcoAgenti','IcoLavori') {
  Set-Variable -Name "x$n" -Value $w.FindName($n)
}

# --- vetro liquido, angoli tondi, appoggio sul desktop ---
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Vetro {
  [StructLayout(LayoutKind.Sequential)]
  struct ACCENTPOLICY { public int Stato; public int Flag; public int Colore; public int Anima; }
  [StructLayout(LayoutKind.Sequential)]
  struct WINCOMPATTRDATA { public int Attributo; public IntPtr Dati; public int Misura; }
  [DllImport("user32.dll")] static extern int SetWindowCompositionAttribute(IntPtr h, ref WINCOMPATTRDATA d);
  [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int a, ref int v, int s);
  [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int cx, int cy, uint f);
  [DllImport("user32.dll")] public static extern int GetWindowLong(IntPtr h, int i);
  [DllImport("user32.dll")] public static extern int SetWindowLong(IntPtr h, int i, int v);
  const int ACRILICO = 4, SFOCATO = 3, ATTR_ACCENT = 19, ANGOLI = 33, TONDO = 2;
  static IntPtr BOTTOM = new IntPtr(1);
  const uint NOSIZE = 0x0001, NOMOVE = 0x0002, NOACTIVATE = 0x0010, SHOWWINDOW = 0x0040;
  const int EXSTYLE = -20, TOOLWINDOW = 0x00000080, NOACTIVATE_EX = 0x08000000;
  public static void Applica(IntPtr h, int tinta) {
    ACCENTPOLICY p = new ACCENTPOLICY();
    p.Stato = ACRILICO; p.Flag = 2; p.Colore = tinta; p.Anima = 0;
    int misura = Marshal.SizeOf(p);
    IntPtr dati = Marshal.AllocHGlobal(misura);
    Marshal.StructureToPtr(p, dati, false);
    WINCOMPATTRDATA d = new WINCOMPATTRDATA();
    d.Attributo = ATTR_ACCENT; d.Dati = dati; d.Misura = misura;
    if (SetWindowCompositionAttribute(h, ref d) == 0) {
      p.Stato = SFOCATO;
      Marshal.StructureToPtr(p, dati, false);
      SetWindowCompositionAttribute(h, ref d);
    }
    Marshal.FreeHGlobal(dati);
    int tondo = TONDO;
    DwmSetWindowAttribute(h, ANGOLI, ref tondo, sizeof(int));
  }
  public static void Appoggia(IntPtr h) {
    int s = GetWindowLong(h, EXSTYLE);
    SetWindowLong(h, EXSTYLE, s | TOOLWINDOW | NOACTIVATE_EX);
    SetWindowPos(h, BOTTOM, 0, 0, 0, 0, NOSIZE | NOMOVE | NOACTIVATE | SHOWWINDOW);
  }
}
"@

$LOG = Join-Path $PSScriptRoot 'errori.txt'
function Prova($blocco) {
  try { & $blocco }
  catch { "$(Get-Date -f HH:mm:ss)  $($_.Exception.Message)" | Out-File $LOG -Append -Encoding utf8 }
}
function Chiedi($rotta) { try { Invoke-RestMethod -Uri "$BASE$rotta" -TimeoutSec 3 } catch { $null } }

$CLAUDE = Join-Path $env:USERPROFILE '.claude'

function TitoloSessione($file) {
  try {
    foreach ($riga in (Get-Content $file -TotalCount 220 -Encoding UTF8 -ErrorAction Stop)) {
      if ($riga -notlike '*"type":"user"*') { continue }
      try { $o = $riga | ConvertFrom-Json } catch { continue }
      $c = $o.message.content
      $t = if ($c -is [string]) { $c } else { ($c | Where-Object { $_.type -eq 'text' } | Select-Object -First 1).text }
      if ($t -and $t[0] -ne '<' -and $t -notlike 'Caveat:*') { return ($t -replace '\s+', ' ').Trim() }
    }
  } catch {}
  'sessione senza titolo'
}

function Quando($data) {
  $m = [int]((Get-Date) - $data).TotalMinutes
  if ($m -lt 1) { return 'ADESSO' }
  if ($m -lt 60) { return "$m MIN FA" }
  $o = [int]($m / 60)
  if ($o -lt 24) { return "$o H FA" }
  $g = [int]($o / 24)
  if ($g -eq 1) { 'IERI' } else { "$g GIORNI FA" }
}

function Sessioni($quante) {
  $cartella = Join-Path $CLAUDE 'projects'
  if (-not (Test-Path $cartella)) { return @() }
  Get-ChildItem $cartella -Filter '*.jsonl' -Recurse -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending | Select-Object -First $quante | ForEach-Object {
      [pscustomobject]@{
        id     = $_.BaseName
        titolo = TitoloSessione $_.FullName
        quando = Quando $_.LastWriteTime
        # la cartella dove e' nata: ripresa da un'altra parte non e' la stessa sessione
        dove   = Cartella-Sessione $_.FullName
      }
    }
}

function Agenti {
  $cartella = Join-Path $CLAUDE 'agents'
  if (-not (Test-Path $cartella)) { return @() }
  $miei = 'mixy','vally','whyrig','clack','designo'
  Get-ChildItem $cartella -Filter '*.md' -ErrorAction SilentlyContinue |
    Where-Object { $miei -contains $_.BaseName } | ForEach-Object { $_.BaseName }
}

function Manda($corpo) {
  switch ($corpo.tipo) {
    'continua' { ApriClaude '--continue' $env:USERPROFILE }
    'sessione' { if ($corpo.id -match '^[0-9a-f\-]{36}$') { ApriClaude ('--resume ' + $corpo.id) $corpo.dove } }
    'agente'   { ApriClaude ('"usa l' + [char]39 + 'agente ' + $corpo.agente + '"') $env:USERPROFILE }
  }
}
function Taglia($t, $n) { if ($t.Length -gt $n) { $t.Substring(0, $n) + [char]0x2026 } else { $t } }

function Entra($elemento, $ritardo) {
  $sb = New-Object Windows.Media.Animation.Storyboard
  $a = New-Object Windows.Media.Animation.DoubleAnimation
  $a.From = 0; $a.To = 1
  $a.Duration = [Windows.Duration]::new([TimeSpan]::FromMilliseconds(300))
  $a.BeginTime = [TimeSpan]::FromMilliseconds($ritardo)
  [Windows.Media.Animation.Storyboard]::SetTarget($a, $elemento)
  [Windows.Media.Animation.Storyboard]::SetTargetProperty($a, (New-Object Windows.PropertyPath('Opacity')))
  $sb.Children.Add($a); $sb.Begin()
}

function RigaSessione($testo, $quando, $azione) {
  $g = New-Object Windows.Controls.Grid
  $g.Margin = '0,0,0,7'; $g.Cursor = 'Hand'; $g.Opacity = 0
  $g.Background = [Windows.Media.Brushes]::Transparent
  $c1 = New-Object Windows.Controls.ColumnDefinition
  $c2 = New-Object Windows.Controls.ColumnDefinition; $c2.Width = 'Auto'
  $g.ColumnDefinitions.Add($c1); $g.ColumnDefinitions.Add($c2)
  $t1 = New-Object Windows.Controls.TextBlock
  $t1.Text = $testo; $t1.Foreground = '#E8FFFFFF'; $t1.FontSize = 12; $t1.FontFamily = 'Segoe UI'
  $t1.TextTrimming = 'CharacterEllipsis'
  $t2 = New-Object Windows.Controls.TextBlock
  $t2.Text = $quando; $t2.Foreground = '#5CFFFFFF'; $t2.FontSize = 10; $t2.FontFamily = 'Segoe UI'
  $t2.Margin = '10,1,0,0'
  [Windows.Controls.Grid]::SetColumn($t2, 1)
  $g.Children.Add($t1) | Out-Null; $g.Children.Add($t2) | Out-Null
  $g.Add_MouseLeftButtonUp($azione)
  $g.Add_MouseEnter({ $this.Children[0].Foreground = 'White' })
  $g.Add_MouseLeave({ $this.Children[0].Foreground = '#E8FFFFFF' })
  $g
}


# ================= il ponte con l'iPhone =================
# Quello che copi sul telefono finisce negli appunti di questo PC.
# Sta dentro il widget apposta: cosi' non c'e' un secondo programma da tenere acceso.

$PORTA = 8787
$CASSETTO = Join-Path $env:LOCALAPPDATA 'WhyWidget'
if (-not (Test-Path $CASSETTO)) { New-Item -ItemType Directory -Path $CASSETTO -Force | Out-Null }

# la chiave sta fuori dal progetto: questa cartella un giorno finisce su GitHub
$FILE_CHIAVE = Join-Path $CASSETTO 'ponte.txt'
if (Test-Path $FILE_CHIAVE) {
  $CHIAVE = (Get-Content $FILE_CHIAVE -Raw).Trim()
} else {
  $b = New-Object byte[] 9
  [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b)
  $CHIAVE = ([Convert]::ToBase64String($b) -replace '\+', '-' -replace '/', '_' -replace '=', '')
  Set-Content -LiteralPath $FILE_CHIAVE -Value $CHIAVE -Encoding ascii
}

$PONTE = [hashtable]::Synchronized(@{ acceso = $false; testo = ''; quando = $null; errore = '' })

$rsPonte = [runspacefactory]::CreateRunspace()
$rsPonte.ApartmentState = 'STA'    # senza STA gli appunti di Windows non si toccano
$rsPonte.ThreadOptions  = 'ReuseThread'
$rsPonte.Open()
$rsPonte.SessionStateProxy.SetVariable('P', $PONTE)
$rsPonte.SessionStateProxy.SetVariable('CHIAVE', $CHIAVE)
$rsPonte.SessionStateProxy.SetVariable('PORTA', $PORTA)

$psPonte = [powershell]::Create()
$psPonte.Runspace = $rsPonte
$null = $psPonte.AddScript({
  function Rispondi($flusso, $testo) {
    $corpo = [Text.Encoding]::UTF8.GetBytes($testo)
    $testa = [Text.Encoding]::ASCII.GetBytes(
      "HTTP/1.1 200 OK`r`nContent-Type: text/plain; charset=utf-8`r`n" +
      "Content-Length: $($corpo.Length)`r`nConnection: close`r`n`r`n")
    $flusso.Write($testa, 0, $testa.Length)
    $flusso.Write($corpo, 0, $corpo.Length)
    $flusso.Flush()
  }

  # se il widget e' appena ripartito, la porta puo' essere ancora occupata
  # da quello di prima: aspetta invece di arrendersi
  $ascolto = $null
  for ($tentativo = 1; $tentativo -le 12; $tentativo++) {
    try {
      $ascolto = New-Object Net.Sockets.TcpListener([Net.IPAddress]::Any, $PORTA)
      $ascolto.ExclusiveAddressUse = $false
      $ascolto.Start()
      $P.acceso = $true; $P.errore = ''
      break
    } catch {
      $ascolto = $null; $P.errore = $_.Exception.Message
      Start-Sleep -Seconds 5
    }
  }
  if (-not $ascolto) { return }

  while ($true) {
    $cliente = $null
    try {
      $cliente = $ascolto.AcceptTcpClient()
      $cliente.ReceiveTimeout = 5000
      $f = $cliente.GetStream()

      # la testa della richiesta e' ASCII, il corpo e' UTF-8: vanno tenuti byte per
      # byte, altrimenti gli accenti dell'iPhone arrivano rotti
      $pezzo = New-Object byte[] 8192
      $tutto = New-Object Collections.Generic.List[byte]
      $stacco = -1
      while ($stacco -lt 0) {
        $letti = $f.Read($pezzo, 0, $pezzo.Length)
        if ($letti -le 0) { break }
        for ($i = 0; $i -lt $letti; $i++) { $tutto.Add($pezzo[$i]) }
        for ($i = 0; $i -le $tutto.Count - 4; $i++) {
          if ($tutto[$i] -eq 13 -and $tutto[$i+1] -eq 10 -and $tutto[$i+2] -eq 13 -and $tutto[$i+3] -eq 10) { $stacco = $i; break }
        }
        if ($tutto.Count -gt 2000000) { break }
      }
      if ($stacco -lt 0) { $cliente.Close(); continue }

      $righe = ([Text.Encoding]::ASCII.GetString($tutto.ToArray(), 0, $stacco)) -split "`r`n"
      $lunghez = 0
      foreach ($r in $righe) { if ($r -match '^(?i)content-length:\s*(\d+)') { $lunghez = [int]$Matches[1] } }
      $avuti = $tutto.Count - ($stacco + 4)
      while ($avuti -lt $lunghez) {
        $letti = $f.Read($pezzo, 0, $pezzo.Length)
        if ($letti -le 0) { break }
        for ($i = 0; $i -lt $letti; $i++) { $tutto.Add($pezzo[$i]) }
        $avuti = $tutto.Count - ($stacco + 4)
      }
      $corpo = if ($lunghez -gt 0) {
        [Text.Encoding]::UTF8.GetString($tutto.ToArray(), $stacco + 4, [Math]::Min($lunghez, $avuti))
      } else { '' }

      $percorso = ((($righe[0] -split ' ')[1]) -split '\?')[0]
      if ($percorso.Trim('/') -eq $CHIAVE) {
        $P.testo = $corpo; $P.quando = Get-Date
        try { Set-Clipboard -Value $corpo } catch {}
        if ($corpo -match '^https?://') { try { Start-Process $corpo.Trim() } catch {} }
        Rispondi $f 'preso'
      } else {
        Rispondi $f 'WhyWidget'
      }
    } catch { }
    finally { if ($cliente) { $cliente.Close() } }
  }
})
$null = $psPonte.BeginInvoke()


# ================= i dispositivi, letti in disparte =================
# Interrogare Windows su cosa e' collegato costa secondi: lo fa un filo suo, ogni
# minuto, e lascia qui il risultato. La finestra non si ferma mai.

$DISPOSITIVI = [hashtable]::Synchronized(@{ elenco = @() })
$rsDisp = [runspacefactory]::CreateRunspace()
$rsDisp.ApartmentState = 'STA'; $rsDisp.ThreadOptions = 'ReuseThread'
$rsDisp.Open()
$rsDisp.SessionStateProxy.SetVariable('D', $DISPOSITIVI)
$rsDisp.SessionStateProxy.SetVariable('MODULO', (Join-Path $RADICE 'parti\dispositivi.ps1'))
$psDisp = [powershell]::Create()
$psDisp.Runspace = $rsDisp
$null = $psDisp.AddScript({
  . $MODULO
  while ($true) {
    try { $D.elenco = @(Dispositivi-Collegati) } catch { }
    Start-Sleep -Seconds 60
  }
})
$null = $psDisp.BeginInvoke()

# riga di un dispositivo: nome a sinistra, e a destra la carica dove Windows la dice
function RigaDispositivo($d) {
  $g = New-Object Windows.Controls.Grid
  $g.Margin = '0,0,0,6'; $g.Opacity = 0
  $c1 = New-Object Windows.Controls.ColumnDefinition
  $c2 = New-Object Windows.Controls.ColumnDefinition; $c2.Width = 'Auto'
  $g.ColumnDefinitions.Add($c1); $g.ColumnDefinitions.Add($c2)

  $t = New-Object Windows.Controls.TextBlock
  $t.Text = $d.nome; $t.Foreground = '#E8FFFFFF'; $t.FontSize = 10.5; $t.FontFamily = 'Segoe UI'
  $t.TextTrimming = 'CharacterEllipsis'; $t.VerticalAlignment = 'Center'

  $destra = New-Object Windows.Controls.StackPanel
  $destra.Orientation = 'Horizontal'; $destra.VerticalAlignment = 'Center'
  $destra.Margin = '8,0,0,0'
  [Windows.Controls.Grid]::SetColumn($destra, 1)

  if ($null -ne $d.carica) {
    # barra piatta: il pieno e' bianco, e diventa rosso solo agli sgoccioli
    $fondo = New-Object Windows.Controls.Grid
    $fondo.Width = 24; $fondo.Height = 4; $fondo.Margin = '0,0,6,0'; $fondo.VerticalAlignment = 'Center'
    $vuoto = New-Object Windows.Shapes.Rectangle
    $vuoto.Fill = '#26FFFFFF'; $vuoto.RadiusX = 2; $vuoto.RadiusY = 2
    $pieno = New-Object Windows.Shapes.Rectangle
    $pieno.Fill = $(if ($d.carica -le 20) { '#FF5A6B' } else { '#E8FFFFFF' })
    $pieno.RadiusX = 2; $pieno.RadiusY = 2
    $pieno.Width = [Math]::Max(2, 24 * $d.carica / 100); $pieno.HorizontalAlignment = 'Left'
    $fondo.Children.Add($vuoto) | Out-Null; $fondo.Children.Add($pieno) | Out-Null
    $destra.Children.Add($fondo) | Out-Null
    $p = New-Object Windows.Controls.TextBlock
    $p.Text = [string]$d.carica + '%'; $p.Foreground = '#E8FFFFFF'; $p.FontSize = 9.5
    $p.FontFamily = 'Segoe UI'; $p.VerticalAlignment = 'Center'
    $destra.Children.Add($p) | Out-Null
  } else {
    $q = New-Object Windows.Shapes.Ellipse
    $q.Width = 5; $q.Height = 5; $q.VerticalAlignment = 'Center'
    $q.Fill = $(if ($d.acceso) { '#FF5A6B' } else { '#5CFFFFFF' })
    $destra.Children.Add($q) | Out-Null
  }

  $g.Children.Add($t) | Out-Null; $g.Children.Add($destra) | Out-Null
  $g
}

function Aggiorna {
  $xOra.Text  = (Get-Date).ToString('HH:mm')
  $xData.Text = (Get-Date).ToString('ddd d MMM', [Globalization.CultureInfo]::GetCultureInfo('it-IT')).ToUpper()

  $sessioni = @(Sessioni ([int]$IMP.quante.sessioni + 1))
  if ($sessioni.Count -gt 0) { $xUltimaQ.Text = $sessioni[0].quando } else { $xUltimaQ.Text = '' }

  $xSessioni.Children.Clear()
  $ritardo = 0
  foreach ($s in $sessioni | Select-Object -Skip 1 -First ([int]$IMP.quante.sessioni)) {
    $id = $s.id
    $dove = $s.dove
    $r = RigaSessione (Taglia $s.titolo 44) $s.quando { Manda @{ tipo = 'sessione'; id = $id; dove = $dove } }.GetNewClosure()
    $xSessioni.Children.Add($r) | Out-Null
    Entra $r $ritardo
    $ritardo += 70
  }

  $xAgenti.Children.Clear()
  foreach ($a in (Agenti)) {
    $nome = $a
    $p = New-Object Windows.Controls.Border
    $p.Background = '#14FFFFFF'; $p.BorderBrush = '#26FFFFFF'; $p.BorderThickness = '1'
    $p.CornerRadius = 9; $p.Padding = '8,3,8,4'; $p.Margin = '0,0,5,5'
    $p.Cursor = 'Hand'; $p.Opacity = 0
    $t = New-Object Windows.Controls.TextBlock
    $t.Text = $nome; $t.Foreground = '#E8FFFFFF'; $t.FontSize = 10.5; $t.FontFamily = 'Segoe UI'
    $p.Child = $t
    $p.Add_MouseLeftButtonUp({ Manda @{ tipo = 'agente'; agente = $nome } }.GetNewClosure())
    $p.Add_MouseEnter({ $this.Background = '#38FFFFFF' })
    $p.Add_MouseLeave({ $this.Background = '#14FFFFFF' })
    $xAgenti.Children.Add($p) | Out-Null
    Entra $p $ritardo
    $ritardo += 50
  }

  # cosa e' collegato adesso
  $xCollegati.Children.Clear()
  $collegati = @($DISPOSITIVI.elenco | Select-Object -First ([int]$IMP.quante.collegati))
  if ($collegati.Count -eq 0) {
    $t = New-Object Windows.Controls.TextBlock
    $t.Text = 'sto guardando...'; $t.Foreground = '#5CFFFFFF'; $t.FontSize = 10.5; $t.FontFamily = 'Segoe UI'
    $xCollegati.Children.Add($t) | Out-Null
  } else {
    foreach ($d in $collegati) {
      $r = RigaDispositivo $d
      $xCollegati.Children.Add($r) | Out-Null
      Entra $r $ritardo
      $ritardo += 50
    }
  }

  # il ponte con l'iPhone
  if ($PONTE.acceso) {
    $xPonteVivo.Fill = '#FF5A6B'
    if ($PONTE.quando) {
      $xPonteT.Text = Taglia (($PONTE.testo -replace '\s+', ' ').Trim()) 40
      $xPonteQ.Text = 'ARRIVATO ' + (Quando $PONTE.quando)
    } else {
      $xPonteT.Text = 'copia sul telefono, si incolla qui'
      $xPonteQ.Text = 'PRONTO'
    }
  } else {
    $xPonteVivo.Fill = '#5CFFFFFF'
    $xPonteT.Text = 'ponte spento'
    $xPonteQ.Text = if ($PONTE.errore) { 'PORTA OCCUPATA' } else { 'NON PARTITO' }
  }

  $xPiedeQ.Text = 'AGGIORNATO ' + (Get-Date).ToString('HH:mm')

  $lista = Chiedi '/dati'
  $n = if ($lista) { @($lista.voci | Where-Object { $_.s -eq 'ora' }).Count } else { 0 }
  $xContaLavori.Text = if ($lista) { [string]$n } else { '--' }

  $xLavori.Children.Clear()
  if ($lista) {
    foreach ($v in ($lista.voci | Where-Object { $_.s -eq 'ora' } | Select-Object -First 2)) {
      $t = New-Object Windows.Controls.TextBlock
      $t.Text = Taglia $v.t 22
      $t.Foreground = '#8FFFFFFF'; $t.FontSize = 10.5; $t.FontFamily = 'Segoe UI'
      $t.TextTrimming = 'CharacterEllipsis'; $t.Margin = '0,0,0,3'; $t.Opacity = 0
      $xLavori.Children.Add($t) | Out-Null
      Entra $t $ritardo
      $ritardo += 55
    }
  }
}

# le icone: disegnate a mano, vettoriali, sempre in movimento
$xIcoSessioni.Content     = Icona 'sessioni' '#9EFFFFFF'
$xIcoAgenti.Content       = Icona 'agenti' '#9EFFFFFF'
$xIcoLavori.Content       = Icona 'lavori' '#9EFFFFFF'
$xIcoCollegati.Content    = Icona 'dispositivi' '#9EFFFFFF'
$xIcoPonte.Content        = Icona 'iphone' '#9EFFFFFF'
$xIcoImpostazioni.Content = Icona 'impostazioni' '#9EFFFFFF'


# ================= il pannello impostazioni =================
# Si cambia da qui e si vede subito: niente file da aprire, niente riavvio.

function Bottone($testo, $acceso, $azione) {
  $b = New-Object Windows.Controls.Border
  $b.Background = $(if ($acceso) { '#38FFFFFF' } else { '#14FFFFFF' })
  $b.BorderBrush = $(if ($acceso) { '#5CFFFFFF' } else { '#26FFFFFF' })
  $b.BorderThickness = '1'; $b.CornerRadius = 9; $b.Padding = '9,3,9,4'; $b.Margin = '0,0,5,5'
  $b.Cursor = 'Hand'
  $t = New-Object Windows.Controls.TextBlock
  $t.Text = $testo; $t.FontSize = 10.5; $t.FontFamily = 'Segoe UI'
  $t.Foreground = $(if ($acceso) { 'White' } else { '#9EFFFFFF' })
  $b.Child = $t
  $b.Add_MouseLeftButtonUp($azione)
  $b
}

function RigaComando($etichetta, $bottoni) {
  $sp = New-Object Windows.Controls.StackPanel
  $sp.Margin = '0,0,0,10'
  $e = New-Object Windows.Controls.TextBlock
  $e.Text = $etichetta; $e.Foreground = '#5CFFFFFF'; $e.FontSize = 10.5
  $e.FontFamily = 'Segoe UI'; $e.FontWeight = 'SemiBold'; $e.Margin = '0,0,0,6'
  $sp.Children.Add($e) | Out-Null
  $fila = New-Object Windows.Controls.WrapPanel
  foreach ($b in $bottoni) { $fila.Children.Add($b) | Out-Null }
  $sp.Children.Add($fila) | Out-Null
  $sp
}

function Applica {
  Salva-Impostazioni $IMP
  Prova { Aggiorna }
  Prova { MettiAPosto }
  DisegnaPannello
}

function MettiAPosto {
  $w.UpdateLayout()
  $schermo = [Windows.SystemParameters]::WorkArea
  $w.Left = if ($IMP.lato -eq 'destra') { $schermo.Width - $w.ActualWidth - 18 } else { 18 }
  $w.Top  = if ($IMP.alto -eq 'sopra') { 18 } else { $schermo.Height - $w.ActualHeight - 18 }
}

function DisegnaPannello {
  $xComandi.Children.Clear()

  $xComandi.Children.Add((RigaComando 'DA CHE PARTE' @(
    (Bottone 'SINISTRA' ($IMP.lato -eq 'sinistra') { $IMP.lato = 'sinistra'; Applica }),
    (Bottone 'DESTRA'   ($IMP.lato -eq 'destra')   { $IMP.lato = 'destra';   Applica })
  ))) | Out-Null

  $xComandi.Children.Add((RigaComando 'IN ALTO O IN BASSO' @(
    (Bottone 'IN ALTO'  ($IMP.alto -eq 'sopra') { $IMP.alto = 'sopra'; Applica }),
    (Bottone 'IN BASSO' ($IMP.alto -eq 'sotto') { $IMP.alto = 'sotto'; Applica })
  ))) | Out-Null

  $etichette = [ordered]@{
    sessioni = 'SESSIONI'; agenti = 'AGENTI'; lavori = 'DA FARE'
    collegati = 'COLLEGATI'; iphone = 'IPHONE'
  }
  $bottoni = @()
  foreach ($k in $etichette.Keys) {
    $chiave = $k
    $bottoni += (Bottone $etichette[$k] $IMP.tessere[$chiave] {
      $IMP.tessere[$chiave] = -not $IMP.tessere[$chiave]; Applica
    }.GetNewClosure())
  }
  $xComandi.Children.Add((RigaComando 'COSA SI VEDE' $bottoni)) | Out-Null

  $xComandi.Children.Add((RigaComando ('QUANTE SESSIONI   ' + [int]$IMP.quante.sessioni) @(
    (Bottone 'MENO' $false { $IMP.quante.sessioni = [Math]::Max(1, $IMP.quante.sessioni - 1); Applica }),
    (Bottone 'PIU' $false { $IMP.quante.sessioni = [Math]::Min(8, $IMP.quante.sessioni + 1); Applica })
  ))) | Out-Null

  $nota = New-Object Windows.Controls.TextBlock
  $nota.Text = 'Si salva da solo.'; $nota.Foreground = '#5CFFFFFF'; $nota.FontSize = 10
  $nota.FontFamily = 'Segoe UI'
  $xComandi.Children.Add($nota) | Out-Null
}

$xIcoPannello.Content = Icona 'impostazioni' '#E8FFFFFF'
$xCardImpostazioni.Add_MouseLeftButtonUp({
  DisegnaPannello
  $xPannello.Visibility = 'Visible'
  $a = New-Object Windows.Media.Animation.DoubleAnimation
  $a.From = 0; $a.To = 1; $a.Duration = [Windows.Duration]::new([TimeSpan]::FromMilliseconds(200))
  $xPannello.BeginAnimation([Windows.UIElement]::OpacityProperty, $a)
})
$xChiudi.Add_MouseLeftButtonUp({ $xPannello.Visibility = 'Collapsed' })

$xCardPonte.Add_MouseLeftButtonUp({ if ($PONTE.testo) { Set-Clipboard -Value $PONTE.testo } })
$xCardRiprendi.Add_MouseLeftButtonUp({ Manda @{ tipo = 'continua' } })
$xCardLavori.Add_MouseLeftButtonUp({ Start-Process "$BASE/lavori" })

$w.Add_Loaded({
  Prova { Aggiorna }
  $w.UpdateLayout()

  if ($Foto) {
    # ritratto della finestra, non dello schermo: viene giusto anche se sopra c'e'
    # un video a tutto schermo, e non disturba quello che sta facendo lui
    # il vetro lo fa Windows dietro la finestra, e nel ritratto non ci finisce:
    # ci metto un fondo scuro finto, cosi' almeno il contenuto si legge
    $w.Opacity = 1
    $w.Background = '#1B1B22'
    if ($ConPannello) { DisegnaPannello; $xPannello.Visibility = 'Visible' }
    $scatta = New-Object Windows.Threading.DispatcherTimer
    $scatta.Interval = [TimeSpan]::FromSeconds(3)
    $scatta.Add_Tick({
      $scatta.Stop()
      $w.UpdateLayout()
      $cx = [int]$w.ActualWidth; $cy = [int]$w.ActualHeight
      $rtb = New-Object Windows.Media.Imaging.RenderTargetBitmap($cx, $cy, 96, 96, [Windows.Media.PixelFormats]::Pbgra32)
      $rtb.Render($w)
      $enc = New-Object Windows.Media.Imaging.PngBitmapEncoder
      $enc.Frames.Add([Windows.Media.Imaging.BitmapFrame]::Create($rtb))
      $flusso = [IO.File]::Create($Foto)
      $enc.Save($flusso); $flusso.Close()
      [Environment]::Exit(0)
    })
    $scatta.Start()
    return
  }
  $schermo = [Windows.SystemParameters]::WorkArea
  $w.Left = if ($LATO -eq 'destra') { $schermo.Width - $w.ActualWidth - 18 } else { 18 }
  $w.Top  = 18

  $h = (New-Object Windows.Interop.WindowInteropHelper $w).Handle
  [Vetro]::Applica($h, 0x4D14141A)
  [Vetro]::Appoggia($h)

  $op = New-Object Windows.Media.Animation.DoubleAnimation
  $op.From = 0; $op.To = 1
  $op.Duration = [Windows.Duration]::new([TimeSpan]::FromMilliseconds(400))
  $w.BeginAnimation([Windows.Window]::OpacityProperty, $op)

  $ease = New-Object Windows.Media.Animation.ExponentialEase
  $ease.EasingMode = 'EaseOut'; $ease.Exponent = 5
  foreach ($asse in @([Windows.Media.ScaleTransform]::ScaleXProperty, [Windows.Media.ScaleTransform]::ScaleYProperty)) {
    $sc = New-Object Windows.Media.Animation.DoubleAnimation
    $sc.From = 0.94; $sc.To = 1
    $sc.Duration = [Windows.Duration]::new([TimeSpan]::FromMilliseconds(520))
    $sc.EasingFunction = $ease
    $xEntrata.BeginAnimation($asse, $sc)
  }
})

$timer = New-Object Windows.Threading.DispatcherTimer
$timer.Interval = [TimeSpan]::FromSeconds(20)
$timer.Add_Tick({
  Prova { Aggiorna }
  $h = (New-Object Windows.Interop.WindowInteropHelper $w).Handle
  [Vetro]::Appoggia($h)
})
$timer.Start()

$w.ShowDialog() | Out-Null
