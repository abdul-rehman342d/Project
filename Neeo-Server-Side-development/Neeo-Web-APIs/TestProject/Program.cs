using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using LibNeeo.Activation;
using LibNeeo.IO;
using LibNeeo.MUC;
using LibNeeo.Plugin;
using LibNeeo;
using DAL;
using System.Data;
using System.Runtime.InteropServices;
using System.Globalization;
using LibNeeo.Network;
using LibNeeo.Voip;
using Microsoft.Owin.Hosting;
using Microsoft.SqlServer.Server;
using PushSharp.Core;
using PushSharp.Apple;
using PushSharp.Windows;
using PushSharp;
using System.Configuration;
using System.Data.SqlTypes;
using System.Timers;
using PowerfulPal.Sms;
using System.Web;

namespace TestProject
{
    public enum AppCapabilities : ushort
    {
        /// <summary>
        /// VoIP Audio Calling
        /// </summary>
        Vac = 1 << 0,

        /// <summary>
        /// VoIP Video Calling
        /// </summary>
        Vvc = 1 << 1,

        /// <summary>
        /// Audio Sharing
        /// </summary>
        Ash = 1 << 2,

        /// <summary>
        /// Video Sharing
        /// </summary>
        Vsh = 1 << 3,

        /// <summary>
        /// Photo Sharing
        /// </summary>
        Psh = 1 << 4,

        /// <summary>
        /// Group Chat
        /// </summary>
        Gch = 1 << 5,

        /// <summary>
        /// All Capabilities
        /// </summary>
        All = Vac | Vvc | Ash | Vsh | Psh | Gch

    }

