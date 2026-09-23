# Le impostazioni del widget: stanno in un file JSON in AppData, fuori dal progetto.
# Si cambiano dal pannello dentro il widget: qui non c'e' niente da toccare a mano.

$CARTELLA_DATI = Join-Path $env:LOCALAPPDATA 'WhyWidget'
$FILE_IMPOST   = Join-Path $CARTELLA_DATI 'widget.json'

function Impostazioni-Base {
  [ordered]@{
    lato    = 'destra'     # 'destra' o 'sinistra'
    alto    = 'sopra'      # 'sopra' o 'sotto'
    tessere = [ordered]@{
      collegati    = $true
      iphone       = $true
      agenti       = $true
      lavori       = $true
      sessioni     = $true
    }
    quante  = [ordered]@{ sessioni = 3; collegati = 3; lavori = 2 }
  }
}

function Unisci($base, $letto) {
  if ($null -eq $letto) { return $base }
  foreach ($k in @($base.Keys)) {
    $v = $null
    if ($letto -is [hashtable] -or $letto -is [System.Collections.Specialized.OrderedDictionary]) {
      if ($letto.Contains($k)) { $v = $letto[$k] }
    } elseif ($letto.PSObject.Properties.Name -contains $k) { $v = $letto.$k }
    if ($null -eq $v) { continue }
    if ($base[$k] -is [System.Collections.Specialized.OrderedDictionary]) { $base[$k] = Unisci $base[$k] $v }
    else { $base[$k] = $v }
  }
  $base
}

function Leggi-Impostazioni {
  $i = Impostazioni-Base
  if (Test-Path $FILE_IMPOST) {
    try { $i = Unisci $i (Get-Content $FILE_IMPOST -Raw -Encoding utf8 | ConvertFrom-Json) } catch { }
  }
  if ($i.lato -ne 'sinistra') { $i.lato = 'destra' }
  if ($i.alto -ne 'sotto')    { $i.alto = 'sopra' }
  foreach ($k in @($i.quante.Keys)) {
    if ($i.quante[$k] -lt 1) { $i.quante[$k] = 1 }
    if ($i.quante[$k] -gt 8) { $i.quante[$k] = 8 }
  }
  $i
}

function Salva-Impostazioni($i) {
  if (-not (Test-Path $CARTELLA_DATI)) { New-Item -ItemType Directory -Path $CARTELLA_DATI -Force | Out-Null }
  ($i | ConvertTo-Json -Depth 8) | Set-Content -LiteralPath $FILE_IMPOST -Encoding utf8
}
