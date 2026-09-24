# Regole di qualità WhyEd (UI, UX, motion, video)

Queste sono le regole usate per **WhyChat** (github.com/OfficialWhyEd/whychat, file DESIGN.md e PRODUCT.md) e da **Designo**, l'agente design di WhyEd. Valgono per ogni lavoro visivo di questa repo. Il livello richiesto è altissimo: "deve essere fatto davvero, davvero tanto bene".
Si prende il **metodo**, non la palette di WhyChat: ogni progetto ha la sua identità.

## 1. Principi
- **Estetica = qualità.** Deve sembrare costoso, vivo, curato al pixel. WhyEd nota subito tagli, proporzioni e allineamenti sbagliati.
- **Tutto deve funzionare davvero.** Niente che si rompe scrivendo, ridimensionando, ruotando il telefono, con testi lunghi, con zero dati, in caricamento, in errore. Si provano gli stati limite, non solo lo stato pulito.
- **Il design si giudica usandolo, non dallo screenshot.** Provare il flusso vero, come lo usa lui, su telefono e tablet (li usa più del PC).
- **Onestà.** Mai dire "fatto" senza averlo visto funzionare. Mai voti gonfiati: se è 6/10 si scrive 6/10 e perché.
- **Mai la strada facile.** Il gusto è tutto. Se una soluzione è banale o "da template", va rifatta.
- **Mai ricominciare da zero** ciò che è già fatto e approvato: si migliora sopra.
- **Vivo, mai statico morto.** Micro-movimento dove ha senso, ma sempre con uno scopo.

## 2. Anti-slop (cosa NON deve sembrare)
- Niente look "l'ha fatto un'AI": griglie di 3 card uguali, hero con metrica gigante, gradient-text sui titoli, viola/neon da AI, arcobaleno, glow neon, emoji illustrative, bordo colorato laterale come accento, card dentro card.
- Niente cartoon o giocattolo quando serve professionale.
- Niente `#000` puro e niente `#fff` puro: neri e bianchi sempre leggermente tinti.
- Niente testi da marketing ("Elevate", "Seamless", "Next-Gen", "Unlock").
- Niente scritte o simboli giapponesi. Niente trattini lunghi (il carattere "—") nei testi.

## 3. Colore
- **Una strategia sola**: superficie neutra (tinta verso la temperatura del brand) + **un accento** principale.
- L'accento occupa al massimo il 15% circa della superficie: bottoni primari, stati attivi, momenti chiave.
- Per dati con più categorie, usare una rampa **dentro la stessa famiglia** di colore, non colori a caso.
- Contrasto testo AA minimo (4.5:1 per il testo normale).
- Colori come token (CSS custom properties), mai valori sparsi nel codice.

## 4. Tipografia
- Massimo 2 o 3 famiglie: una per l'interfaccia, una display con carattere, una mono per numeri e label tecniche.
- Gerarchia fatta con scala e peso, rapporto di almeno 1.25 fra un livello e l'altro.
- **Numeri (saldi, contatori, vincite, timer) sempre in mono o tabular-nums**, così non ballano.
- Label tecniche: maiuscolo piccolo, spaziatura larga.

## 5. Layout e componenti
- Liste editoriali con linee sottili (hairline) invece di griglie di card, dove funziona.
- Le card solo se l'elevazione comunica gerarchia.
- Spaziature da una scala fissa (4/8 px). Allineamenti precisi: tutto su una griglia.
- Bersagli touch di almeno 44x44 px. Pensato **prima per telefono**, poi tablet, poi desktop.
- Più contenuto utile a schermo, meno vuoto decorativo.

## 6. Materiali
- Vetro (glass) solo scuro e **solido** (sfondo semi-opaco + blur + bordo sottile). Mai vetro WebGL che campiona lo sfondo sugli input: si rompe al resize.
- Bordi hairline; ombre tinte verso il colore del brand, mai nere piatte.

## 7. Motion
- Entrate con ease-out esponenziale `cubic-bezier(0.22, 1, 0.36, 1)`, uscite con ease-in. **Niente bounce o elastic** nell'interfaccia.
- Spring (rigidità circa 480, smorzamento circa 34) per liste e accenti che si spostano.
- Animare **solo transform e opacity**, mai proprietà di layout (width, top, margin...).
- Durate: micro 120-200 ms, transizioni 250-400 ms, momenti grandi (vincita, drop, bonus) fino a 1-2 s con coreografia a più tempi.
- Principi Disney usati con misura: anticipazione, follow-through, sovrapposizione, stagger.
- Rispettare `prefers-reduced-motion`.
- Librerie: **GSAP** per il motion complesso, **Three.js** solo per il 3D. Mai Remotion.

## 8. Video e spot
- Ogni taglio cade su un colpo vero della musica (cassa, rullante, drop), non solo sulla griglia teorica.
- Gancio nei primi 2 secondi; una storia chiara: cosa è, perché è figo, invito finale.
- Pochi testi, grandi, leggibili sul telefono; mai sopra scritte già presenti nel materiale.
- Audio a 24 bit / 48 kHz, mai degradato prima del master finale.

## 9. Come si lavora (il ciclo di qualità)
1. **Riferimenti veri prima di disegnare**: 5 o più esempi reali di alto livello (prodotti, siti, spot), analizzati. Si scrive cosa fanno meglio di noi.
2. **Piano scritto**: cosa cambia e perché.
3. **Costruzione.**
4. **Controllo con gli occhi**: screenshot a 390 px (telefono), 768 px (tablet) e 1440 px (desktop); console del browser senza errori; stati limite provati.
5. **Voto onesto da 1 a 10** su: gerarchia, colore, tipografia, spaziatura, motion, funzionamento, "sembra fatto da un'AI?". Sotto 8 si torna al punto 3.
6. **Consegna**: pull request con gli screenshot o il video, il voto e cosa resta da migliorare.

## 10. Regole di WhyEd per i testi
- Testi dell'interfaccia in italiano (o inglese se è il tono del progetto), frasi corte.
- Niente trattini lunghi. Niente scritte giapponesi.
