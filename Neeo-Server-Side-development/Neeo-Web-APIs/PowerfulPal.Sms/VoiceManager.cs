using Common;
using Logger;
using System;
using System.Configuration;

namespace PowerfulPal.Sms
{
    /// <summary>
    /// Calls to the provided phone number.(Singleton)
    /// </summary>
    public sealed class VoiceManager : IVoice
    {
        #region Data Members
        /// <summary>
        /// 
        /// </summary>
        private static readonly VoiceManager Instance = new VoiceManager();

        /// <summary>
        /// 
        /// </summary>
        private readonly IVoice _successorApi;
        private readonly IVoice _primaryApi;
        private readonly IVoice _secondaryApi;
        /// <summary>
        /// 
        /// </summary>
        private readonly NexmoApi _nexmoInstance;
        /// <summary>
        /// 
        /// </summary>
        private readonly TwilioApi _twilioInstance;
        /// <summary>
        /// 
        /// </summary>
        private readonly ExpertTextingApi _expertApiInstance;
        /// <summary>
        /// Gets primary api instance
        /// </summary>
        public IVoice PrimaryApi
        {
            get
            {
                return _primaryApi;
            }
        }
        /// <summary>
        /// Gets secondary api instance
        /// </summary>
        public IVoice SecondaryApi
        {
            get
            {
                return _secondaryApi;
            }
        }
        /// <summary>
        /// Gets Nexmos api instance
        /// </summary>
        public IVoice Nexmo
        {
            get
            {
                return _nexmoInstance;
            }
        }
        /// <summary>
        /// Gets Twilio api instance.
        /// </summary>
        public IVoice Twilio
        {
            get
            {
                return _twilioInstance;
            }
        }

        /// <summary>
        /// Specifies the successor api.
        /// </summary>
        public IVoice SuccessorApi { get; set; }

        #endregion

        /// <summary>
        /// Constructor of the singleton class.
        /// </summary>
        private VoiceManager()
        {
            _nexmoInstance = new NexmoApi();
            _twilioInstance = new TwilioApi();
            string apiHierarchy = ConfigurationManager.AppSettings[NeeoConstants.VoiceApiHierarchy];

            if(string.IsNullOrWhiteSpace(apiHierarchy))
            {
                _primaryApi = _nexmoInstance;
                _secondaryApi = _twilioInstance;
            }
            else
            {
                string[] apiList = apiHierarchy.Split(new string[] {"-"}, 2, StringSplitOptions.RemoveEmptyEntries);

                if(apiList.Length == 2)
                {
                    if(apiList[0] == "nexmo")
                    {
                        _primaryApi = _nexmoInstance;
                    }

                    if (apiList[0] == "twilio")
                    {
                        _primaryApi = _twilioInstance;
                    }

                    if (apiList[1] == "nexmo")
                    {
                        _secondaryApi = _nexmoInstance;
                    }

                    if (apiList[1] == "twilio")
                    {
                        _secondaryApi = _twilioInstance;
                    }
                }
                else
                {
                    _primaryApi = _nexmoInstance;
                    _secondaryApi = _twilioInstance;
                }
            }
            

            _successorApi = _primaryApi;
            _primaryApi.SuccessorApi = _secondaryApi;
        }

        /// <summary>
        /// Gets the singleton instance of the respective class.
        /// </summary>
        /// <returns>singleton instance of the VoiceManager class.</returns>
        public static VoiceManager GetInstance()
        {
            return Instance;
        }

        #region Member Functions

        /// <summary>
        /// Calls to the given number for code.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <param name="msgBody">A string containing message body text.</param>
        /// <param name="isUnicode">if true, message body contains unicode text; otherwise false.(Optional)</param>
        public void Call(string phoneNumber, string code)
        {
            try
            {
                _successorApi.Call(phoneNumber, code);
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                _secondaryApi.Call(phoneNumber, code);
            }
        }

        #endregion
    }
}
