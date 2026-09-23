# Aprire una sessione deve dare la stessa identica cosa che ottiene lui a mano:
# stesso terminale (Windows Terminal, profilo predefinito) e soprattutto
# STESSA CARTELLA. Una sessione ripresa dalla cartella sbagliata non e' quella sessione.

$CLAUDE = Join-Path $env:USERPROFILE '.claude'

# la cartella di lavoro sta scritta dentro il diario della sessione
function Cartella-Sessione($fileJsonl) {
  try {
    $lettore = [IO.File]::OpenText($fileJsonl)
    try {
      for ($i = 0; $i -lt 40; $i++) {
        $riga = $lettore.ReadLine()
        if ($null -eq $riga) { break }
        if ($riga -notlike '*"cwd"*') { continue }
        $o = $riga | ConvertFrom-Json
        if ($o.cwd -and (Test-Path $o.cwd)) { return $o.cwd }
      }
    } finally { $lettore.Close() }
  } catch { }
  $env:USERPROFILE
}

# apre il terminale come lo apre lui, nella cartella giusta
function ApriClaude($argomenti, $cartella) {
  if (-not $cartella) { $cartella = $env:USERPROFILE }
  $riga = if ($argomenti) { "claude $argomenti" } else { 'claude' }

  $wt = Get-Command wt.exe -ErrorAction SilentlyContinue
  if ($wt) {
    # -d: parte gia' dentro la cartella. Il profilo e' quello predefinito, come quando apre lui.
    Start-Process $wt.Source -ArgumentList @('-d', $cartella, 'powershell.exe', '-NoExit', '-Command', $riga)
  } else {
    Start-Process powershell.exe -ArgumentList @('-NoExit', '-Command', "Set-Location -LiteralPath '$cartella'; $riga")
  }
}
