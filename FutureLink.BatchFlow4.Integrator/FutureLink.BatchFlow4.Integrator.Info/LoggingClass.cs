using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace FutureLink.BatchFlow4.Integrator.Info
{
    public class LoggingClass
    {
        private NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public LoggingClass(string loggerName)
        {
            setName(loggerName);
        }



        public void setName(string loggerName)
        {
            logger = LogManager.GetLogger(loggerName);
        }

        #region Log trace message
        public void Trace(string message, string componentName, string methodName)
        {
            logger.Trace(FormatLogEntry(message, "", componentName, methodName));
        }

        #endregion

        public void Debug(string message, string componentName, string methodName)
        {
            logger.Debug(FormatLogEntry(message, "", componentName, methodName));
        }

        public void Info(string message, string componentName, string methodName)
        {
            logger.Info(FormatLogEntry(message, "", componentName, methodName));
        }

        public void Warn(string message, string componentName, string methodName)
        {
            logger.Warn(FormatLogEntry(message, "", componentName, methodName));
        }

        public void Error(string message, string componentName, string methodName)
        {
            logger.Error(FormatLogEntry(message, "", componentName, methodName));
        }

        public void Fatal(string message, string componentName, string methodName)
        {
            logger.Fatal(FormatLogEntry(message, "", componentName, methodName));
        }

        public void Fatal(string message, string componentName, string methodName, Exception ex)
        {
            logger.Fatal(FormatLogEntry(message, ex.StackTrace, componentName, methodName));
        }

        private string FormatLogEntry(
               string message,
               string stackTrace,
               string componentName, string methodName)
        {
            StringBuilder logEntry = new StringBuilder();
            logEntry.Append("<LOGENTRY>");
            logEntry.AppendFormat("<TIMESTAMP>{0}</TIMESTAMP>", DateTime.Now);
            logEntry.AppendFormat("<COMPONENT>{0}</COMPONENT>", componentName);
            logEntry.AppendFormat("<METHOD>{0}</METHOD>", methodName);
            logEntry.AppendFormat("<MSG>{0}</MSG>", message);
            logEntry.AppendFormat("<STACKTRACE>{0}</STACKTRACE></LOGENTRY>", stackTrace);
            logEntry.AppendLine();

            return logEntry.ToString();
        }

    }
}
