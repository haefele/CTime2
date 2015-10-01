using System;

namespace CTime2.Core.Logging
{
    public class LoggerFactoryAttribute : Attribute
    {
        public Type LoggerFactoryType { get; }

        public LoggerFactoryAttribute(Type loggerFactoryType)
        {
            this.LoggerFactoryType = loggerFactoryType;
        }
    }
}