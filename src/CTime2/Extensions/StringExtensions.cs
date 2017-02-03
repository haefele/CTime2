using System;

namespace CTime2.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string self, string value, StringComparison comparison)
        {
            return self.IndexOf(value, comparison) >= 0;
        }
    }
}