using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common
{
    /// <summary>
    /// Contains all constants keys used to retrieve values from .config file of the application and constant strings used in the application  .
    /// </summary>
    public static class NeeoConstants
    {
        /// <summary>
        /// Key for getting admin key of the XMPP User Service for performing administrative operations.
        /// </summary>
        public const string AdminKey = "adminKey";

        /// <summary>
        /// Containing the application name.
        /// </summary>
        public const string AppName = "Neeo";

        /// <summary>
        /// Containing the application resource name.
        /// </summary>
        public const string ResourceName = "mobile";

        /// <summary>
        /// Key for getting path of the apple certificate.
        /// </summary>
        public const string AppleCertificatePath = "appleCertificatePath";

        /// <summary>
        /// Key for getting password for the apple certificate.
        /// </summary>
        public const string AppleCertificatePwd = "appleCertificatePwd";

        /// <summary>
        /// Key for getting the activation code mask.
        /// </summary>
        /// <remarks> This will be replaced with the actual activation code.</remarks>
        public const string ActivationCodeMask = "activationCodeMask";

        /// <summary>
        /// Key for getting avatar handler information.
        /// </summary>
        /// <remarks>This is used to dynamically build url to access the user's avatar.</remarks>
        public const string AvatarHandler = "avatarHandler";

        /// <summary>
        /// Key for getting base url of the XMPP server.
        /// </summary>
        public const string XmppBaseUrl = "xmppBaseURL";

        /// <summary>
        /// Key for getting connection string for database server.
        /// </summary>
        public const string ConnectionStringName = "neeoConnectionString";

        /// <summary>
        /// Contains database code for invalid user.
        /// </summary>
        public const int DbInvalidUserCode = 50404;

        /// <summary>
        /// Key for getting application domain.
        /// </summary>
        public const string Domain = "domain";

        /// <summary>
        /// Key for getting file server url.
        /// </summary>
        public const string FileServerUrl = "fileServerURL";

        /// <summary>
        /// Key for getting google api key.
        /// </summary>
        public const string GoogleApiKey = "googleAPIKey";

        /// <summary>
        /// Key for getting image extension specifying type of image.
        /// </summary>
        public const string ImageExtension = "imageExtension";

        /// <summary>
        /// Key for getting image handler information.
        /// </summary>
        /// <remarks>This is used to dynamically build url to access the user's shared image.</remarks>
        public const string ImageHandler = "imageHandler";

        /// <summary>
        /// Key for getting address of the ip locator api.
        /// </summary>
        public const string IpLocatorApiUrl = "ipLocatorApiUrl";

        //public const string nonSslPort = "nonSslPort";

        /// <summary>
        /// Key for getting Nexmo account api key used in sms sending process.
        /// </summary>
        public const string NexmoApiKey = "nexmoApiKey";

        /// <summary>
        /// Key for getting Nexmo account api secret code used in sms sending process.
        /// </summary>
        public const string NexmoApiSecret = "nexmoApiSecret";

        /// <summary>
        /// Key for getting password encryption key.
        /// </summary>
        public const string PwdKey = "pwdKey";

        /// <summary>
        /// Key for getting root path to the application directory.
        /// </summary>
        public const string RootPath = "rootPath";

        /// <summary>
        /// Key for getting sms body used in sms sending process.
        /// </summary>
        public const string ActivationSmsText = "activationSmsText";

        //public const string sslPort = "sslPort";

        /// <summary>
        /// Key for getting signature key used in encryption.
        /// </summary>
        public const string SignatureKey = "signatureKey";

        /// <summary>
        /// Key for getting Twilio account secret id used in sms sending process.
        /// </summary>
        public const string TwilioAccountSid = "twilioAccountSid";

        /// <summary>
        /// Key for getting Twilio authentication token used in sms sending process.
        /// </summary>
        public const string TwilioAuthToken = "twilioAuthToken";

        /// <summary>
        /// Key for getting Twilio phone number used in sms sending process.
        /// </summary>
        public const string TwilioPhoneNumber = "twilioPhoneNumber";

        /// <summary>
        /// Key for getting name of the user's profile directory.
        /// </summary>
        public const string UserProfileDirectory = "profileDirectory";

        /// <summary>
        /// Key for getting name of the user's offline files directory.
        /// </summary>
        public const string SharedFilesPath = "sharedFilesPath";

        /// <summary>
        /// Key for getting file servers list.
        /// </summary>
        public const string FileServers = "fileServers";

        /// <summary>
        /// Key for getting file store application port.
        /// </summary>
        public const string FileStorePort = "fileStorePort";

        /// <summary>
        /// Key for getting voip server url.
        /// </summary>
        public const string VoipServerUrl = "voipServerUrl";

        /// <summary>
        /// Key for getting voip server secrect key for account registration.
        /// </summary>
        public const string VoipSecretKey = "voipSecretKey";

        /// <summary>
        /// A value specifying the number of characters in each hierarchy level.
        /// </summary>
        public const int HierarchyLevelLimit = 3;

        /// <summary>
        /// Key for getting value to decide whether to log request and response or not .
        /// </summary>
        public const string LogRequestResponse = "logRequestResponse";

        /// <summary>
        /// Key for getting value to decide whether to check number validity or not.
        /// </summary>
        public const string NumberValidityCheck = "numberValidityCheck";

        /// <summary>
        ///A constant key for getting the caller name value in dictionary.
        /// </summary>
        public const string CallerName = "callerName";

        /// <summary>
        ///A constant key for getting the receiver device token value in dictionary.
        /// </summary>
        public const string ReceiverDeviceToken = "receiverUserDToken";

        /// <summary>
        ///A constant key for getting the receiver device platfrom  value in dictionary.
        /// </summary>
        public const string ReceiverUserDeviceplatform = "receiverUserDp";

        /// <summary>
        ///A constant key for getting the receiver calling tone  value in dictionary.
        /// </summary>
        public const string ReceiverCallingTone = "receiverCallingTone";

        /// <summary>
        ///A constant key for getting the receiver offlineMsgCount value in dictionary.
        /// </summary>
        public const string OfflineMessageCount = "offlineMsgCount";

        /// <summary>
        /// Key for getting data encryption/decryption key.
        /// </summary>
        public const string EncryptionDecryptionKey = "encryptionDecryptionKey";

        /// <summary>
        /// Key for getting the web protocols for the url.
        /// </summary>
        public const string WebProtocol = "webProtocol";

        /// <summary>
        /// Key for getting file transfer supported version.
        /// </summary>
        public const string FileTransferSupportedVersions = "fileTransferSupportedVersions";

        /// <summary>
        /// Key for getting neeo invitation message text.
        /// </summary>
        public const string InvitationText = "invitationText";

        /// <summary>
        /// Key for android critical version.
        /// </summary>
        public const string AndroidCriticalVersion = "androidCriticalVersion";

        /// <summary>
        /// Key for android feature version for force install feature.
        /// </summary>
        public const string AndroidFeatureVersion = "androidFeatureVersion";

        /// <summary>
        /// Key for Ios critical version.
        /// </summary>
        public const string IosCriticalVersion = "iosCriticalVersion";

        /// <summary>
        /// Key for Ios critical version.
        /// </summary>
        public const string WPCriticalVersion = "wpCriticalVersion";

        /// <summary>
        /// Key for getting neeo invitation sender.
        /// </summary>
        public const string InvitationSenderKey = "invitationSenderKey";
        /// <summary>
        ///A constant string specifying timestamp format.
        /// </summary>
        public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Key for getting group image root path.
        /// </summary>
        public const string GroupImageRootPath = "groupImageRootPath";

        /// <summary>
        /// Key for getting resumable api url.
        /// </summary>
        public const string ResumableApiUrl = "resumableApiUrl";

        /// <summary>
        /// Key for getting push notification source.
        /// </summary>
        public const string PushNotificationSource = "pnSource";
        /// <summary>
        /// Key for getting sms sending source.
        /// </summary>
        public const string SmsSendingSource = "smsSendingSource";
        /// <summary>
        /// Key for getting sms api hierarchy.
        /// </summary>
        public const string SmsApiHierarchy = "smsApiHierarchy";
        /// <summary>
        /// Key for getting voice api hierarchy.
        /// </summary>
        public const string VoiceApiHierarchy = "voiceApiHierarchy";
        /// <summary>
        /// Key for getting sms api hierarchy.
        /// </summary>
        public const string GoogleTranslationKey = "googleTranslationKey";
        /// <summary>
        /// Key for getting disable swagger value.
        /// </summary>
        public const string DisableSwagger = "disableSwagger";

        /// <summary>
        /// Key for the usage of Amazon sms service.
        /// </summary>
        public const string AWSStatus = "AWSStatus";

        /// <summary>
        /// Enable restriction to work for specified country codes.
        /// </summary>
        public const string RestrictSpecifiedAeasSMS = "RestrictSpecifiedAeasSMS";

        /// <summary>
        /// Enable registration request check before sending the activation sms.
        /// </summary>
        public const string EnableRegisterationRequestCheck = "EnableRegisterationRequestCheck";

        

    }
}