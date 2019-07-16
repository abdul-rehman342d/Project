using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    /// <summary>
    /// 
    /// </summary>
    public interface IErrorLogger
    {
        /// <summary>
        /// Logs the error message along with the specified logger.
        /// </summary>
        /// <param name="type">A variable that specifies type of the logger.</param>
        /// <param name="errorMsg">A string containing the error message for logging.</param>
        /// <param name="methodName">A string containing name of the calling method.(Optional)</param>
        void LogError(Type type, string errorMsg, string methodName = "");

        /// <summary>
        /// Logs the error message and exception details along with the specified logger.
        /// </summary>
        /// <param name="type">A variable that specifies type of the logger.</param>
        /// <param name="errorMsg">A string containing the error message for logging.</param>
        /// <param name="exp">An variable containing "Exception" object for logging.</param>
        /// <param name="methodName">A string containing name of the calling method.(Optional)</param>
        void LogError(Type type, string errorMsg, Exception exp, string methodName = "");
    }
}
