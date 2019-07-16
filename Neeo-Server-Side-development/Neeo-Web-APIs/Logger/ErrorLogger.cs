using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace Logger
{
    /// <summary>
    /// A class that logs only error level details in the error.log file.
    /// </summary>
    internal class ErrorLogger : IErrorLogger
    {
        /// <summary>
        /// Logs the error message along with the specified logger.
        /// </summary>
        /// <param name="type">A variable that specifies type of the logger.</param>
        /// <param name="errorMsg">A string containing the error message for logging.</param>
        /// <param name="methodName">A string containing name of the calling method.(Optional)</param>
        public void LogError(Type type, string errorMsg, string methodName = "")
        {
            ILog log = log4net.LogManager.GetLogger(type);
            log.Error((methodName == "" ? "" : methodName + " ===> ") + errorMsg);
        }

        /// <summary>
        /// Logs the error message and exception details along with the specified logger.
        /// </summary>
        /// <param name="type">A variable that specifies type of the logger.</param>
        /// <param name="errorMsg">A string containing the error message for logging.</param>
        /// <param name="exp">An variable containing "Exception" object for logging.</param>
        /// <param name="methodName">A string containing name of the calling method.(Optional)</param>
        public void LogError(Type type, string errorMsg, Exception exp, string methodName = "")
        {
            ILog log = log4net.LogManager.GetLogger(type);
            log.Error((methodName == "" ? "" : methodName + " ===> ") + errorMsg, exp);
        }
    }
}
