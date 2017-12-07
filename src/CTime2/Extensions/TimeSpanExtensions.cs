using System;

namespace CTime2.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToFormattedString(this TimeSpan self)
        {
            bool isNegative = Math.Sign(self.TotalHours) == -1;
            int hours = (int)Math.Abs(self.TotalHours);
            int minutes = (int)Math.Abs(self.Minutes);

            string negativePrefix = isNegative ? "-" : string.Empty;
            string hoursPart = hours > 0 ? $"{hours} h" : string.Empty;
            string minutesPart = $"{minutes} min";

            return $"{negativePrefix} {hoursPart} {minutesPart}".Trim();
        }
    }
}