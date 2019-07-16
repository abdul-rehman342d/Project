using System;
using System.ComponentModel;
using System.Reflection;

namespace Common
{
    #region Activation

    /// <summary>
    /// Specifies the supported device platforms
    /// </summary>
    public enum DevicePlatform : short
    {
        iOS = 1,
        Android = 2,
        WindowsMobile = 3
        //Tablet = 4,
        //Mac = 5
    }

    /// <summary>
    /// Specifies the code sending service
    /// </summary>
    public enum CodeSendingService : short
    {
        Sms = 0,
        Call = 1
    }

    /// <summary>
    /// Specifies the sending status of sms.
    /// </summary>
    public enum SmsSendingStatus
    {
        //UserBlocked = -2,
        SendingFailed = 0,
        Sent = 1
    }

    /// <summary>
    /// Specifies the call status.
    /// </summary>
    public enum CallStatus
    {
        //UserBlocked = -2,
        CallFailed = 0,
        CalledSuccessfully = 1
    }

    /// <summary>
    /// Specifies the user blocking state.
    /// </summary>
    public enum UserState
    {
        NotBlocked = 0,
        Blocked = 1
    }

    #endregion

    #region Contact Syncing

    /// <summary>
    /// Specifies the contact roster state.
    /// </summary>
    public enum ContactRosterState
    {
        InvalidUser = -1,
        NotExists = 0,
        Exists = 1
    }

    /// <summary>
    /// Specifies the availabe roster subscriptions 
    /// </summary>
    public enum RosterSubscription
    {
        None = 0,
        To = 1,
        From = 2,
        Both = 3
    }

    #endregion

    #region Avatar State & FileSharing

    /// <summary>
    /// Specifies the user's avatar state reside on server side.
    /// </summary>
    public enum AvatarState
    {
        NotExist = 1,
        Modified = 2,
        NotModified = 3
    }

    /// <summary>
    /// Specifies the request type for file sharing.
    /// </summary>
    public enum RequestType
    {
        Profile,
        OfflineFiles
    }

    /// <summary>
    /// Specifies the user's avatar state reside on server side.
    /// </summary>
    public enum FileCategory : short
    {
        Profile = 1,
        Shared = 2,
        Group = 3
    }

    #endregion

    #region Notification

    /// <summary>
    /// Specifies the types of the push notifications.
    /// </summary>
    public enum NotificationType : short
    {
        /// <summary>
        /// Instant Message
        /// </summary>
        Im = 1,

        /// <summary>
        /// Incoming Sip Call
        /// </summary>
        IncomingSipCall = 2,

        /// <summary>
        /// Missed Call Record
        /// </summary>
        Mcr = 3,

        /// <summary>
        /// Group Instant Message
        /// </summary>
        GIm = 4,

        /// <summary>
        /// Group Invitation
        /// </summary>
        GInvite = 5
    }

    #endregion

    #region Settings

    /// <summary>
    /// Specifies the IM tones available for selection.
    /// </summary>
    public enum IMTone : ushort
    {
        [Description("imTone1.m4r")]
        ImTone1 = 1,
        [Description("imTone2.m4r")]
        ImTone2 = 2,
        [Description("imTone3.m4r")]
        ImTone3 = 3,
        [Description("imTone4.m4r")]
        ImTone4 = 4,
        [Description("imTone5.m4r")]
        ImTone5 = 5,
        [Description("imTone6.m4r")]
        ImTone6 = 6,
        [Description("imTone7.m4r")]
        ImTone7 = 7,
        [Description("imTone8.m4r")]
        ImTone8 = 8
    }

