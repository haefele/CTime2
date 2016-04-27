using System;
using JetBrains.Annotations;

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
        public static bool IsEntered([CanBeNull]this TimeState? self)
        {
            return self != null && self.Value.IsEntered();
        }

        public static bool IsLeft(this TimeState self)
        {
            return self.HasFlag(TimeState.Left);
        }

        public static bool IsLeft([CanBeNull]this TimeState? self)
        {
            return self != null && self.Value.IsLeft();
        }

        public static bool IsTrip(this TimeState self)
        {
            return self.HasFlag(TimeState.Trip);
        }

        public static bool IsTrip([CanBeNull]this TimeState? self)
        {
            return self != null && self.Value.IsTrip();
        }

        public static bool IsHomeOffice(this TimeState self)
        {
            return self.HasFlag(TimeState.HomeOffice);
        }

        public static bool IsHomeOffice([CanBeNull]this TimeState? self)
        {
            return self != null && self.Value.IsHomeOffice();
        }
    }
}