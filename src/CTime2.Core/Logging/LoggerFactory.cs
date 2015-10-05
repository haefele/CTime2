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
            config.AddTarget(LogLevel.Trace, LogLevel.Fatal, new FileStreamingTarget());

            _logManager = LogManagerFactory.CreateLogManager(config);
        }

        public static Logger GetLogger<T>()
        {
            return new Logger(_logManager.GetLogger<T>());
        }

        public static Task<Stream> GetCompressedLogs()
        {
            return _logManager.GetCompressedLogs();
        }
    }
}