using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neeo.Nexmo
{
    public static class NexmoDictionaries
    {
        public static Dictionary<short, string> MessageStatusDescriptionDictionary = new Dictionary<short, string>()
            {
                {0, "The message was successfully accepted for delivery by Nexmo."},
                {1, "You have exceeded the submission capacity allowed on this account, please back-off and retry."},
                {2, "Your request is incomplete and missing some mandatory parameters."},
                {3, "The value of one or more parameters is invalid."},
                {4, "The api_key / api_secret you supplied is either invalid or disabled."},
                {5, "An error has occurred in the Nexmo platform whilst processing this message."},
                {6, "The Nexmo platform was unable to process this message, for example, an un-recognized number prefix or the number is not whitelisted if your account is new."},
                {7, "The number you are trying to submit to is blacklisted and may not receive messages."},
                {8, "The api_key you supplied is for an account that has been barred from submitting messages."},
                {9, "Your pre-pay account does not have sufficient credit to process this message."},
                {11, "This account is not provisioned for REST submission, you should use SMPP instead."},
                {12, "Applies to Binary submissions, where the length of the UDH and the message body combined exceed 140 octets."},
                {13, "Message was not submitted because there was a communication failure."},
                {14, "Message was not submitted due to a verification failure in the submitted signature."},
                {15, " The sender address (from parameter) was not allowed for this message. Restrictions may apply depending on the destination."},
                {16, "The ttl parameter values is invalid."},
                {19, "Your request makes use of a facility that is not enabled on your account."},
                {20, "The message class value supplied was out of range (0 - 3)."}
            };
    }
}
