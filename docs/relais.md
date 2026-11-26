# Relais

But — Adapter le groupe au redimensionnement et aux changements d’état du cadre, et le faire passer devant au clic.
État — La grille est recalculée au plus toutes les 50 ms pendant le geste, puis à la taille finale. L’activation d’un membre remet le cadre derrière le groupe.
Preuve — Les 11 nouveaux cas échouent avant l’implémentation. Ensuite, les 17 tests Core réussissent et Mosaique.exe compile en Release.
Incertitude — La fluidité, la géométrie finale, les transitions et l’ordre réel des fenêtres restent à essayer sur Windows 11.
Prochaine action — Relire la PR, contrôler la CI et essayer le redimensionnement, le clic, la réduction, la restauration et l’agrandissement.
