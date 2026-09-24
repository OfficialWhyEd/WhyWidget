# WhyWidget

**A desktop widget for Windows built on the same model as iPhone widgets: glass tiles of different sizes, each with its own data and expiry, one single .exe compiled with the C# compiler already inside Windows.**

`C#` · `WPF` · `.NET Framework 4` · `Lucide icons` · `PowerShell` · stato: **in uso**

Un pannello sempre aperto sul bordo dello schermo: le sessioni di Claude Code da riprendere, la lista dei
lavori, gli agenti al lavoro, la batteria di mouse e cuffie, e un ponte con l'iPhone.

## Cosa fa
Sette tessere, ognuna accendibile e spegnibile, in tre misure:
- **Ora**: orologio e data;
- **Riprendi**: l'ultima sessione di Claude Code, riaperta esattamente come la si apre a mano;
- **Sessioni**: le ultime sessioni, una per riga;
- **Agenti**: cosa stanno facendo gli agenti in background;
- **Lavori**: la lista dei lavori, dalla parte "da fare ora";
- **Collegati**: batteria dei dispositivi Bluetooth e USB;
- **iPhone**: il ponte appunti, quello che copi sul telefono finisce negli appunti del PC.

Il vetro è quello vero di Windows dietro la finestra, non un'immagine. Il widget sta sul desktop: non copre le
finestre, non compare in Alt+Tab. Le impostazioni si cambiano dentro il widget.

## Come funziona
Il motore copia l'architettura di WidgetKit di Apple:

| iPhone | qui |
|---|---|
| widget | `Tessera`, una classe in `Tessere.cs` |
| TimelineProvider | il metodo `Dati()` della tessera |
| reload policy | `Scadenza`: ogni tessera ha la sua, in secondi |
| WidgetFamily small/medium/large | `Misura`: piccola, media, grande |
| la vista SwiftUI | il metodo `Vista()`: riceve i dati e disegna |
| galleria dei widget | il pannello impostazioni |

Tre regole: la vista non va mai a cercarsi i dati, li riceve pronti; ogni tessera si rinfresca quando scade la
sua roba, non tutte insieme; la misura decide quanta roba mostrare, non solo quanto è grande.
```
file di Claude Code ─┐
lista dei lavori ────┼─► Dati.cs (filo a parte) ─► Tessere.cs ─► Pelle.cs ─► Finestra.cs
Windows (batterie) ──┘
iPhone ─► Ponte.cs (HTTP, porta 8787) ─► appunti del PC
```

## Struttura
| Percorso | Cosa contiene |
|---|---|
| `programma/Programma.cs` | l'avvio, l'icona nella barra vicino all'orologio |
| `programma/Nucleo.cs` | il motore stile WidgetKit |
| `programma/Tessere.cs` | le sette tessere |
| `programma/Dati.cs` | da dove arrivano i dati |
| `programma/Pelle.cs` | tutti i mattoni grafici: vetro, angoli, colori |
| `programma/Finestra.cs` | la griglia e il pannello impostazioni |
| `programma/Icone.cs` | icone Lucide (licenza ISC), animate |
| `programma/Impostazioni.cs` | impostazioni in JSON dentro AppData, fuori dal progetto |
| `programma/Ponte.cs` | il ponte con l'iPhone |
| `programma/COMPILA.cmd` | compila `WhyWidget.exe` con il `csc` di Windows |
| `WhyWidget.ps1`, `parti/` | la prima versione in PowerShell, ancora funzionante |
| `APRI LA PORTA PER L IPHONE.cmd` | apre la porta 8787 nel firewall |

## Come si avvia
```
programma\COMPILA.cmd      # crea WhyWidget.exe, senza Visual Studio
AVVIA WIDGET.cmd           # oppure la versione PowerShell
```
Serve Windows 10 o 11. Per il ponte, dal telefono si apre `http://IP-DEL-PC:8787` nella rete di casa.

## Stato
In uso tutti i giorni. Da fare: ricompilare l'exe sull'ultima versione, rifinire il design, mettere accanto la
mascotte Aphelios.

## Perché è nato
Riprendere una sessione di Claude Code voleva dire aprire il terminale, cercare l'ID e ricordarsi dove si era
rimasti. Ora è un clic, e tutto quello che serve sapere sta sul bordo dello schermo.

---

Parte di **[WhyEcosystem 2023-2026](https://github.com/OfficialWhyEd/WhyEcosystem-2023-2026)**: il percorso di WhyEd, producer e sound engineer che costruisce sistemi AI dirigendo gli agenti.  
Costruito da WhyEd con Claude Code
