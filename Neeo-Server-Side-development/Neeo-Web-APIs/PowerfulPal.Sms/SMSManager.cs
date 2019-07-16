using Common;
using Logger;
using System;
using System.Configuration;

namespace PowerfulPal.Sms
{
    /// <summary>
    /// Sends sms to the provided phone number.(Singleton)
    /// </summary>
    public sealed class SmsManager : SmsApi
    {
        #region Data Members
        /// <summary>
        /// 
        /// </summary>
        private static readonly SmsManager Instance = new SmsManager();

        /// <summary>
        /// 
        /// </summary>
        private readonly SmsApi _successorSmsApi;
        private readonly SmsApi _primaryApi;
        private readonly SmsApi _secondaryApi;
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
        public SmsApi PrimaryApi
        {
            get
            {
                return _primaryApi;
            }
        }
        /// <summary>
        /// Gets secondary api instance
        /// </summary>
        public SmsApi SecondaryApi
        {
            get
            {
                return _secondaryApi;
            }
        }
        /// <summary>
        /// Gets Nexmos api instance
        /// </summary>
        public SmsApi Nexmo
        {
            get
            {
                return _nexmoInstance;
            }
        }
        /// <summary>
        /// Gets Twilio api instance.
        /// </summary>
        public SmsApi Twilio
        {
            get
            {
                return _twilioInstance;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public SmsApi ExpertApi
        {
            get
            {
                return _expertApiInstance;
            }
        }
        #endregion

        /// <summary>
        /// Constructor of the singleton class.
        /// </summary>
        private SmsManager()
        {
            _nexmoInstance = new NexmoApi();
            _twilioInstance = new TwilioApi();
            string apiHierarchy = ConfigurationManager.AppSettings[NeeoConstants.SmsApiHierarchy];

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
            

            _successorSmsApi = _primaryApi;
            _primaryApi.SuccessorSmsApi = _secondaryApi;
        }

        /// <summary>
        /// Gets the singleton instance of the respective class.
        /// </summary>
        /// <returns>singleton instance of the SmsManager class.</returns>
        public static SmsManager GetInstance()
        {
            return Instance;
        }

        #region Member Functions

        /// <summary>
        /// Sends SMS to the given number.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <param name="msgBody">A string containing message body text.</param>
        /// <param name="isUnicode">if true, message body contains unicode text; otherwise false.(Optional)</param>
        /// <returns>true if SMS is successfully sent; otherwise, false.</returns>
        public override void SendSms(string[] phoneNumber, string msgBody, bool isUnicode = false)
        {
            try
            {
                _successorSmsApi.SendSms(phoneNumber, msgBody, isUnicode);
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                _secondaryApi.SendSms(phoneNumber, msgBody, isUnicode);
            }
        }

        #endregion
    }
}