    class Program
    {
        static void Main(string[] args)
        {

            //sendrequest();
            //Console.WriteLine(HttpStatusCode.Conflict.GetDescription());
            //Console.WriteLine(DevicePlatform.Android.GetDescription());
            //Console.WriteLine(IMTone.ImTone5.GetDescription());
            //string phoneNumber = "77015116075";
            //var result = NeeoUtility.IsPhoneNumberInInternationalFormat(phoneNumber);

            // var result2 = NeeoUtility.ValidatePhoneNumber(NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber));

            //PowerfulPal.Sms.SmsManager.GetInstance().ExpertApi.SendSms(new[] { "+923338735963" }, "ایک نیا پیغام کار ایپلی کیشن کے لئے خوش آمدید",true);

            //FileServerManager.GetInstance().SelectServer();
            //PowerfulPal.Sms.SmsManager.GetInstance().SendSms(new[] { "+5521986364875" }, "This is a test message being sent on \"+" + 5521986364875 + "\". Neeo Support");
            //PowerfulPal.Sms.SmsManager.GetInstance().SendSms(new[] { "+55986364875" }, "This is a test message being sent on \"+" + 55986364875 + "\". Neeo Support");
            //PowerfulPal.Sms.SmsManager.GetInstance().Nexmo.SendSms(new[] { "+923458412963" }, "Neeo، ایک نیا پیغام کار ایپلی کیشن کے لئے خوش آمدید. Neeo، ایک نیا پیغام کار ایپلی کیشن کے لئے خوش آمدید.",true);
            // PowerfulPal.Sms.SmsManager.GetInstance().Nexmo.SendSms(new[] { "+923458412963" }, "Neeo app real time translation app");
            //PowerfulPal.Sms.SmsManager.GetInstance().Twilio.SendSms(new[] { "+923458412963" }, "This is the test message.");
            //PowerfulPal.Sms.CmSmsGatewayApi i = new CmSmsGatewayApi();
            //i.SendSms(new[] { "+923458412963" }, "Neeo، ایک نیا پیغام کار ایپلی کیشن کے لئے خوش آمدید. Neeo، ایک نیا پیغام کار ایپلی کیشن کے لئے خوش آمدید.", true);
            //PowerfulPal.Sms.CmSmsGatewayApi smsGatewayApi = new CmSmsGatewayApi();
            //smsGatewayApi.SendSms(new[] { "+923338735963" }, "ہیلو دنیا");
            //LoadBalancer b1 = LoadBalancer.GetLoadBalancer();

            //LoadBalancer b2 = LoadBalancer.GetLoadBalancer();

            //LoadBalancer b3 = LoadBalancer.GetLoadBalancer();

            //LoadBalancer b4 = LoadBalancer.GetLoadBalancer();



            //// Confirm these are the same instance

            //if (b1 == b2 && b2 == b3 && b3 == b4)
            //{

            //    Console.WriteLine("Same instance\n");

            //}



            //// Next, load balance 15 requests for a server

            //LoadBalancer balancer = LoadBalancer.GetLoadBalancer();

            //for (int i = 0; i < 15; i++)
            //{

            //    string serverName = balancer.NextServer.Name;

            //    Console.WriteLine("Dispatch request to: " + serverName);

            //}



            //// Wait for user

            //Console.ReadKey();
            //string baseAddress = "http://+:80/";

            //StartOptions options = new StartOptions();
            //char[] delimeter = {';'};
            //string[] urls = ConfigurationManager.AppSettings["urls"].Split(delimeter);
            //foreach (var url in urls)
            //{
            //    options.Urls.Add(url);
            //}

            ////// Start OWIN host 
            //WebApp.Start<Startup>(options);

            ////var timer = new System.Timers.Timer();
            ////timer.Elapsed += TimerOnElapsed;
            ////timer.Interval = 270000;
            ////timer.Start();
            //Console.ReadLine();
            ////timer.Stop();

            #region old work

            //using (WebApp.Start<Startup>(url: baseAddress))
            //{
            //    // Create HttpCient and make a request to api/values 
            //    //HttpClient client = new HttpClient();

            //    //var response = client.GetAsync(baseAddress + "api/values").Result;

            //    //Console.WriteLine(response);
            //    //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            //}
            //string gid = "ADGSD2342SF234df";
            //Console.WriteLine(gid.ToLower());

            // HttpWebRequest request = null;
            // HttpWebResponse response = null;
            // string reqBody = "{\"userID\":\"79670424444\",\"contacts\":[{\"AvatarTimeStamp\":\"0\",\"ContactPhoneNumber\":\"923214894640\"},{\"AvatarTimeStamp\":\"0\",\"ContactPhoneNumber\":\"82 1063271665\"},{\"AvatarTimeStamp\":\"0\",\"ContactPhoneNumber\":\"7(906)7457575\"},{\"AvatarTimeStamp\":\"0\",\"ContactPhoneNumber\":\"971+971569459205\"},{\"AvatarTimeStamp\":\"0\",\"ContactPhoneNumber\":\"%%%%%%%%%%%%\"},{\"AvatarTimeStamp\":\"0\",\"ContactPhoneNumber\":\"%%%%79164738086\"}]}";
            //byte[] postBytes = new ASCIIEncoding().GetBytes(reqBody.ToString());



            // try
            // {
            //     //string url = ConfigurationManager.AppSettings[pushNotificationURL].ToString();
            //     string url = "http://nserv-sync.neeopal.com/Service/NeeoSyncingService.svc/SyncUpContacts";
            //     request = (HttpWebRequest)WebRequest.Create(url);
            //     request.Method = "POST";
            //     request.ContentType = "application/json;charset=utf-8";
            //     Stream postStream = request.GetRequestStream();
            //     postStream.Write(postBytes, 0, postBytes.Length);
            //     postStream.Flush();
            //     postStream.Dispose();
            //     Console.WriteLine(DateTime.Now.Ticks);
            //     response = (HttpWebResponse)request.GetResponse();
            //     response.Close();
            // //    Console.WriteLine(DateTime.Now.Ticks);
            // //    var taskGetResponse = Task.Factory.StartNew(() =>
            // //    {
            // //        Console.WriteLine(DateTime.Now.Ticks);
            // //        response = (HttpWebResponse)request.GetResponse();
            // //        response.Close();
            // //        Console.WriteLine(DateTime.Now.Ticks);
            // //    });
            // //    //Thread.Sleep(1000);
            // //    //taskGetResponse.Wait();
            // }
            // catch (System.Configuration.ConfigurationErrorsException ex)
            // {
            //     SqlContext.Pipe.Send(ex.Message);
            // }
            // catch (System.Net.WebException ex)
            // {
            // }
            // catch (System.ObjectDisposedException ex)
            // {
            // }
            // catch (System.NotSupportedException ex)
            // {
            // }
            // catch (System.Net.ProtocolViolationException ex)
            // {
            // }
            // catch (System.Security.SecurityException ex)
            // {
            // }
            // catch (System.UriFormatException ex)
            // {

            // }

            //List<NeeoUser> lstUser = NeeoGroup.GetGroupParticipants(10, "923336356682");
            //RegisterVoipUsers();
            //Console.WriteLine("Program is going to end.");
            //Console.ReadLine();



            //string[] contacts = {"qewrqewr232"};
            //ulong temp;
            //var result = contacts.Select(item => item).Where(e => ulong.TryParse(e, out temp) == true).Aggregate((a, b) => a + "," + b);
            ////if (result.Any())
            ////{
            ////    string result2 = result.Aggregate((a, b) => a + "," + b);
            ////}
            //string o;
            //ushort i;
            //Console.WriteLine("Request capability");
            //i = Convert.ToUInt16(Console.ReadLine());

            //if ((AppCapabilities) i == AppCapabilities.All)
            //{
            //    Console.WriteLine("return all capabilities");
            //}
            //else
            //{
            //    var enumValues = (AppCapabilities[])Enum.GetValues(typeof (AppCapabilities));
            //    for (int index = 0; index < enumValues.Length - 1;index ++)
            //    {
            //        AppCapabilities appCapabilities =
            //            (AppCapabilities) (i & Convert.ToInt16(enumValues[index].ToString("D")));

            //        switch (appCapabilities)
            //        {
            //                case AppCapabilities.Vac:
            //                    Console.WriteLine("return Voip audio call capability");
            //                    break;
            //                case AppCapabilities.Vvc:
            //                    Console.WriteLine("return Voip video call capability");
            //                    break;
            //                case AppCapabilities.Ash:
            //                    Console.WriteLine("return audio sharing capability");
            //                    break;
            //                case AppCapabilities.Vsh:
            //                    Console.WriteLine("return video sharing capability");
            //                    break;
            //                case AppCapabilities.Psh:
            //                    Console.WriteLine("return photo sharing capability");
            //                    break;
            //                case AppCapabilities.Gch:
            //                    Console.WriteLine("return group chat capability");
            //                    break;

            //        }
            //    }
            //if ((AppCapabilities) (i & Convert.ToInt16(AppCapabilities.Vac.ToString("D"))) == AppCapabilities.Vac)
            //{
            //    Console.WriteLine("return Voip audio call capability");
            //}
            //if ((AppCapabilities)(i & Convert.ToInt16(AppCapabilities.Vvc.ToString("D"))) == AppCapabilities.Vvc)
            //{
            //    Console.WriteLine("return Voip video call capability");
            //}
            //if ((AppCapabilities)(i & Convert.ToInt16(AppCapabilities.Ash.ToString("D"))) == AppCapabilities.Ash)
            //{
            //    Console.WriteLine("return audio sharing capability");
            //}
            //if ((AppCapabilities)(i & Convert.ToInt16(AppCapabilities.Vsh.ToString("D"))) == AppCapabilities.Vsh)
            //{
            //    Console.WriteLine("return video sharing capability");
            //}
            //if ((AppCapabilities)(i & Convert.ToInt16(AppCapabilities.Psh.ToString("D"))) == AppCapabilities.Psh)
            //{
            //    Console.WriteLine("return photo sharing capability");
            //}
            //if ((AppCapabilities)(i & Convert.ToInt16(AppCapabilities.Gch.ToString("D"))) == AppCapabilities.Gch)
            //{
            //    Console.WriteLine("return group chat capability");
            //}

            //Console.WriteLine("Enter 0 to exit");
            //i = Convert.ToUInt16(Console.ReadLine());

            //}

            //try
            //{

            //         HttpWebRequest request = null;
            //HttpWebResponse response = null;
            //        string body =
            //            "{\"api_key\":\"ec6e3dc8\",\"api_secret\":\"52909b41\",\"from\":\"zohaib\",\"to\":\"+923458412963\",\"text\":\"コール\"}";
            //         byte[] postBytes = new UTF8Encoding().GetBytes(body.ToString());
            //        string url = "https://rest.nexmo.com/sms/json";
            //    request = (HttpWebRequest)WebRequest.Create(url);
            //    request.Method = "POST";
            //    request.ContentType = "application/json;charset=utf-8";
            //    Stream postStream = request.GetRequestStream();
            //    postStream.Write(postBytes, 0, postBytes.Length);
            //    postStream.Flush();
            //    postStream.Dispose();
            //    response = (HttpWebResponse)request.GetResponse();
            //    response.Close();
            //                string encryptedString =
            //NeeoUtility.EncryptData("190934823409234");
            //                //encryptedString = encryptedString.Replace('V', 'v').Replace('g', 'G');
            //                string origionalData = NeeoUtility.DecryptData(encryptedString);
            //            }
            //            catch (Exception)
            //            {

            //            }


            //test comment
            //DateTime dt = DateTime.UtcNow;
            //string t = dt.ToString("yyyy-MM-dd HH:mm:ss +0000");
            //var json =
            //    new WebClient().DownloadString(
            //        "http://neeotest.neeopal.com:9000/Service/NeeoActivationService.svc/GetCountry");

            //var data =
            //    new WebClient().DownloadData(
            //        "http://neeotest.neeopal.com:9000/Service/NeeoActivationService.svc/GetCountry");
            //Stream strm = new MemoryStream(data);
            //StreamReader strmReader = new StreamReader(strm);
            //string result =strmReader.ReadToEnd();
            TestProject.NotificationManager nmManager = new TestProject.NotificationManager();
            nmManager.SendNotification();
            //var d = NeeoVoipApi.GetMcrCount("923458412963");
            //NeeoVoipApi.RegisterUser("9277777799","12345",0);
            //NeeoVoipApi.UpdatePushEnabled("9277777799",1);
            //NeeoVoipApi.UpdatePushEnabled("9277777799", 0);
            //NeeoVoipApi.RegisterUser("9277777799", "12345", 0);
            // string myString;
            //Console.Write("Enter your message: ");
            //myString = Console.ReadLine();
            //MessageBox((IntPtr)0, myString, "Warning", 6);
            //string phoneNumber = "92900980";
            //string directoryHierarchy = "";
            //int hierarchyLevels = phoneNumber.Length / NeeoConstants.HierarchyLevelCharacterLimit;
            //int index = 0;
            //for (int ii = 0; ii < hierarchyLevels; ii++)
            //{
            //    directoryHierarchy = Path.Combine(directoryHierarchy, phoneNumber.Substring(index, NeeoConstants.HierarchyLevelCharacterLimit));
            //    index += NeeoConstants.HierarchyLevelCharacterLimit;
            //}


            //string directoryPath = @"E:\NeeoUserDirectories";
            //directoryPath = Path.Combine(directoryPath, Path.Combine(directoryHierarchy,phoneNumber));
            //DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryPath);
            //NeeoUtility.ValidatePhoneNumber("adsfasdfsdf");
            //NeeoUtility.ValidatePhoneNumber("+92524260957");
            //NeeoUtility.ValidatePhoneNumber("123");
            //string[] i = Directory.GetFiles(directoryPath);
            //string[] o = Directory.GetDirectories(directoryPath);
            //DirectorySecurity dir = new DirectorySecurity();
            //string[] i = Directory.SetAccessControl(directoryPath,new );
            // string contacts = "admin,ali,ahmad,ahmadali,923214849158,923333333524,923458412968,923213849134,923214844108,13";
            //string user = "+923388412963";
            //bool o = Convert.ToBoolean(null);
            //bool result = NeeoUtility.ValidatePhoneNumber(user);
            //Console.WriteLine(result);
            // User user = new User("923458412963");
            //bool result = user.UpdateUsersDisplayName("ali");
            //CultureInfo enUS = new CultureInfo("en-US");
            //DateTime currentTime = DateTime.Now;
            //Console.WriteLine("Time Stamp: "+ GetTimeStamp(currentTime));
            //Console.WriteLine(currentTime.ToOADate().ToString());
            //DateTime conversionTime;
            //if (DateTime.TryParseExact(currentTime.Minute.ToString(), "hh:ss:mm", enUS, DateTimeStyles.None,
            //    out conversionTime))
            //{
            //    Console.WriteLine("converted");
            //}
            //Console.WriteLine(conversionTime.ToString());
            //Console.WriteLine(conversionTime.ToBinary().ToString());

            //DirectoryInfo dir=new DirectoryInfo(@"E:\Neeo User Directories\923458412963\Profile");
            // dir.GetFiles();

            //User currentUser = new User(user);
            //currentUser.GetAvatarState();
            //Locator.GetLocation("202.166.175.78");
            //string key = NeeoActivation.GenerateDeviceKey("+923336699701", "874B8453-A03B-495A-90F5-16B581E89330", "CA6CAE08-43E0-4E38-8A04-F8CA05F001F9");
            //Console.WriteLine(key);

            #endregion
        }

