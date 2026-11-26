using System;
using System.Windows.Forms;
using Mosaique.Core;

namespace Mosaique.App
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CadreForm());
        }
    }

    /// <summary>Cadre du groupe. Tranche 0 : cadre vide ; le suivi des fenêtres arrive en tranche 2.</summary>
    internal sealed class CadreForm : Form
    {
        public CadreForm()
        {
            Text = Info.Nom + " — cadre vide (tranche 0)";
            Width = 1200;
            Height = 800;
            StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