    /// <summary>
    /// Specifies the calling tones available for selection.
    /// </summary>
    public enum CallingTone : ushort
    {
        [Description("ringtone.mp3")]
        RingToneRingingGently = 0,
        [Description("Another Pattern.mp3")]
        RingToneAnotherPattern = 1,
        [Description("Anybody Home.mp3")]
        RingToneAnybodyHome = 2,
        [Description("Clean Clang.mp3")]
        RingToneCleanClang = 3,
        [Description("Crisp.mp3")]
        RingToneCrisp = 4,
        [Description("Dry.mp3")]
        RingToneDry = 5,
        [Description("Good Vibes.mp3")]
        RingToneGoodVibes = 6,
        [Description("Metro.mp3")]
        RingToneMetro = 7,
        [Description("Morning Call.mp3")]
        RingToneMorningCall = 8,
        [Description("Pattern.mp3")]
        RingTonePattern = 9,
        [Description("Progressive.mp3")]
        RingToneProgressive = 10,
        [Description("The Genuine Ring.mp3")]
        RingToneGenuineRing = 11,
        [Description("Woolly Ring.mp3")]
        RingToneWoollyRing = 12
    }

    public enum ToneType : ushort
    {
        IMTone = 1,
        CallingTone = 2
    }

    #endregion

    #region Restful Web Services

    /// <summary>
    /// Specifies the types of the operations.
    /// </summary>
    public enum OperationType : short
    {
        Add = 1,
        Update = 2,
        Delete = 3
    }

    /// <summary>
    /// Specifies the application specific http status code.
    /// </summary>
    public enum CustomHttpStatusCode
    {
        //Stardards Status
        VariantAlsoNegotiates = 506,
        InsufficientStorage = 507,
        LoopDetected = 508,
        BandwidthLimitExceeded = 509,
        NotExtended = 510,
        NetworkAuthenticationRequired = 511,
        OriginError = 520,
        ConnectionTimedOut = 522,
        ProxyDeclinedRequest = 523,
        ATimeoutOccurred = 524,
        NetworkReadTimeOutError = 598,
        NetworkConnectTimeOutError = 599,

        // Custom Errors
        [Description("The given number is invalid")]
        InvalidNumber = 530,
        [Description("An exception is thrown by SMS API")]
        SmsApiException = 531,
        [Description("Database transaction failed")]
        TransactionFailure = 532,
        [Description("Database operation failed")]
        DatabaseOperationFailure = 533,
        [Description("Request arguments are invalid")]
        InvalidArguments = 534,
        [Description("File system exception occured")]
        FileSystemException = 535,
        [Description("File data is not valid (not 64 base encoding)")]
        InvalidFileData = 536,
        [Description("File format is not same")]
        FileFormatMismatched = 537,
        [Description("Server connection failed")]
        ServerConnectionError = 538,
        [Description("Server internal error")]
        ServerInternalError = 539,
        [Description("Unknow error occured")]
        UnknownError = 540,
        [Description("User has been blocked")]
        UserBlocked = 541,
        [Description("Invalid user")]
        InvalidUser = 542,
        [Description("Incompatible application version")]
        Incompatible = 543,
        [Description("OS is incompatible")]
        OsIncompatible = 544,
        [Description("App version is incompatible")]
        AppIncompatible = 545,
        [Description("File transfer is not supported")]
        FileTransferNotSupported = 546,
        [Description("Invalid language code")]
        InvalidLanguageCode = 547
    }

    #endregion

    #region Offline Count Types

    /// <summary>
    /// Specifies the types of offline counts.
    /// </summary>
    public enum OfflineCount : short
    {
        Offline = 1,
        MCR = 2
    }

    #endregion

    public enum MucMessageType
    {
        [Description("Instant Message")]
        Im = 1,
        [Description("Image Message")]
        Pm = 2,
        [Description("Audio Message")]
        Am = 3,
        [Description("Voice Message")]
        Avm = 4,
        [Description("Video Message")]
        Vm = 5
    }

    public enum ResourceType
    {
        [Description("Background")]
        Background = 1,
        [Description("Sticker")]
        Sticker = 2
    }

#region APNS

    public enum PushNotificationSource
    {
        [Description("Default")] 
        Default,
        [Description("Pushy")]
        Pushy
    }

    public enum FriendRequestStatus
    {
        NotExist = -1,
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }

    #endregion




}
