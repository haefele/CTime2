using System;
using JetBrains.Annotations;

namespace CTime2.Core.Common
{
    public class CTimeException : Exception
    {
        public CTimeException([NotNull]string message)
            : base(message)
        {
            Guard.NotNull(message, nameof(message));
        }

        public CTimeException([NotNull]string message, [NotNull]Exception inner)
            : base(message, inner)
        {
            Guard.NotNull(message, nameof(message));
            Guard.NotNull(inner, nameof(inner));
        }
    }
}