        private static void sendrequest()
        {

            string PackageID = "ms-app://s-1-15-2-1027086546-3793768442-914081366-159525117-234043670-2279954648-2289932992";
            string ClientSecret = "zwj31+N0gDU2gFwXXTSFN4wrLkGtKZ1/";
            //string ChannerURi = "https://db3.notify.windows.com/?token=AwYAAAAqXMszqk%2bfAUVbw0UF7MVDfYowCTJdlj4MYr2Ng9SQCRfe72FBYw17u%2bMhm9QCKPJ6oGNyPIoH3P9XpGmA4wRPhtJCf8QYMSxLRM4QihH%2fAbHjhr9%2b8PMIHSrJ40A8Ac4%3d";
            // <Identity Name="POWERFULPALLTD.Neeo" Publisher="CN=0863A05C-A70C-40CD-83DB-1178A44AE92E" Version="1.0.0.0"  />
            try
            {


                string urlEncodedSid = HttpUtility.UrlEncode(PackageID);
                string urlEncodedSecret = HttpUtility.UrlEncode(ClientSecret);

                string body = String.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", urlEncodedSid, urlEncodedSecret);


                WebClient client = new WebClient();
                client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                client.UploadStringAsync(new Uri("https://login.live.com/accesstoken.srf", UriKind.Absolute), body);
                client.UploadStringCompleted += client_UploadStringCompleted;

                Thread.Sleep(20000);

            }


            catch (Exception ex)
            {


            }


        }
        static void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            string result = e.Result;

        }
       


        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var client = new HttpClient();
            var response = client.GetAsync("http://localhost:9004/api/notification/keepalive").Result;
        }

        public static IEnumerable<byte> GetBytesFromByteString(string s)
        {
            for (int index = 0; index < s.Length; index += 2)
            {
                yield return Convert.ToByte(s.Substring(index, 2), 16);
            }
        }
        public static void RegisterVoipUsers()
        {
            //var _conStr = ConfigurationManager.ConnectionStrings["NeeoDb"].ConnectionString;
            //var _con = new SqlConnection(_conStr);
            //try
            //{
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.CommandText = "Select username, deviceVenderID, applicationID, deviceToken from neUserExtension";
            //    cmd.CommandType = CommandType.Text;
            //    cmd.Connection = _con;
            //    Console.WriteLine("Starting script.");
            //    DataTable dtContactsInfo = new DataTable();
            //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //    adapter.Fill(dtContactsInfo);

            //    string deviceKey = null;
            //    if (dtContactsInfo.Rows.Count > 0)
            //    {
            //        Console.WriteLine(dtContactsInfo.Rows.Count);
            //        Console.WriteLine();
            //        for (int i = 0; i < dtContactsInfo.Rows.Count; i++)
            //        {
            //            Console.WriteLine();
            //            Console.WriteLine(dtContactsInfo.Rows[i]["username"].ToString());
            //            //deviceKey = NeeoActivation.GenerateDeviceKey(dtContactsInfo.Rows[i]["username"].ToString(), dtContactsInfo.Rows[i]["deviceVenderID"].ToString(), dtContactsInfo.Rows[i]["applicationID"].ToString());
            //            try
            //            {
            //                NeeoVoipApi.RegisterUser(dtContactsInfo.Rows[i]["username"].ToString(), deviceKey,
            //                    dtContactsInfo.Rows[i]["deviceToken"].ToString() == "" ? PushEnabled.False : PushEnabled.True);
            //            }
            //            catch (ApplicationException applicationException)
            //            {
            //                Console.WriteLine(applicationException.Message);
            //            }
            //            catch (Exception exception)
            //            {
            //                Console.WriteLine(exception.Message);
            //            }
            //            Console.WriteLine("Registered.");
            //            Console.WriteLine();
            //            deviceKey = null;

            //        }
            //    }
            //}
            //catch (SqlException sqlEx)
            //{
            //   Console.WriteLine(sqlEx.Message);
            //}
            //catch (Exception Ex)
            //{
            //    Console.WriteLine(Ex.Message);
            //}
            //finally
            //{
            //    if (_con.State != ConnectionState.Closed)
            //    {
            //        _con.Close();
            //    }
            //}
        }
        public static string GetTimeStamp(DateTime datetime)
        {
            return datetime.Year.ToString().Substring(2, 2) + datetime.Second.ToString() + datetime.Day.ToString() +
                   datetime.Hour.ToString() + datetime.Month.ToString() + datetime.Minute.ToString();
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

    }

    sealed class LoadBalancer
    {

        // Static members are 'eagerly initialized', that is,

        // immediately when class is loaded for the first time.

        // .NET guarantees thread safety for static initialization

        private static readonly LoadBalancer _instance =

          new LoadBalancer();



        // Type-safe generic list of servers

        private List<Server> _servers;

        private Random _random = new Random();



        // Note: constructor is 'private'

        private LoadBalancer()
        {

            // Load list of available servers

            _servers = new List<Server>

        {

         new Server{ Name = "ServerI", IP = "120.14.220.18" },

         new Server{ Name = "ServerII", IP = "120.14.220.19" },

         new Server{ Name = "ServerIII", IP = "120.14.220.20" },

         new Server{ Name = "ServerIV", IP = "120.14.220.21" },

         new Server{ Name = "ServerV", IP = "120.14.220.22" },

        };

        }



        public static LoadBalancer GetLoadBalancer()
        {

            return _instance;

        }



        // Simple, but effective load balancer

        public Server NextServer
        {

            get
            {

                int r = _random.Next(_servers.Count);

                return _servers[r];

            }

        }

    }



    /// <summary>

    /// Represents a server machine

    /// </summary>

    class Server
    {

        // Gets or sets server name

        public string Name { get; set; }



        // Gets or sets server IP address

        public string IP { get; set; }

    }



    public sealed class NotificationManager
    {
        /// <summary>
        /// An object responsible for sending push notification. 
        /// </summary>
        private static PushBroker _push;
        //public static NotificationManager Notification = new NotificationManager();
        private const string NotificationID = "pnid";
        private static string _iosApplicationDefaultTone;
        private static string _iosIncomingCallingTone;
        private static string _incomingCallingMsgText;
        private static string _mcrMsgText;
        private static string _actionKeyText;


        /// <summary>
        /// 
        /// </summary>

        public NotificationManager()
        {
            if (_push == null)
            {
                _push = new PushBroker();
                //Registring events.
                _push.OnNotificationSent += NotificationSent;
                _push.OnChannelException += ChannelException;
                _push.OnServiceException += ServiceException;
                _push.OnNotificationFailed += NotificationFailed;
                _push.OnNotificationRequeue += NotificationRequeue;
                _push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                _push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                _push.OnChannelCreated += ChannelCreated;
                _push.OnChannelDestroyed += ChannelDestroyed;


                //_push.RegisterGcmService(new GcmPushChannelSettings(ConfigurationManager.AppSettings[NeeoConstants.GoogleApiKey].ToString()));

            }
        }

        /// <summary>
        /// Sends notifications to the specified device token and device platform .
        /// </summary>
        /// <param name="notificationType">An enum specifying the notification type.</param>
        /// <param name="devicePlatform">An enum specifying the device platform.</param>
        /// <param name="deviceToken">A string containing the device token.</param>
        /// <param name="data">A dictionary containing the notification data.</param>
        public void SendNotification()
        {

            //string dToken = "69b81fdb67b24b8141f4a4539a558ffe8b19216de0d8ea2985dc1ca1653e75ef";
            //var appleCert = System.IO.File.ReadAllBytes(@"E:\Zohaib\Projects\Neeo\Services\Certificate\apn_identity.p12");
            //_push.RegisterAppleService(new ApplePushChannelSettings(true, appleCert, "PowerfulP1234"));
            //_push.QueueNotification(new AppleNotification()
            //                           .ForDeviceToken(dToken)
            //                           .WithAlert("Hello World!")
            //                           .WithBadge(7)
            //                           .WithSound("default"));

            //Thread.Sleep(10000);

            //-------------------------
            // WINDOWS NOTIFICATIONS
            //-------------------------
            //Configure and start Windows Notifications
            _push.RegisterWindowsService(new WindowsPushChannelSettings("POWERFULPALLTD.Neeo",
                "ms-app://s-1-15-2-1027086546-3793768442-914081366-159525117-234043670-2279954648-2289932992", "zwj31+N0gDU2gFwXXTSFN4wrLkGtKZ1/"));
            //Fluent construction of a Windows Toast Notification
            _push.QueueNotification(new WindowsToastNotification()
                .AsToastText01("Hy i am WNS")
                .ForChannelUri("https://db3.notify.windows.com/?token=AwYAAAC2cuRUeNPTE1OQAAwC1Lmb5BAD98Mk02%2fGY1HvP360VaRtVQqTWRVHWWfJY6RYwzeUICclQbT2nYKGRui18FGlI%2bM9zQW2vqcb7crbULg4GI7XvrSeJTwdKq9k9waOxAs%3d"));

            Thread.Sleep(20000);
        }
        /// <summary>
        /// An event that is called when channel destroyed.
        /// </summary>
        /// <param name="sender"></param>
        private void ChannelDestroyed(object sender)
        {
        }
        /// <summary>
        /// An event is called when channel created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushChannel"></param>
        private void ChannelCreated(object sender, PushSharp.Core.IPushChannel pushChannel)
        {

        }
        /// <summary>
        /// An event that is called when device subscription changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oldSubscriptionId"></param>
        /// <param name="newSubscriptionId"></param>
        /// <param name="notification"></param>
        private void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, PushSharp.Core.INotification notification)
        {
            // LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Device subscription changed.");
        }
        /// <summary>
        /// An event that is called when device subscrpition expired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="expiredSubscriptionId"></param>
        /// <param name="expirationDateUtc"></param>
        /// <param name="notification"></param>
        private void DeviceSubscriptionExpired(object sender, string expiredSubscriptionId, DateTime expirationDateUtc, PushSharp.Core.INotification notification)
        {
            //  LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Device subscription expired.");
        }
        /// <summary>
        /// An event that is called when notification failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        /// <param name="error"></param>
        private void NotificationFailed(object sender, PushSharp.Core.INotification notification, Exception error)
        {
            //LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, error.Message);
        }
        /// <summary>
        /// An event that is called when service exception occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        private void ServiceException(object sender, Exception error)
        {
            // LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, error.Message);
        }
        /// <summary>
        /// An event that is called when channel exception occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushChannel"></param>
        /// <param name="error"></param>
        private void ChannelException(object sender, PushSharp.Core.IPushChannel pushChannel, Exception error)
        {
            // LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, error.Message);
        }
        /// <summary>
        /// An event that is call when notification successfully sent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        private void NotificationSent(object sender, PushSharp.Core.INotification notification)
        {

        }

        private void NotificationRequeue(object sender, NotificationRequeueEventArgs e)
        {

        }
    }

}
