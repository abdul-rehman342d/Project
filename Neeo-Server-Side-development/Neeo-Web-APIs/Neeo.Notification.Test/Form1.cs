using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logger;
using Microsoft.Owin.Hosting;
using NeeoPushNotificationService;

namespace NeeoPushNotificationService.Notification.Test
{
    public partial class Form1 : Form
    {
        private StartOptions _options = null;
        private IDisposable _server = null;
        private System.Timers.Timer _timer;
        private const string Urls = "urls";
        private const string TimerInterval = "timerInterval";
        private double _interval;

        public Form1()
        {
            InitializeComponent();

            _options = new StartOptions();

            try
            {
                char[] delimeter = { ';' };
                string[] urls = ConfigurationManager.AppSettings[Urls].Split(delimeter);
                foreach (var url in urls)
                {
                    _options.Urls.Add(url);
                }
            }
            catch (ConfigurationErrorsException exception)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw;
            }

            _server = WebApp.Start<Startup>(_options);

            try
            {
                _interval = Convert.ToDouble(ConfigurationManager.AppSettings[TimerInterval]);
            }
            catch (FormatException exception)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (OverflowException exception)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Service has been started!!!", System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
    }
}
