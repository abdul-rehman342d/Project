using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// A class that is holding device specific information.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Specifies the device model details
        /// </summary>
        public string DeviceModel { get; set; }
        /// <summary>
        /// Specifies device OS version
        /// </summary>
        public string OsVersion { get; set; }
        /// <summary>
        /// Specifies the device platform on which application is installed.
        /// </summary>
        public DevicePlatform DevicePlatform { get; set; }
        /// <summary>
        /// Specifies the unique device vender id information.
        /// </summary>
        public string DeviceVenderID { get; set; }
        /// <summary>
        /// Specifies the user's device token 
        /// </summary>
        public string DeviceToken { get; set; }
        /// <summary>
        /// Specifies the user's device token VoIP
        /// </summary>
        public string DeviceTokenVoIP { get; set; }
        /// <summary>
        /// Specifies the push notification source for sending out push notifications  
        /// </summary>
        public PushNotificationSource PushNotificationSource { get; set; }
    }
}
