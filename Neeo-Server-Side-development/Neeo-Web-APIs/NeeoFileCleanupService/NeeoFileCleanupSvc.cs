using System;
using System.ServiceProcess;
using System.Configuration;
using System.Threading;
using LibNeeo.SharedMedia;
using Logger;

namespace NeeoFileCleanupService
{
    public partial class NeeoFileCleanupSvc : ServiceBase
    {
        private System.Threading.Timer _eventTimer;

        public NeeoFileCleanupSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            _eventTimer = new System.Threading.Timer(Callback);
            UpdateTimeInterval();
        }

        private void Callback(object state)
        {
            LogManager.CurrentInstance.InfoLogger.LogInfo(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                "Callback fired at " + DateTime.UtcNow.ToString());
                       
            try
            {
                SharedMedia.DeleteExpiredFiles();
            }
            catch (ApplicationException applicationException)
            {
                //already logged.
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
            }
            UpdateTimeInterval();
        }

        private void UpdateTimeInterval()
        {
            const string triggeringTimeHours = "triggeringTimeHours";
            const string triggeringTimeMinutes = "triggeringTimeMinutes";
            DateTime triggeringTime = DateTime.Today.AddHours(Convert.ToDouble(ConfigurationManager.AppSettings[triggeringTimeHours])).AddMinutes(Convert.ToDouble(ConfigurationManager.AppSettings[triggeringTimeMinutes])).AddSeconds(0.0);
            DateTime currentTime = DateTime.UtcNow;
            if (currentTime > triggeringTime)
            {
                triggeringTime = triggeringTime.AddDays(1);
            }
            int remainingTime = (int)(triggeringTime - currentTime).TotalMilliseconds;
            LogManager.CurrentInstance.InfoLogger.LogInfo(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                "CurrentTime : " + currentTime + ", RemainingTime : " + remainingTime);
            _eventTimer.Change(remainingTime, Timeout.Infinite);
        }

        protected override void OnStop()
        {
            _eventTimer.Dispose();
        }
    }
}
