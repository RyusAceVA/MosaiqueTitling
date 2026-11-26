using Mosaique.Core;
using Xunit;

public class InfoTests
{
    // Test de fumée de la tranche 0 : prouve que la chaîne de test tourne.
    // Les tests d'acceptation (docs/spec.md) arrivent en tranche 1.
    [Fact]
    public void Le_nom_est_defini() => Assert.False(string.IsNullOrEmpty(Info.Nom));
}
