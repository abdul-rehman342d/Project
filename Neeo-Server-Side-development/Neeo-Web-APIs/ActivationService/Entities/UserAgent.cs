using System.Configuration;
using System.Runtime.Serialization;

namespace ActivationService
{
    /// <summary>
    /// A class holds the information about the client.
    /// </summary>
    [DataContract]
    public class UserAgent
    {
        #region Iphone 2.0.0, Android x.x.x

        /// <summary>
        /// Specifies the version of the client
        /// </summary>
        [DataMember]
        public string AppVer { get; set; }
        /// <summary>
        /// Specifies device OS version
        /// </summary>
        [DataMember]
        public string OsVer { get; set; }
        /// <summary>
        /// Specifies the device model details
        /// </summary>
        [DataMember]
        public string DM { get; set; }
        /// <summary>
        /// Specifies the device platform on which application is installed.
        /// </summary>
        [DataMember]
        public short DP { get; set; }
        /// <summary>
        /// Specifies the unique device vender id information.
        /// </summary>
        [DataMember]
        public string DVenID { get; set; }
        /// <summary>
        /// Specifies the unique application id 
        /// </summary>
        [DataMember]
        public string AppID { get; set; }
        /// <summary>
        /// Specifies the user's device token 
        /// </summary>
        [DataMember]
        public string DToken { get; set; }

        /// <summary>
        /// Specifies the user's device token 
        /// </summary>
        [DataMember]
        public string DTokenVoIP { get; set; }

        /// <summary>
        /// Specifies the push notification source for sending out push notifications  
        /// </summary>
        [DataMember]
        public int Pns { get; set; }

        #endregion

        #region Iphone 1.x.x, Android 1.x.x
        /*They will be deprecated in future.*/

        /// <summary>
        /// Specifies the device platform on which application is installed.
        /// </summary>
        [DataMember]
        public short DevicePlatform { get; set; }
        /// <summary>
        /// Specifies the unique device vender id information.
        /// </summary>
        [DataMember]
        public string DeviceVenderID { get; set; }
        /// <summary>
        /// Specifies the unique application id 
        /// </summary>
        [DataMember]
        public string ApplicationID { get; set; }
        /// <summary>
        /// Specifies the user's device token 
        /// </summary>
        [DataMember]
        public string DeviceToken { get; set; }

        #endregion
    }
}
