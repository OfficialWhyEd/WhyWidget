# Come lavora WhyEd (leggere PRIMA di qualsiasi lavoro visivo)

Questa repo è di **WhyEd** (Edoardo, @whyed). Il livello richiesto è altissimo. L'ultimo giro è stato giudicato "AI slop completo": non deve succedere più.

## Obbligatorio, sempre
1. **Leggi `REGOLE-QUALITA.md`** (radice della repo): sono le regole usate per WhyChat, il suo progetto migliore.
2. **Lavora come l'agente Designo** (`.claude/agents/designo.md`): per ogni lavoro di UI, grafica o motion, delegalo all'agente `designo` oppure segui tu le sue fasi alla lettera.
3. **Usa TUTTE le skill di design in `.claude/skills/`**, leggendo davvero ogni SKILL.md prima di iniziare:
   - critica e qualità: `impeccable`, `taste-skill`, `gpt-tasteskill`, `design-taste-frontend`, `high-end-visual-design`
   - sistema e riferimenti: `ui-ux-pro-max` (script di ricerca in `scripts/`), `awesome-design-md`, `design-dna`, `skillui`
   - redesign: `redesign-skill`, `redesign-existing-projects`, `minimalist-ui`, `soft-skill`, `frontend-design`
   - motion: `motion-design`, `motion-doctrine`, `gsap-core`, `seam-craft`, `cast`, `paint`
   - video: `hyperframes`, `hyperframes-animation`, `hyperframes-creative`
4. **Riferimenti veri prima di disegnare** (almeno 5 esempi reali di alto livello, cercati sul web) e scritti in un file di note.
5. **Voto onesto** con impeccable e taste-skill **prima** e **dopo** il lavoro, scritto nella pull request. Sotto 8/10 non si consegna: si rifà.
6. **Controllo con gli occhi**: screenshot a 390, 768 e 1440 px, console senza errori, stati limite provati. Allegali alla pull request.

## Font e identità
- Mai font di sistema o "Inter per tutto". Scegli una coppia con carattere (display + testo) e una mono per i numeri, motivata nelle note.
- Un solo accento di colore, neutri tinti, niente `#000`/`#fff` puri.
- Se il progetto ha già un'identità approvata (logo, colori, Aphi la mascotte), si rispetta e si alza di livello, non si butta.

## Regole fisse di WhyEd
- Italiano nei testi e nelle risposte. Niente trattini lunghi. Niente scritte giapponesi. Mai Remotion.
- Audio a 24 bit / 48 kHz, mai degradato.
- Mai ricominciare da zero ciò che è approvato. Mai dire "fatto" senza averlo visto funzionare.
- Consegna sempre su un branch `cloud/...` con pull request: WhyEd e il suo Claude locale la controllano prima di unire.
