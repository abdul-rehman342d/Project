namespace Neeo.Nexmo
{
    public enum MessageStatus
    {
        /// <summary>
        /// The message was successfully accepted for delivery by Nexmo.
        /// </summary>
        Success = 0,
        /// <summary>
        /// You have exceeded the submission capacity allowed on this account, please back-off and retry.
        /// </summary>
        Throttled = 1,
        /// <summary>
        /// Your request is incomplete and missing some mandatory parameters.
        /// </summary>
        MissingParams = 2,
        /// <summary>
        /// The value of one or more parameters is invalid.
        /// </summary>
        InvalidParams = 3,
        /// <summary>
        /// The api_key / api_secret you supplied is either invalid or disabled.
        /// </summary>
        InvalidCredentials = 4,
        /// <summary>
        /// An error has occurred in the Nexmo platform whilst processing this message.
        /// </summary>
        InternalError = 5,
        /// <summary>
        /// The Nexmo platform was unable to process this message, for example, an un-recognized number prefix or the number is not whitelisted if your account is new.
        /// </summary>
        InvalidMessage = 6,
        /// <summary>
        /// The number you are trying to submit to is blacklisted and may not receive messages.
        /// </summary>
        NumberBarred = 7,
        /// <summary>
        /// The api_key you supplied is for an account that has been barred from submitting messages.
        /// </summary>
        PartnerAccountBarred = 8,
        /// <summary>
        /// Your pre-pay account does not have sufficient credit to process this message.
        /// </summary>
        PartnerQuotaExceeded = 9,
        /// <summary>
        /// This account is not provisioned for REST submission, you should use SMPP instead.
        /// </summary>
        AccountNotEnabledForRest = 11,
        /// <summary>
        /// Applies to Binary submissions, where the length of the UDH and the message body combined exceed 140 octets.
        /// </summary>
        MessageTooLong = 12,
        /// <summary>
        /// Message was not submitted because there was a communication failure.
        /// </summary>
        CommunicationFailed = 13,
        /// <summary>
        /// Message was not submitted due to a verification failure in the submitted signature.
        /// </summary>
        InvalidSignature = 14,
        /// <summary>
        /// The sender address (from parameter) was not allowed for this message. Restrictions may apply depending on the destination.
        /// </summary>
        InvalidSenderAddress = 15,
        /// <summary>
        /// The ttl parameter values is invalid.
        /// </summary>
        InvalidTTL = 16,
        /// <summary>
        /// Your request makes use of a facility that is not enabled on your account.
        /// </summary>
        FacilityNotAllowed = 19,
        /// <summary>
        /// The message class value supplied was out of range (0 - 3).
        /// </summary>
        InvalidMessageClass = 20
    }
}
