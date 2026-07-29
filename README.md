# SonrisaTask
## Repo for AI agent homework


## TODO or TO-CONSIDER:
A beszélt nyelv rugalmas és szubjektív!
Az AI válaszok pontossága a feltett kérdés finomításával, pontosításával minél körültekintőbb megfogalmazásával, minél több szóba jöhető részlet tisztázásával növelhető.
Ezért a: 

    "User want to be able to set up alerts so they get notified when something important happens in the world"
     
kérés találati pontossága növelhető:
- az általános szematikai temperature állításával (0.0 vagy 0.2) minél alacsonayabb anál tényszerűbb
- egyes modellek specifikus paramétereinek állításával
- A feltett kérdés (vagy inkább legyen az egy kívánt értesítés értesítési feltétel)-t jól-bőségesen leíró szöveg részlet szematikai vektorának (embedding során képzett rész eredmény ami determinisztikus az eredeti szöveggel) és az aktuális találatok (hírek cikkek) szematikai vektorának hasonlítása (koszinusz hasonlóság az egyhez közeli érték nagy fokú egyezést jelent). Ez nagyságrendekkel kisebb teljesítmény ígényű mint egy teljes AI válasz generálása. A pozitív értesítési esetekre ezután érdemes csak átfogalmazás tömör lényeg kiemelés jellegel AI feldolgozást kérni.



