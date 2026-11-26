using Mosaique.Core;
using Xunit;

public class GroupBehaviorTests
{
    [Fact]
    public void Le_redimensionnement_interactif_est_limite_a_une_fois_toutes_les_50_ms()
    {
        var cadence = new ResizeCadence();

        Assert.True(cadence.ShouldApply(100));
        Assert.False(cadence.ShouldApply(149));
        Assert.True(cadence.ShouldApply(150));
        Assert.False(cadence.ShouldApply(151));
    }

    [Fact]
    public void Le_relachement_applique_la_taille_finale_meme_avant_50_ms()
    {
        var cadence = new ResizeCadence();

        Assert.True(cadence.ShouldApply(100));
        Assert.False(cadence.ShouldApply(110));
        Assert.True(cadence.ShouldApply(111, final: true));
        Assert.True(cadence.ShouldApply(112)); // Nouveau geste.
    }

    [Theory]
    [InlineData(2, new[] { 2, 0, 1, 3 })]
    [InlineData(-1, new[] { 0, 1, 2, 3 })]
    public void La_fenetre_active_reste_devant_les_autres_sans_changer_leurs_cases(
        int active, int[] expected)
    {
        Assert.Equal(expected, GroupBehavior.FrontToBack(4, active));
    }

    [Theory]
    [InlineData(FrameState.Normal, FrameState.Minimized, GroupAction.Minimize)]
    [InlineData(FrameState.Maximized, FrameState.Minimized, GroupAction.Minimize)]
    [InlineData(FrameState.Minimized, FrameState.Normal, GroupAction.RestoreAndLayout)]
    [InlineData(FrameState.Minimized, FrameState.Maximized, GroupAction.RestoreAndLayout)]
    [InlineData(FrameState.Normal, FrameState.Maximized, GroupAction.Layout)]
    [InlineData(FrameState.Maximized, FrameState.Normal, GroupAction.Layout)]
    [InlineData(FrameState.Normal, FrameState.Normal, GroupAction.None)]
    public void Les_transitions_du_cadre_determinent_quoi_faire_du_groupe(
        FrameState from, FrameState to, GroupAction expected)
    {
        Assert.Equal(expected, GroupBehavior.OnStateChange(from, to));
    }
}
