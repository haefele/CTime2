using System;
using System.Globalization;

namespace CTime2.Core.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset RoundDownToFullWeek(this DateTimeOffset self)
        {
            var firstDayOfWeek = DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;

            while (self.DayOfWeek != firstDayOfWeek)
            {
                self = self.AddDays(-1);
            }

            return self;
        }
    }
}