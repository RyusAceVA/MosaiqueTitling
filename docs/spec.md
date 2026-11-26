# Spécification — Mosaïque flottante v1

Brief validé (porte 1) le 26/11/2026. Groupe valable pendant la session seulement.

## Choix techniques (et pourquoi)
- **C# / WinForms sur .NET Framework 4.8** : déjà présent dans Windows 11, rien à installer
  sur le poste géré (sans droits admin) ; l'expérience a prouvé que ce code s'y exécute.
- **Deux projets** : `Mosaique.Core` (netstandard2.0 : calcul de grille, compensation des
  bordures, règles du groupe, **sans appel Windows**) testable partout, y compris dans
  openbot-dev sous Linux ; `Mosaique.App` (net48, WinForms + appels Win32) construit et
  testé par la CI Windows de GitHub, essayé par toi.
- **Approche « chef d'orchestre »** (validée par l'expérience) : les fenêtres restent celles
  de leurs applications ; le cadre les déplace et rétablit l'ordre d'affichage.

## Parcours principal
1. Tu lances `Mosaique.exe` : un cadre vide apparaît (icône dans la barre des tâches).
2. Menu du cadre « Ajouter une fenêtre » puis clic sur une fenêtre ouverte : elle rejoint la grille.
   Raccourci : « Ajouter tous les explorateurs ouverts » (jusqu'à 4).
3. Grille automatique : 1 = plein cadre, 2 = côte à côte, 3-4 = 2×2.
4. Déplacer le cadre : les fenêtres suivent sans décalage visible.
5. Redimensionner : la grille suit au plus toutes les 50 ms, et s'ajuste exactement au relâchement.
6. Réduire / restaurer / agrandir le cadre : tout le groupe suit.
7. Cliquer dans une fenêtre du groupe : tout le groupe passe devant, le cadre derrière.
8. Fermer le cadre : boîte de choix **« Fermer le groupe et ses fenêtres »** /
   **« Libérer les fenêtres »** (défaut) / Annuler.

## Cas d'échec à gérer
- Une fenêtre du groupe est fermée par son application : sa case disparaît, la grille se recalcule.
- Fenêtre impossible à déplacer (application administrateur) : refus avec message, pas de plantage.
- Une fenêtre agrandie est ajoutée : elle est d'abord restaurée.

## Limites de cette version
- Pas de sauvegarde des groupes, pas d'onglets, un seul écran, 4 fenêtres maximum, un groupe à la fois.

## Exemples d'acceptation
| Entrée / action | Résultat attendu |
|---|---|
| Grille(cadre 1200×800 à (100,100), 4 fenêtres) | cases 600×400 aux positions (100,100) (700,100) (100,500) (700,500) |
| Grille(…, 3 fenêtres) | 2 cases en haut, 1 case pleine largeur en bas |
| Grille(…, 1 fenêtre) | une case = tout le cadre |
| Case 600×400 et bordures invisibles (7,0,7,7) | rectangle demandé à Windows : x−7, y, largeur 614, hauteur 407 |
| Fenêtre fermée dans un groupe de 4 | groupe de 3, grille recalculée |
| Fermeture, choix « Libérer » | aucune fenêtre fermée |
| Fermeture, choix « Fermer tout » | chaque fenêtre reçoit une demande de fermeture normale (elle peut demander « Enregistrer ? ») |
| Essai manuel (toi) | déplacer vite 10 s : note ≥ 4/5 ; redimensionner : note ≥ 3/5 |

Les exemples des lignes 1 à 7 deviennent des tests de `Mosaique.Core` **avant** le code.

## Contradictions relevées par la relecture
- (à remplir par Codex après lecture : rien ne doit contredire le brief)
