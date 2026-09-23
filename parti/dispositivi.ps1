# Cosa e' collegato al PC, e quanta batteria gli resta.
# Windows la percentuale la da' solo per certi dispositivi (quelli Bluetooth LE che
# la dichiarano). Dove non la da', qui non viene inventata: si dice solo "collegato".
# Gira in un filo suo perche' interrogare i dispositivi costa mezzo secondo.

$CHIAVE_BATTERIA = '{104EA319-6EE2-4701-BD47-8DDBF425BBE5} 2'

function Batteria-Di($instanceId) {
  try {
    $p = Get-PnpDeviceProperty -InstanceId $instanceId -KeyName $CHIAVE_BATTERIA -ErrorAction Stop
    if ($null -ne $p.Data -and $p.Data -ge 0 -and $p.Data -le 100) { return [int]$p.Data }
  } catch { }
  $null
}

# Windows chiama le cose come gli pare: "Altoparlanti (Focusrite USB Audio)".
# Qui resta solo il nome vero.
function Nome-Pulito($n) {
  if (-not $n) { return '' }
  $n = $n.ToString().Trim()
  if ($n -match '\(([^)]{3,})\)') { $n = $Matches[1] }
  $n = $n -replace '^(Altoparlanti|Speakers|Cuffie|Headphones|Auricolari)\s*[-:]?\s*', ''
  $n = $n -replace '\s{2,}', ' '
  $n.Trim()
}

function Dispositivi-Collegati {
  $fuori = New-Object Collections.Generic.List[object]

  # 1) Bluetooth e Bluetooth LE: solo i dispositivi veri, non i servizi di sistema
  try {
    foreach ($d in (Get-PnpDevice -PresentOnly -ErrorAction SilentlyContinue |
                    Where-Object { $_.InstanceId -like ('BTHENUM' + [char]92 + 'DEV_*') -or
                                   $_.InstanceId -like ('BTHLE' + [char]92 + 'DEV_*') -or
                                   $_.InstanceId -like ('BTHLEDEVICE' + [char]92 + '*') })) {
      $fuori.Add([pscustomobject]@{
        nome     = Nome-Pulito $d.FriendlyName
        via      = 'BLUETOOTH'
        carica   = (Batteria-Di $d.InstanceId)
        acceso   = ($d.Status -eq 'OK')
      })
    }
  } catch { }

  # 2) qualsiasi altro dispositivo che dichiara una batteria (penne, tastiere, cuffie USB)
  try {
    foreach ($d in (Get-PnpDevice -PresentOnly -Class HIDClass -ErrorAction SilentlyContinue |
                    Where-Object { $_.InstanceId -like ('*BTHLE*') -or $_.InstanceId -like ('*BTHENUM*') -or
                                   $_.FriendlyName -match 'Bluetooth|Wireless|senza fili' })) {
      $c = Batteria-Di $d.InstanceId
      if ($null -ne $c -and -not ($fuori | Where-Object { $_.nome -eq $d.FriendlyName })) {
        $fuori.Add([pscustomobject]@{ nome = (Nome-Pulito $d.FriendlyName); via = 'SENZA FILO'; carica = $c; acceso = $true })
      }
    }
  } catch { }

  # 3) batteria del PC: sui fissi non c'e', sui portatili si'
  try {
    foreach ($b in (Get-CimInstance Win32_Battery -ErrorAction SilentlyContinue)) {
      $fuori.Add([pscustomobject]@{ nome = 'BATTERIA PC'; via = 'INTERNA'; carica = [int]$b.EstimatedChargeRemaining; acceso = $true })
    }
  } catch { }

  # 4) l'uscita audio in uso: e' sempre roba collegata, e a lui interessa vederla
  try {
    $a = Get-PnpDevice -PresentOnly -Class AudioEndpoint -ErrorAction SilentlyContinue |
         Where-Object { $_.Status -eq 'OK' -and $_.FriendlyName -notmatch 'Microfono|Microphone|Mix|Ingresso' } |
         Select-Object -First 1
    if ($a) { $fuori.Add([pscustomobject]@{ nome = (Nome-Pulito $a.FriendlyName); via = 'AUDIO'; carica = $null; acceso = $true }) }
  } catch { }

  # chi ha la percentuale sta in cima, poi il resto
  @($fuori | Sort-Object @{ e = { if ($null -eq $_.carica) { 1 } else { 0 } } }, nome)
}
