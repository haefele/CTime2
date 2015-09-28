using System;
using System.Runtime.Serialization;

namespace CTime2.Core.Common
{
    public class CTimeException : Exception
    {
        public CTimeException()
        {
        }

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