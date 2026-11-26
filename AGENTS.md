# Consignes des agents — MosaiqueTiling

## Commandes
- Tester la logique (Linux ou Windows, .NET 8 SDK) : `dotnet test tests/Mosaique.Core.Tests -c Release`
- Construire l'application (Windows) : `dotnet build src/Mosaique.App -c Release`
- Vérifier avant toute PR : la commande de test ci-dessus ; le build de l'App est vérifié par la CI Windows.

## Architecture
- `src/Mosaique.Core` (netstandard2.0) : calculs purs (grille, bordures, règles du groupe). **Aucun appel Windows ici**, tout y est testé.
- `src/Mosaique.App` (net48, WinForms) : cadre, appels Win32 (`DeferWindowPos`, `DwmGetWindowAttribute`…), fine couche au-dessus de Core.
- .NET Framework 4.8 exprès : déjà présent sur Windows 11, aucune installation sur le poste (sans droits admin).

## Règles
- Branche `openbot/<sujet>` ; une PR par tranche de `docs/plan.md` ; la fusion revient au propriétaire.
- Les exemples d'acceptation de `docs/spec.md` deviennent des tests **avant** le code ; ne pas les modifier (sinon le dire en 1re ligne de PR).
- Mettre à jour `docs/relais.md` (5 lignes) avant de s'arrêter.
- Ne pas modifier `.github/workflows/` : la CI appartient au propriétaire.

## Pièges connus (appris pendant l'expérience)
- Windows 11 ajoute une bordure **invisible** (~7 px) autour des fenêtres : compenser avec `DWMWA_EXTENDED_FRAME_BOUNDS` (9).
- Quand on attrape le cadre, Windows le met au premier plan : rétablir l'ordre (fenêtres devant, cadre derrière) à chaque mouvement.
- Explorateurs : les repérer par la classe `CabinetWClass` (les onglets Windows 11 partagent une fenêtre).
- L'événement `Move` n'est pas fiable pendant le glisser : suivre la position (minuteur ou `WM_MOVING`).
- Redimensionner une fenêtre de l'Explorateur la fait redessiner : limiter la fréquence (~50 ms) et ajuster au relâchement.
