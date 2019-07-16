using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NotificationService
{
    /// <summary>
    /// Contains the signatures of  method/resources exposed by the Notification service.
    /// </summary>
    [ServiceContract]
    interface INeeoNotificationService
    {
        /// <summary>
        /// Sends push notifications with message and badge count to the user device specified with device token.
        /// </summary>
        /// <param name="nType">An integer containing specifying notification type.</param>
        /// <param name="dp">An integer containing specifying device platform.</param>
        /// <param name="dToken">A string containing the device token of the user device to whom message has to be delivered.</param>
        /// <param name="data">A dictionary having notification data.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/SendNotification", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        void SendNotification(short nType, short dp, string dToken, Dictionary<string, string> data);
    }
}
