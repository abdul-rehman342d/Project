using System;
using System.Configuration;

namespace Neeo.Notification
{
    public static class StringConstants
    {
        /// <summary>
        /// Key for getting incoming calling message text.
        /// </summary>
        public const string IncomingCallText = "incomingCallingMsgText";

        /// <summary>
        /// Key for getting mcr message text.
        /// </summary>
        public const string McrText = "mcrMsgText";

        /// <summary>
        /// Key for getting action key text.
        /// </summary>
        public const string ActionKeyText = "actionKeyText";

        /// <summary>
        /// Key for getting the name of ios application default tone.
        /// </summary>
        public const string IosApplicationMcrTone = "iosApplicationMcrTone";

        /// <summary>
        /// Key for getting the name of ios incoming calling tone.
        /// </summary>
        public const string IosIncomingCallTone = "iosIncomingCallTone";

        /// <summary>
        ///A constant string for senderID.
        /// </summary>
        public const string SenderID = "senderID";

        /// <summary>
        ///A constant string for callerID.
        /// </summary>
        public const string CallerID = "callerID";

        /// <summary>
        ///A constant string for timestamp.
        /// </summary>
        public const string Timestamp = "timestamp";

       

        /// <summary>
        ///A constant string for notification id.
        /// </summary>
        public const string NotificationID = "pnid";

        /// <summary>
        ///A key for getting allowed length for notification message.
        /// </summary>
        public const string NotificationMsgLength = "notificationMsgLength";

        /// <summary>
        /// A constant string for room ID.
        /// </summary>
        public const string RoomID = "rID";

        /// <summary>
        /// A constant string for room subject.
        /// </summary>
        public const string RoomSubject = "[roomSubject]";

        /// <summary>
        ///A key for getting group message text.
        /// </summary>
        public const string GroupMsgText = "groupMsgText";

        /// <summary>
        ///A key for getting windows package name.
        /// </summary>
        public const string WindowsPackageName = "windowsPackageName";

        /// <summary>
        ///A key for getting windows security Id.
        /// </summary>
        public const string WindowsSId = "windowsSId";

        /// <summary>
        ///A key for getting windows client secret.
        /// </summary>
        public const string WindowsClientSecret = "windowsClientSecret";

        /// <summary>
        /// Key for getting google api key.
        /// </summary>
        public const string GoogleApiKey = "googleAPIKey";

        /// <summary>
        /// Key for getting google api key.
        /// </summary>
        public const string AppleCertificatePath = "appleCertificatePath";

        /// <summary>
        /// Key for getting google api key.
        /// </summary>
        public const string AppleCertificateType = "appleCertificateType";


        /// <summary>
        /// Key for getting google api key.
        /// </summary>
        public const string AppleCertificatePwd = "appleCertificatePwd";

        /// <summary>
        /// A constant string for message id.
        /// </summary>
        public const string MessageId = "msgID";

        /// <summary>
        /// A constant string for profile pic update text.
        /// </summary>
        public const string ProfilePicUpdateText = "profilePicUpdateText";

    }

    public static class WnsAppConfiguration
    {
        public static string WindowsPackageName
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.WindowsPackageName];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.WindowsPackageName);
                }

                return value;
            }
        }

        public static string WindowsSId
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.WindowsSId];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.WindowsSId);
                }

                return value;
            }
        }

        public static string WindowsClientSecret
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.WindowsClientSecret];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.WindowsClientSecret);
                }

                return value;
            }
        }
    }

    public static class GcmAppConfiguration
    {
        public static string GoogleApiKey
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.GoogleApiKey];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.GoogleApiKey);
                }

                return value;
            }
        }
    }

    public static class ApnsAppConfiguration
    {
        public static string CertificatePath
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.AppleCertificatePath];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.AppleCertificatePath);
                }

                return value;
            }
        }

        public static string CertificatePwd
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.AppleCertificatePwd];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.AppleCertificatePwd);
                }

                return value;
            }
        }

        public static string CertificateType
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.AppleCertificateType];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.AppleCertificateType);
                }

                return value;
            }
        }

        public static string McrTone
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.IosApplicationMcrTone];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.IosApplicationMcrTone);
                }

                return value;
            }
        }

        public static string IncomingCallTone
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.IosIncomingCallTone];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.IosIncomingCallTone);
                }

                return value;
            }
        }

        public static string ActionKeyText
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.ActionKeyText];

                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.ActionKeyText);
                }

                return value;
            }
        }
    }

    public static class NotificationAppConfiguration
    {
        public static int NotificationMsgLength
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.NotificationMsgLength];

                if (String.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.NotificationMsgLength);
                }

                return Convert.ToInt32(value);
            }
        }

        public static string IncomingCallText
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.IncomingCallText];

                if (String.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.IncomingCallText);
                }

                return value;
            }
        }

        public static string McrText
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.McrText];

                if (String.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.McrText);
                }

                return value;
            }
        }

        public static string GroupMsgText
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.GroupMsgText];

                if (String.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.GroupMsgText);
                }

                return value;
            }
        }

        public static string ProfilePicUpdateText
        {
            get
            {
                var value = ConfigurationManager.AppSettings[StringConstants.ProfilePicUpdateText];

                if (String.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(StringConstants.ProfilePicUpdateText);
                }

                return value;
            }
        }
    }

    public static class ApnsStringConstant
    {
        public const string Aps = "aps";
        public const string Alert = "alert";
        public const string Badge = "badge";
        public const string Sound = "sound";
        public const string ActionLocKey = "action-loc-key";
        public const string Title = "title";
        public const string Body = "body";
        public const string ContentAvailable = "content-available";
    }

    public static class GcmStringConstant
    {
        public const string Alert = "alert";
    }
}