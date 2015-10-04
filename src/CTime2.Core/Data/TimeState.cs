using System;

namespace CTime2.Core.Data
{
    [Flags]
    public enum TimeState
    {
        Entered = 1,
        Left = 2,
        ShortBreak = 4,
        Trip = 8,
        HomeOffice = 16
    }

    public static class TimeStateExtensions
    {
        public static bool IsEntered(this TimeState self)
        {
            return self.HasFlag(TimeState.Entered);
        }
        public static bool IsEntered(this TimeState? self)
        {
            return self != null && self.Value.HasFlag(TimeState.Entered);
        }

        public static bool IsLeft(this TimeState self)
        {
            return self.HasFlag(TimeState.Left);
        }
        public static bool IsLeft(this TimeState? self)
        {
            return self != null && self.Value.HasFlag(TimeState.Left);
        }
    }
}