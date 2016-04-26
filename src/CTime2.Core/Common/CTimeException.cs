using System;

namespace CTime2.Core.Common
{
    public class CTimeException : Exception
    {
        public CTimeException(string message)
            : base(message)
        {
        }

        public CTimeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}