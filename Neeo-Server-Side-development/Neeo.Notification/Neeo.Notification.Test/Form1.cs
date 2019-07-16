using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibNeeo;
using Logger;
using Microsoft.Owin.Hosting;
using Neeo.Notification.Factory;
using Neeo.Notification.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;


namespace Neeo.Notification.Test
{
    public partial class Form1 : Form
    {
        private StartOptions _options = null;
        private IDisposable _server = null;
        private System.Timers.Timer _timer;
        private const string Urls = "urls";
        private NotificationService service;

        private ApnsServiceBroker _apnsServiceBroker;

        public Form1()
        {
            InitializeComponent();
            try
            {
                NotificationHandler.GetInstance().StartService();
                //service = new NotificationServiceFactory().GetServiceInstance(PushNotificationType.Apns);

                //var apnsConfig = new ApnsConfiguration(
                //                                       ApnsAppConfiguration.CertificateType == "production" ? ApnsConfiguration.ApnsServerEnvironment.Production : ApnsConfiguration.ApnsServerEnvironment.Sandbox,
                //                                       ApnsAppConfiguration.CertificatePath,
                //                                       ApnsAppConfiguration.CertificatePwd);
                //_apnsServiceBroker = new ApnsServiceBroker(apnsConfig);
                //_apnsServiceBroker.ChangeScale(10);
                //_apnsServiceBroker.Start();
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            //service.Send(new List<NeeoUser>(), new NotificationModel(){ NType = NotificationType.Im, Alert = "zohaib says: hello", Badge = 6});
            var payload = "{";
            payload += "\"aps\":{";
            payload += "\"alert\":\"hello world\",";
            //payload += "\"sound\":\"" + notificationModel.IMTone.GetDescription() + "\",";
            payload += "\"sound\":\"imTone1.m4r\",";
            payload += "\"badge\" : 7";
            payload += "},";
            payload += "}";
            var notification = new ApnsNotification("cee0ffae7a0ad0ee3a2b70092c3ba3d29be0be39c42b719d146681c11710b9a9", JObject.Parse(payload));
            _apnsServiceBroker.QueueNotification(notification);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_server != null)
            {
                _server.Dispose();
            }
            service.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            service.Send(new List<NeeoUser>(), new NotificationModel() { NType = NotificationType.IncomingSipCall, Alert = "ali is calling", Badge = 10 });
        }
    }
}
