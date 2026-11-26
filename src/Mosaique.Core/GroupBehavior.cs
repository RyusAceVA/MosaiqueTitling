namespace Mosaique.Core
{
    public enum FrameState
    {
        Normal,
        Minimized,
        Maximized
    }

    public enum GroupAction
    {
        None,
        Minimize,
        RestoreAndLayout,
        Layout
    }

    public static class GroupBehavior
    {
        public static int[] FrontToBack(int count, int active)
        {
            var order = new int[count];
            int next = 0;

            if (active >= 0 && active < count)
                order[next++] = active;

            for (int index = 0; index < count; index++)
            {
                if (index != active)
                    order[next++] = index;
            }

            return order;
        }

        public static GroupAction OnStateChange(FrameState from, FrameState to)
        {
            if (from == to)
                return GroupAction.None;
            if (to == FrameState.Minimized)
                return GroupAction.Minimize;
            if (from == FrameState.Minimized)
                return GroupAction.RestoreAndLayout;

            return GroupAction.Layout;
        }
    }

    public sealed class ResizeCadence
    {
        private const int IntervalMilliseconds = 50;
        private long? lastApplied;

        public bool ShouldApply(long milliseconds, bool final = false)
        {
            if (final)
            {
                lastApplied = null;
                return true;
            }

            if (lastApplied.HasValue && milliseconds - lastApplied.Value < IntervalMilliseconds)
                return false;

            lastApplied = milliseconds;
            return true;
        }
    }
}
