using System.Collections.Generic;

namespace CTime2.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> self)
        {
            if (self == null)
                return new T[0];

            return self;
        }
    }
}