using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Common;
using Logger;
using Microsoft.Owin.Hosting;

namespace NeeoPushNotificationService
{
    public partial class NeeoPNSvc : ServiceBase
    {
        private StartOptions _options = null;
        private IDisposable _server = null;
        private System.Timers.Timer _timer;
        private const string Urls = "urls";
        private const string TimerInterval = "timerInterval";
        private double _interval;
        

        public NeeoPNSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            _options = new StartOptions();

            try
            {
                char[] delimeter = {';'};
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
            
            _timer = new System.Timers.Timer();
            _server = WebApp.Start<Startup>(_options);
            _timer.Elapsed += TimerOnElapsed;
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
            
            _timer.Interval = _interval <= 0 ? 0 : _interval;
            _timer.Start();
            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Service has been started!!!", System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnStop()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
            _timer.Stop();
            var client = new HttpClient();

            //var task = Task.Run(async () => { await client.GetAsync(UrlWithoutPort + "api/notification/stop"); });
            //task.Wait();
            base.OnStop();
            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Service has been stopped!!!", System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Stop();
            var client = new HttpClient();
            var response = client.GetAsync(_options.Urls[0] + "api/notification/keepalive").Result;
            _timer.Start();
        }
    }
}
