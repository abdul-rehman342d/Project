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
    public interface IInfoLogger
    {
        /// <summary>
        /// Logs the message along with the specified logger.
        /// </summary>
        /// <param name="type">A variable that specifies type of the logger.</param>
        /// <param name="msg">A string containing the message for logging</param>
        /// <param name="methodName">A string containing name of the calling method.(Optional)</param>
        void LogInfo(Type type, string msg, string methodName = "");

        /// <summary>
        /// Logs the message and exception details along with the specified logger.
        /// </summary>
        /// <param name="type">A variable that specifies type of the logger.</param>
        /// <param name="msg">A string containing the message for logging.</param>
        /// <param name="exp">An variable containing "Exception" object for logging.</param>
        /// <param name="methodName">A string containing name of the calling method.(Optional)</param>
        void LogInfo(Type type, string msg, Exception exp, string methodName = "");
    }
}
