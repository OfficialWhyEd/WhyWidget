# WhyWidget

**A desktop widget for Windows, built like iPhone widgets: tiles of different sizes, data that expires, you choose what to show.**

Il pannello sempre aperto sul bordo dello schermo: sessioni di Claude Code da riprendere, lista dei lavori,
batteria di mouse e cuffie, stato del PC, e un ponte verso l'iPhone per copiare sul telefono e incollare sul PC.

## Cosa c'è
| Parte | Cosa fa |
|---|---|
| `programma/*.cs` | il programma vero in C# (WinForms): `Finestra`, `Tessere`, `Icone` animate, `Impostazioni`, `Ponte` |
| `programma/COMPILA.cmd` | compila con il `csc` di .NET Framework, senza Visual Studio |
| `WhyWidget.ps1` + `parti/` | la prima versione in PowerShell, ancora funzionante |
| `Ponte.cs` | server HTTP sulla porta 8787: dal telefono si manda un testo e finisce negli appunti del PC |

## Come si usa
```
programma\COMPILA.cmd      (crea WhyWidget.exe)
AVVIA WIDGET.cmd           (avvia la versione PowerShell)
```

## L'idea
Architettura presa da WidgetKit di Apple: ogni tessera ha una misura (piccola, media, grande), una fonte di dati
e una scadenza. Il widget non chiede niente: si aggiorna da solo. Le impostazioni stanno dentro il widget,
niente file da modificare a mano.

## Stato
Funziona tutti i giorni. Da fare: l'exe compilato va rifatto sull'ultima versione, il design va rifinito,
e accanto al widget andrà la mascotte Aphelios.

---

Parte di **[WhyEcosystem 2023-2026](https://github.com/OfficialWhyEd/WhyEcosystem-2023-2026)**: il percorso di WhyEd, producer e sound engineer che costruisce sistemi AI dirigendo gli agenti.  
Costruito da WhyEd con Claude Code
