using System;

namespace CTime2.Extensions
{
    public static class TimeSpanExtensions
    {
        public static DateTime ToDateTime(this TimeSpan self)
        {
            return new DateTime(1, 1, 1).Add(self);
        }
    }
}