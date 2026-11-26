using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Mosaique.Core;

namespace Mosaique.App
{
    internal sealed class CadreForm : Form
    {
        private const int WmMoving = 0x0216;
        private readonly MenuStrip menu = new MenuStrip();
        private readonly List<IntPtr> explorers = new List<IntPtr>(4);
        private Rectangle[] placements = Array.Empty<Rectangle>();

        public CadreForm()
        {
            Text = Info.Nom;
            ClientSize = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            menu.Items.Add("Ajouter tous les explorateurs", null, AddExplorers);
            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        private Point GridOrigin => PointToScreen(new Point(0, menu.Bottom));

        private void AddExplorers(object sender, EventArgs e)
        {
            explorers.Clear();
            NativeWindows.EnumWindows((window, _) =>
            {
                if (NativeWindows.IsExplorer(window))
                    explorers.Add(window);

                return explorers.Count < 4;
            }, IntPtr.Zero);

            var area = new Rectangle(0, 0, ClientSize.Width, Math.Max(0, ClientSize.Height - menu.Bottom));
            placements = GridLayout.Calculate(area, explorers.Count);
            if (explorers.Count == 0)
            {
                MessageBox.Show(this, "Aucune fenêtre de l’Explorateur ouverte.", Info.Nom);
                return;
            }

            for (int index = 0; index < explorers.Count; index++)
            {
                IntPtr window = explorers[index];
                NativeWindows.RestoreIfNeeded(window);
                placements[index] = NativeWindows.CompensateBorders(window, placements[index]);
            }

            PositionGroup(GridOrigin, resize: true);
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (message.Msg == WmMoving && explorers.Count > 0 &&
                NativeWindows.GetWindowRect(Handle, out var current))
            {
                var proposed = Marshal.PtrToStructure<NativeWindows.Rect>(message.LParam);
                Point origin = GridOrigin;
                origin.Offset(proposed.Left - current.Left, proposed.Top - current.Top);
                PositionGroup(origin, resize: false);
            }
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            if (explorers.Count > 0 && WindowState == FormWindowState.Normal)
                PositionGroup(GridOrigin, resize: false);
        }

        private void PositionGroup(Point origin, bool resize)
        {
            IntPtr batch = NativeWindows.BeginDeferWindowPos(explorers.Count + 1);
            IntPtr previous = IntPtr.Zero; // HWND_TOP
            for (int index = 0; batch != IntPtr.Zero && index < explorers.Count; index++)
            {
                IntPtr window = explorers[index];
                if (!NativeWindows.IsWindow(window) || NativeWindows.IsIconic(window))
                    continue;

                Rectangle target = placements[index];
                target.Offset(origin);
                uint flags = NativeWindows.KeepFocus | (resize ? 0u : NativeWindows.NoSize);
                batch = NativeWindows.DeferWindowPos(batch, window, previous,
                    target.X, target.Y, target.Width, target.Height, flags);
                previous = window;
            }

            if (batch != IntPtr.Zero && previous != IntPtr.Zero)
                batch = NativeWindows.DeferWindowPos(batch, Handle, previous, 0, 0, 0, 0,
                    NativeWindows.KeepFocus | NativeWindows.NoMove | NativeWindows.NoSize);

            if (batch != IntPtr.Zero)
                NativeWindows.EndDeferWindowPos(batch);
        }
    }
}
