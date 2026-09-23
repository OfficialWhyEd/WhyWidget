# Fa WhyWidget.ico: Aphi (il marchio bianco gia' approvato, E:\Dev\Mascotte\out\icone)
# sopra una tessera scura arrotondata, la stessa forma delle tessere del widget.
# Dentro ci sono tutte le misure che Windows chiede (barra, Alt+Tab, Esplora file).
# Non e' grafica fatta a mano: e' solo impaginazione e formato di un disegno che esiste gia'.
Add-Type -AssemblyName System.Drawing

$marchio = "E:\Dev\Mascotte\out\icone\aphelios-marchio-bianco.png"
$uscita  = Join-Path $PSScriptRoot "WhyWidget.ico"
$fondo   = [System.Drawing.ColorTranslator]::FromHtml("#1C1C20")   # la tessera sul desktop scuro
$bordo   = [System.Drawing.ColorTranslator]::FromHtml("#3A3A3E")   # il suo filo di luce

$sorgente = [System.Drawing.Image]::FromFile($marchio)

function Tessera([int]$n) {
  $b = New-Object System.Drawing.Bitmap $n, $n, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $g = [System.Drawing.Graphics]::FromImage($b)
  $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
  $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
  $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
  $g.Clear([System.Drawing.Color]::Transparent)

  # angoli tondi: come le tessere (22 su 164), un po' di piu' sulle misure piccole
  $r = [Math]::Max(2.0, $n * 0.2)
  $p = New-Object System.Drawing.Drawing2D.GraphicsPath
  $w = $n - 1.0
  $p.AddArc(0, 0, 2*$r, 2*$r, 180, 90)
  $p.AddArc($w - 2*$r, 0, 2*$r, 2*$r, 270, 90)
  $p.AddArc($w - 2*$r, $w - 2*$r, 2*$r, 2*$r, 0, 90)
  $p.AddArc(0, $w - 2*$r, 2*$r, 2*$r, 90, 90)
  $p.CloseFigure()
  $g.FillPath((New-Object System.Drawing.SolidBrush $fondo), $p)
  if ($n -ge 32) { $g.DrawPath((New-Object System.Drawing.Pen $bordo, 1), $p) }

  # Aphi: riempie il 78% della tessera, al centro
  $m = [Math]::Round($n * 0.78)
  $x = [Math]::Round(($n - $m) / 2)
  $y = [Math]::Round(($n - $m) / 2)
  $g.DrawImage($sorgente, (New-Object System.Drawing.Rectangle $x, $y, $m, $m))
  $g.Dispose()
  return $b
}

# una voce dell'ico in formato bitmap (quello che GDI+ legge senza storie)
function VoceBmp([System.Drawing.Bitmap]$b) {
  $n = $b.Width
  $riga = $n * 4
  $ms = New-Object System.IO.MemoryStream
  $bw = New-Object System.IO.BinaryWriter $ms
  $bw.Write([int32]40); $bw.Write([int32]$n); $bw.Write([int32]($n * 2)); $bw.Write([int16]1); $bw.Write([int16]32)
  $bw.Write([int32]0); $bw.Write([int32]($riga * $n)); $bw.Write([int32]0); $bw.Write([int32]0); $bw.Write([int32]0); $bw.Write([int32]0)
  for ($y = $n - 1; $y -ge 0; $y--) {
    for ($x = 0; $x -lt $n; $x++) {
      $c = $b.GetPixel($x, $y)
      $bw.Write([byte]$c.B); $bw.Write([byte]$c.G); $bw.Write([byte]$c.R); $bw.Write([byte]$c.A)
    }
  }
  # la maschera: tutta a zero, l'alfa fa il lavoro
  $mask = [Math]::Ceiling($n / 32) * 4
  for ($y = 0; $y -lt $n; $y++) { for ($k = 0; $k -lt $mask; $k++) { $bw.Write([byte]0) } }
  $bw.Flush()
  return ,$ms.ToArray()
}

function VocePng([System.Drawing.Bitmap]$b) {
  $ms = New-Object System.IO.MemoryStream
  $b.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
  return ,$ms.ToArray()
}

$misure = 16, 20, 24, 32, 40, 48, 64, 256
$voci = @()
foreach ($n in $misure) {
  $b = Tessera $n
  if ($n -eq 256) { $dati = VocePng $b } else { $dati = VoceBmp $b }
  $voci += ,@{ n = $n; dati = $dati }

  $b.Dispose()
}

$out = New-Object System.IO.MemoryStream
$w = New-Object System.IO.BinaryWriter $out
$w.Write([int16]0); $w.Write([int16]1); $w.Write([int16]$voci.Count)
$offset = 6 + 16 * $voci.Count
foreach ($v in $voci) {
  $n = $v.n
  $w.Write([byte]($(if ($n -ge 256) { 0 } else { $n })))
  $w.Write([byte]($(if ($n -ge 256) { 0 } else { $n })))
  $w.Write([byte]0); $w.Write([byte]0)
  $w.Write([int16]1); $w.Write([int16]32)
  $w.Write([int32]([byte[]]$v.dati).Length); $w.Write([int32]$offset)
  $offset += $v.dati.Length
}
foreach ($v in $voci) { $w.Write([byte[]]$v.dati) }
$w.Flush()
[System.IO.File]::WriteAllBytes($uscita, $out.ToArray())
$sorgente.Dispose()
"Fatto: $uscita ($($voci.Count) misure)"
