@echo off
title WhyWidget - apro la porta per l'iPhone
chcp 65001 >nul

rem  Una volta sola. Serve l'amministratore solo per il firewall di Windows.
rem  Il controllo si fa con fltmc: net session manda in loop, gia' visto.
fltmc >nul 2>&1
if errorlevel 1 (
  powershell -NoProfile -Command "Start-Process '%~f0' -Verb RunAs"
  exit /b
)

echo.
echo   Apro la porta 8787, quella che usa l'iPhone per parlare col PC.
echo.

powershell -NoProfile -Command ^
  "if (-not (Get-NetFirewallRule -DisplayName 'WhyWidget: ponte iPhone' -ErrorAction SilentlyContinue)) { New-NetFirewallRule -DisplayName 'WhyWidget: ponte iPhone' -Direction Inbound -Action Allow -Protocol TCP -LocalPort 8787 -Profile Private,Domain | Out-Null; Write-Host '   fatto.' } else { Write-Host '   era gia'' aperta.' }"

echo.
powershell -NoProfile -Command ^
  "$r = Get-NetConnectionProfile | Select-Object -First 1; if ($r.NetworkCategory -eq 'Public') { Write-Host '   ATTENZIONE: questa rete e'' segnata come Pubblica.'; Write-Host '   Vai in Impostazioni, Rete, e mettila su Privata, se no il telefono non passa.' } else { Write-Host ('   Rete: ' + $r.NetworkCategory + '. Va bene cosi''.') }"

echo.
echo   Puoi chiudere questa finestra.
echo.
timeout /t 10 >nul
