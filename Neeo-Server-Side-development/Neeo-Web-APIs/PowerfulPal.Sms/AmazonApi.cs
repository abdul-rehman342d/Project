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
using System.Threading;
namespace PowerfulPal.Sms
{
    public class AmazonApi
    {
        public void sendSms(string phoneNumber,string messageBody,out string messageid,out string messagestatus)
        {
            string AWSAccessKeyId = ConfigurationManager.AppSettings["AWSAccessKeyId"].ToString();
            string AWSSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"].ToString();
            var sns = new AmazonSimpleNotificationServiceClient(AWSAccessKeyId, AWSSecretKey, Amazon.RegionEndpoint.USEast1);


            var smsAttributes = new Dictionary<string, MessageAttributeValue>();

       
            MessageAttributeValue sMSType = new MessageAttributeValue();
            sMSType.DataType = "String";
            sMSType.StringValue = "Transactional";

    

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;


            smsAttributes.Add("AWS.SNS.SMS.SMSType", sMSType);
    

            PublishRequest publishRequest = new PublishRequest();
            publishRequest.Message = "This is 2nd sample message";
            publishRequest.MessageAttributes = smsAttributes;
            publishRequest.PhoneNumber = "received phone no with + and country code";

            PublishResponse message = sns.Publish(new PublishRequest() { PhoneNumber = phoneNumber, MessageAttributes= smsAttributes, Message = messageBody, Subject = "NeeoApp Activation" });
            messageid = message.MessageId;
            messagestatus = message.HttpStatusCode.ToString();
        }

    }
}
