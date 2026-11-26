using System;
using System.Windows.Forms;

namespace Mosaique.App
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            NativeWindows.SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CadreForm());
        }
    }
}
