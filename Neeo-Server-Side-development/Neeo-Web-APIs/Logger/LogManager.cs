using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly : log4net.Config.XmlConfigurator(Watch = true)]
namespace Logger
{
    /// <summary>
    /// A class containing different loggers for loging information.
    /// </summary>
    public sealed class LogManager
    {

        private static readonly LogManager _logManager = new LogManager();
        private readonly IInfoLogger _infoLogger;
        private readonly IErrorLogger _errorLogger;

        private LogManager()
        {
            _infoLogger = new InfoLogger();
            _errorLogger = new ErrorLogger();
        }

        /// <summary>
        /// Contains the current instance of the LogManager for logging.
        /// </summary>
        public static LogManager CurrentInstance
        {
            get
            {
                return _logManager;
            }
        }

        /// <summary>
        /// Contains the InfoLogger object for logging information.
        /// </summary>
        public IInfoLogger InfoLogger
        {
            get
            {
                return _infoLogger;
            }
        }

        /// <summary>
        /// Contains ErrorLogger object for logging errors.
        /// </summary>
        public IErrorLogger ErrorLogger
        {
            get
            {
                return _errorLogger;
            }
        }
    }
}
