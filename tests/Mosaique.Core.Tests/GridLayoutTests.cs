using System.Drawing;
using Mosaique.Core;
using Xunit;

public class GridLayoutTests
{
    private static readonly Rectangle Cadre = new Rectangle(100, 100, 1200, 800);

    [Fact]
    public void Quatre_fenetres_occupent_une_grille_de_deux_par_deux()
    {
        Assert.Equal(new[]
        {
            new Rectangle(100, 100, 600, 400),
            new Rectangle(700, 100, 600, 400),
            new Rectangle(100, 500, 600, 400),
            new Rectangle(700, 500, 600, 400)
        }, GridLayout.Calculate(Cadre, 4));
    }

    [Fact]
    public void Trois_fenetres_ont_deux_cases_en_haut_et_une_case_pleine_largeur_en_bas()
    {
        Assert.Equal(new[]
        {
            new Rectangle(100, 100, 600, 400),
            new Rectangle(700, 100, 600, 400),
            new Rectangle(100, 500, 1200, 400)
        }, GridLayout.Calculate(Cadre, 3));
    }

    [Fact]
    public void Deux_fenetres_occupent_deux_cases_cote_a_cote()
    {
        Assert.Equal(new[]
        {
            new Rectangle(100, 100, 600, 800),
            new Rectangle(700, 100, 600, 800)
        }, GridLayout.Calculate(Cadre, 2));
    }

    [Fact]
    public void Une_fenetre_occupe_tout_le_cadre()
    {
        Assert.Equal(new[] { Cadre }, GridLayout.Calculate(Cadre, 1));
    }

    [Fact]
    public void Les_bordures_invisibles_agrandissent_le_rectangle_demande()
    {
        var cellule = new Rectangle(100, 100, 600, 400);

        Assert.Equal(new Rectangle(93, 100, 614, 407),
            GridLayout.CompensateBorders(cellule, left: 7, top: 0, right: 7, bottom: 7));
    }
}
