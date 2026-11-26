# Plan — Mosaïque flottante v1

Chaque tranche se termine par une PR, vérifiée par la CI, relue par Claude, fusionnée par toi.

| # | Tranche | Vérification (pass/échec) | Qui |
|---|---|---|---|
| 0 | Modèle de projet + CI Windows (build net48 + tests) | CI verte sur une solution vide | Claude (la CI appartient au propriétaire) |
| 1 | `Mosaique.Core` : calcul de grille + compensation des bordures + tests d'acceptation 1 à 4 | `dotnet test` vert (Linux et CI) | Codex |
| 2 | `Mosaique.App` : cadre + « Ajouter tous les explorateurs » + suivi au déplacement + ordre d'affichage | CI verte + exe téléchargeable + ton essai : déplacement ≥ 4/5 | Codex |
| 3 | Redimensionnement fluide (50 ms + ajustement au relâchement) ; réduire/restaurer/agrandir | CI + ton essai : redimensionnement ≥ 3/5 | Codex |
| 4 | Fenêtre fermée → grille recalculée ; boîte de fermeture (tests 5 à 7) | CI + ton essai des 3 choix | Codex |
| 5 | « Ajouter une fenêtre » par clic (toute application) ; refus propre des fenêtres admin | CI + ton essai avec Explorateur, navigateur, VS Code | Codex |

Coût CI : les minutes Windows comptent double sur un dépôt privé gratuit → CI courte
(un build Release + tests), pas à chaque push de brouillon.

Porte 2 — plan approuvé par le propriétaire le : 26/11/2026
