using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.ComponentModel;
using System.Configuration;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
namespace PowerfulPal.Sms
{
    public class AmazonApi
    {
        public void sendSms(string phoneNumber,string messageBody,out string messageid,out string messagestatus)
        {

            string AWSAccessKeyId = ConfigurationManager.AppSettings["AWSAccessKeyId"].ToString();
            string AWSSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"].ToString();
            var sns = new AmazonSimpleNotificationServiceClient(AWSAccessKeyId, AWSSecretKey, Amazon.RegionEndpoint.USWest2);
            PublishResponse message = sns.Publish(new PublishRequest() { PhoneNumber = phoneNumber, Message = messageBody, Subject = "NeeoApp Activation" });
            messageid = message.MessageId;
            messagestatus = message.HttpStatusCode.ToString();
        }

        public void sendInvitationSms(string phoneNumber, string messageBody)
        {

            string AWSAccessKeyId = ConfigurationManager.AppSettings["AWSAccessKeyId"].ToString();
            string AWSSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"].ToString();
            var sns = new AmazonSimpleNotificationServiceClient(AWSAccessKeyId, AWSSecretKey, Amazon.RegionEndpoint.USWest2);
            PublishResponse message = sns.Publish(new PublishRequest() { PhoneNumber = phoneNumber, Message = messageBody, Subject = "NeeoApp Activation" });
           
        }

    }
}
