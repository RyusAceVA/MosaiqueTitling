using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Mosaique.Core;

namespace Mosaique.App
{
    internal static class NativeWindows
    {
        internal const uint NoSize = 0x0001;
        internal const uint NoMove = 0x0002;
        internal const uint KeepFocus = 0x0010 | 0x0200; // SWP_NOACTIVATE | SWP_NOOWNERZORDER
        private const int ExtendedFrameBounds = 9;
        private const int Restore = 9;
        private const int ShowMinimizedWithoutActivation = 7;
        private const int ShowWithoutActivation = 4;
        private const uint ForegroundEvent = 0x0003;

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            public int Left, Top, Right, Bottom;
        }

        internal static bool IsExplorer(IntPtr window)
        {
            if (!IsWindowVisible(window))
                return false;

            var className = new StringBuilder(256);
            return GetClassName(window, className, className.Capacity) != 0 &&
                className.ToString() == "CabinetWClass";
        }

        internal static void RestoreIfNeeded(IntPtr window)
        {
            if (IsIconic(window) || IsZoomed(window))
                ShowWindow(window, Restore);
        }

        internal static void SetGroupMinimized(IntPtr window, bool minimize)
        {
            if (IsWindow(window) && IsIconic(window) != minimize)
                ShowWindow(window, minimize ? ShowMinimizedWithoutActivation : ShowWithoutActivation);
        }

        internal static IntPtr WatchForeground(ForegroundCallback callback) =>
            SetWinEventHook(ForegroundEvent, ForegroundEvent, IntPtr.Zero, callback, 0, 0, 0);

        internal delegate void ForegroundCallback(IntPtr hook, uint eventType, IntPtr window,
            int objectId, int childId, uint eventThread, uint eventTime);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr module,
            ForegroundCallback callback, uint processId, uint threadId, uint flags);

        [DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr hook);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        internal static Rectangle CompensateBorders(IntPtr window, Rectangle cell)
        {
            if (!GetWindowRect(window, out var outer) ||
                DwmGetWindowAttribute(window, ExtendedFrameBounds, out var visible, Marshal.SizeOf<Rect>()) != 0)
                return cell;

            return GridLayout.CompensateBorders(cell, visible.Left - outer.Left,
                visible.Top - outer.Top, outer.Right - visible.Right, outer.Bottom - visible.Bottom);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        internal delegate bool WindowCallback(IntPtr window, IntPtr parameter);

        [DllImport("user32.dll")]
        internal static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(WindowCallback callback, IntPtr parameter);

        [DllImport("user32.dll")]
        internal static extern bool IsWindow(IntPtr window);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr window);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr window, StringBuilder className, int capacity);

        [DllImport("user32.dll")]
        internal static extern bool IsIconic(IntPtr window);

        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr window);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr window, int command);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr window, out Rect bounds);

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr window, int attribute, out Rect value, int size);

        [DllImport("user32.dll")]
        internal static extern IntPtr BeginDeferWindowPos(int count);

        [DllImport("user32.dll")]
        internal static extern IntPtr DeferWindowPos(IntPtr batch, IntPtr window, IntPtr insertAfter,
            int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        internal static extern bool EndDeferWindowPos(IntPtr batch);
    }
}
