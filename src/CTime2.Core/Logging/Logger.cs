using System;
using MetroLog;

namespace CTime2.Core.Logging
{
    public class Logger
    {
        private readonly ILogger _logger;

        public Logger(ILogger logger)
        {
            this._logger = logger;
        }

        public void Debug(string format, params object[] args) => this._logger.Debug(format, args);
        public void Debug(Exception exception, string format, params object[] args) => this._logger.Debug(string.Format(format, args), exception);
        public bool IsDebugEnabled => this._logger.IsDebugEnabled;
        public void Information(string format, params object[] args) => this._logger.Info(format, args);
        public void Information(Exception exception, string format, params object[] args) => this._logger.Info(string.Format(format, args), exception);
        public bool IsInformationEnabled => this._logger.IsInfoEnabled;
        public void Warning(string format, params object[] args) => this._logger.Warn(format, args);
        public void Warning(Exception exception, string format, params object[] args) => this._logger.Warn(string.Format(format, args), exception);
        public bool IsWarningEnabled => this._logger.IsWarnEnabled;
        public void Error(string format, params object[] args) => this._logger.Error(format, args);
        public void Error(Exception exception, string format, params object[] args) => this._logger.Error(string.Format(format, args), exception);
        public bool IsErrorEnabled => this._logger.IsErrorEnabled;
        public void Fatal(string format, params object[] args) => this._logger.Fatal(format, args);
        public void Fatal(Exception exception, string format, params object[] args) => this._logger.Fatal(string.Format(format, args), exception);
        public bool IsFatalEnabled => this._logger.IsFatalEnabled;
    }
}