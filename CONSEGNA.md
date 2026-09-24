# Consegna: WhyWidget, exe vero e design rifinito (#53)

## Cosa
WhyWidget è un widget per il desktop di Windows fatto come i widget dell'iPhone: tessere di misure diverse, dati che scadono, scegli tu cosa mostrare. Oggi gira come script: WhyEd vuole **un exe vero** che parte con Windows, e un design all'altezza.

## Da fare
1. Leggi la repo e scrivi in `docs/STATO.md` come parte oggi e cosa serve.
2. **Build dell'exe con GitHub Actions** su `windows-latest` (tu lavori su Linux: la build vera la fa il workflow). Scegli la strada più solida per la tecnologia già usata (es. PyInstaller, pkg, Tauri) e motivala. Il workflow produce l'exe come artifact e, con un tag `v*`, una release. Verifica che il workflow passi.
3. Avvio con Windows opzionale (voce nel menu), una sola istanza alla volta, icona nella tray, uscita pulita.
4. **Design**: segui `CLAUDE-QUALITA.md` e `REGOLE-QUALITA.md`. Riferimenti veri: widget di iOS 18 e macOS, i migliori skin Rainmeter, Widgets di Windows 11. Vetro solido scuro, angoli e ombre coerenti, numeri in mono. Screenshot di ogni misura di tessera nella PR con voto onesto.
5. README in inglese (repo pubblica): cosa fa, screenshot, scarica ed esegui.

## Consegna
Branch `cloud/exe`, PR verso `main`. Scrivi come WhyEd prova l'exe sul PC (un solo passo).
