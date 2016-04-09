using System;
using System.IO;
using System.Threading.Tasks;
using MetroLog;
using MetroLog.Targets;

namespace CTime2.Core.Logging
{
    public class LoggerFactory
    {
        private static readonly ILogManager _logManager;

        static LoggerFactory()
        {
            var config = new LoggingConfiguration();
            config.AddTarget(LogLevel.Trace, LogLevel.Fatal, new DebugTarget());

            _logManager = LogManagerFactory.CreateLogManager(config);
        }

        public static Logger GetLogger(Type type)
        {
            return new Logger(_logManager.GetLogger(type));
        }

        public static Logger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public static Task<Stream> GetCompressedLogs()
        {
            return _logManager.GetCompressedLogs();
        }
    }
}