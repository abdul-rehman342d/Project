using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Logger;
using Microsoft.Owin.Hosting;

namespace Neeo.Notification.WinSvc
{
    partial class NeeoPNSvc : ServiceBase
    {
        private StartOptions _options = null;
        private IDisposable _server = null;
        private System.Timers.Timer _timer;
        private const string Urls = "urls";

        public NeeoPNSvc()
        {
            InitializeComponent();
            try
            {
                NotificationHandler.GetInstance().StartService();
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                char[] delimeter = { ';' };
                string[] urls = ConfigurationManager.AppSettings[Urls].Split(delimeter);
                _options = new StartOptions();
                foreach (var url in urls)
                {
                    _options.Urls.Add(url);
                }
                _server = WebApp.Start<Startup>(_options);
                //service.Start();
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (_server != null)
                {
                    _server.Dispose();
                }

                NotificationHandler.GetInstance().StopService();
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
