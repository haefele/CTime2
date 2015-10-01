using MetroLog;

namespace CTime2.Core.Logging
{
    public class LoggerFactory
    {
        static LoggerFactory()
        {
            //TODO: Configure MetroLog
        }

        public static Logger GetLogger<T>()
        {
            return new Logger(LogManagerFactory.DefaultLogManager.GetLogger<T>());
        }
    }
}