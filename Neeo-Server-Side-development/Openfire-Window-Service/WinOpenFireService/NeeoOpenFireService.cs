using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Logger;

namespace WinOpenFireService
{
    public partial class NeeoOpenFireService : ServiceBase
    {
        private Timer _serviceTimer = new Timer();
        private readonly string _executableName;
        private readonly string _executablePath;
        private readonly double _timerInterval;
        private bool _isServiceStopped = false;
        private const string ExecutableName = "executableName";
        private const string ExecutablePath = "executablePath";
        private const string TimerInterval = "timerInterval";

        /*Log Messages*/
        private const string InitializingService = "Initializing service...";
        private const string VariablesLoaded = "Variables have loaded.";
        private const string ServiceStarted = "Service has started.";
        private const string StartingOpenfire = "Starting Openfire Server...";
        private const string OpenfireStarted = "Openfire Server has been started.";
        private const string OpenfireStopped = "Openfire has been stopped.";
        private const string ServiceStopped = "Service has stopped.";


        public NeeoOpenFireService()
        {
            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, InitializingService);
            InitializeComponent();
            try
            {
                _executableName = ConfigurationManager.AppSettings[ExecutableName];
                _executablePath = ConfigurationManager.AppSettings[ExecutablePath];
                _timerInterval = Convert.ToDouble(ConfigurationManager.AppSettings[TimerInterval]);
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, VariablesLoaded);
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                throw;
            }
        }

        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            _isServiceStopped = false;
            _serviceTimer.Interval = _timerInterval;
            _serviceTimer.Elapsed += ServiceTimerOnElapsed;
            _serviceTimer.Enabled = true;
            _serviceTimer.Start();
            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ServiceStarted);
        }

        private void ServiceTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            string executableOutput;
            try
            {
                var runningProcess = Process.GetProcessesByName(_executableName);
                if (runningProcess.Length == 0)
                {
                    using (var process = new System.Diagnostics.Process())
                    {

                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.FileName = Path.Combine(_executablePath, _executableName);
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        _serviceTimer.Enabled = false;
                        while (!_isServiceStopped)
                        {
                            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, StartingOpenfire);
                            process.Start();
                            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, OpenfireStarted);
                            executableOutput = process.StandardOutput.ReadToEnd();
                            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, executableOutput);
                            process.WaitForExit();
                            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, OpenfireStopped);
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                this.Stop();
            }
        }

        protected override void OnStop()
        {
            _serviceTimer.Stop();
            _isServiceStopped = true;
            var process = Process.GetProcessesByName(_executableName);
            if (process.Length > 0)
            {
                process[0].Kill();
            }
            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ServiceStopped);
        }
    }
}
