# Relais

But — Livrer le calcul de grille et la compensation des bordures de la tranche 1.
État — Mosaique.Core calcule les cases et compense les bordures par arithmétique pure.
Preuve — Les quatre nouveaux tests ont échoué avant l’implémentation, puis les cinq tests ont réussi avec `dotnet test tests/Mosaique.Core.Tests -c Release` sous Linux.
Incertitude — La vérification Windows dépend de la CI. L’intégration à l’application relève de la tranche 2.
Prochaine action — Relire la PR de la tranche 1 et contrôler la CI avant la fusion par le propriétaire.
