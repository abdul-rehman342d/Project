namespace PowerfulPal.Sms
{
    public abstract class SmsApi
    {
        /// <summary>
        /// Specifies Successor Sms Api.
        /// </summary>
        internal SmsApi SuccessorSmsApi;
        public abstract void SendSms(string[] phoneNumber,string msgBody, bool isUnicode = false);
    }
}
