using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Mosaique.Core;

namespace Mosaique.App
{
    internal sealed class CadreForm : Form
    {
        private const int WmMoving = 0x0216;
        private const int WmSizing = 0x0214;
        private readonly MenuStrip menu = new MenuStrip();
        private readonly List<IntPtr> explorers = new List<IntPtr>(4);
        private readonly ResizeCadence resizeCadence = new ResizeCadence();
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private readonly NativeWindows.ForegroundCallback foregroundCallback;
        private IntPtr foregroundHook;
        private Rectangle[] placements = Array.Empty<Rectangle>();
        private FrameState frameState;
        private bool sizing;

        public CadreForm()
        {
            foregroundCallback = OnForegroundChanged;
            Text = Info.Nom;
            ClientSize = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            menu.Items.Add("Ajouter tous les explorateurs", null, AddExplorers);
            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        private Point GridOrigin => PointToScreen(new Point(0, menu.Bottom));

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            foregroundHook = NativeWindows.WatchForeground(foregroundCallback);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (foregroundHook != IntPtr.Zero)
            {
                NativeWindows.UnhookWinEvent(foregroundHook);
                foregroundHook = IntPtr.Zero;
            }

            base.OnFormClosed(e);
        }

        private void AddExplorers(object sender, EventArgs e)
        {
            explorers.Clear();
            NativeWindows.EnumWindows((window, _) =>
            {
                if (NativeWindows.IsExplorer(window))
                    explorers.Add(window);

                return explorers.Count < 4;
            }, IntPtr.Zero);

            if (explorers.Count == 0)
            {
                MessageBox.Show(this, "Aucune fenêtre de l’Explorateur ouverte.", Info.Nom);
                return;
            }

            foreach (IntPtr window in explorers)
                NativeWindows.RestoreIfNeeded(window);

            LayoutGroup(GridOrigin, ClientSize);
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            bool resize = message.Msg == WmSizing;
            if ((!resize && message.Msg != WmMoving) || explorers.Count == 0 ||
                !NativeWindows.GetWindowRect(Handle, out var current))
                return;

            if (resize)
            {
                sizing = true;
                if (!resizeCadence.ShouldApply(clock.ElapsedMilliseconds))
                    return;
            }

            var proposed = Marshal.PtrToStructure<NativeWindows.Rect>(message.LParam);
            Point origin = GridOrigin;
            origin.Offset(proposed.Left - current.Left, proposed.Top - current.Top);

            if (resize)
            {
                // Convertir la taille extérieure proposée avant que Windows applique la nouvelle taille.
                var client = new Size(
                    ClientSize.Width + (proposed.Right - proposed.Left) - (current.Right - current.Left),
                    ClientSize.Height + (proposed.Bottom - proposed.Top) - (current.Bottom - current.Top));
                LayoutGroup(origin, client);
            }
            else
            {
                PositionGroup(origin, NativeWindows.NoSize);
            }
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            bool finalLayout = sizing && resizeCadence.ShouldApply(clock.ElapsedMilliseconds, final: true);
            sizing = false;
            if (explorers.Count == 0 || WindowState != FormWindowState.Normal)
                return;

            if (finalLayout)
                LayoutGroup(GridOrigin, ClientSize);
            else
                PositionGroup(GridOrigin, NativeWindows.NoSize);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            FrameState nextState = WindowState == FormWindowState.Minimized ? FrameState.Minimized
                : WindowState == FormWindowState.Maximized ? FrameState.Maximized : FrameState.Normal;
            GroupAction action = GroupBehavior.OnStateChange(frameState, nextState);
            frameState = nextState;
            if (action == GroupAction.None || explorers.Count == 0)
                return;

            if (action == GroupAction.Minimize || action == GroupAction.RestoreAndLayout)
            {
                foreach (IntPtr window in explorers)
                    NativeWindows.SetGroupMinimized(window, action == GroupAction.Minimize);
            }

            if (action != GroupAction.Minimize)
                LayoutGroup(GridOrigin, ClientSize);
        }

        private void OnForegroundChanged(IntPtr hook, uint eventType, IntPtr window,
            int objectId, int childId, uint eventThread, uint eventTime)
        {
            if (foregroundHook == hook && WindowState != FormWindowState.Minimized &&
                explorers.Contains(window) && window == NativeWindows.GetForegroundWindow())
                PositionGroup(Point.Empty, NativeWindows.NoMove | NativeWindows.NoSize);
        }

        private void LayoutGroup(Point origin, Size client)
        {
            var area = new Rectangle(0, 0, Math.Max(0, client.Width), Math.Max(0, client.Height - menu.Bottom));
            placements = GridLayout.Calculate(area, explorers.Count);
            for (int index = 0; index < explorers.Count; index++)
                placements[index] = NativeWindows.CompensateBorders(explorers[index], placements[index]);

            PositionGroup(origin, 0);
        }

        private void PositionGroup(Point origin, uint geometryFlags)
        {
            int active = explorers.IndexOf(NativeWindows.GetForegroundWindow());
            IntPtr batch = NativeWindows.BeginDeferWindowPos(explorers.Count + 1);
            IntPtr previous = IntPtr.Zero; // HWND_TOP
            foreach (int index in GroupBehavior.FrontToBack(explorers.Count, active))
            {
                if (batch == IntPtr.Zero)
                    return;

                IntPtr window = explorers[index];
                if (!NativeWindows.IsWindow(window) || NativeWindows.IsIconic(window))
                    continue;

                Rectangle target = placements[index];
                target.Offset(origin);
                batch = NativeWindows.DeferWindowPos(batch, window, previous,
                    target.X, target.Y, target.Width, target.Height, NativeWindows.KeepFocus | geometryFlags);
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
