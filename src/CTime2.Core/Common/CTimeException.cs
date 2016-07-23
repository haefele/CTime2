using System;
using UwCore.Common;

namespace CTime2.Core.Common
{
    public class CTimeException : Exception
    {
        public CTimeException(string message)
            : base(message)
        {
            Guard.NotNull(message, nameof(message));
        }

        public CTimeException(string message, Exception inner)
            : base(message, inner)
        {
            Guard.NotNull(message, nameof(message));
            Guard.NotNull(inner, nameof(inner));
        }
    }
}