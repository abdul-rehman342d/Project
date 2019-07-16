using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;

namespace SyncingService
{
    /// <summary>
    /// Contains the signatures of  method/resources exposed by the Syncing service.
    /// </summary>
    [ServiceContract]
    public interface ISyncingService
    {
        #region Iphone 1.x.x, Android 1.x.x
        /*They will be deprecated in future.*/

        /// <summary>
        /// Lookup the user's contacts on the server for existance.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="contacts">A array of type 'Contact' containing the contacts.</param>
        /// <returns>A list containing user's contacts those are application users.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/SyncUpContacts", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<ContactDetails> SyncUpContacts(string userID, Contact[] contacts);

        #endregion

        #region Iphone 2.0.0, Android x.x.x

        /// <summary>
        /// Lookup the user's contacts on the server for existance.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <param name="contacts">A array of type 'Contact' containing the contacts.</param>
        /// <returns>A list containing user's contacts those are application users.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/SyncContacts", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [ServiceKnownType(typeof(List<ContactState>))]
        [ServiceKnownType(typeof(List<ContactSubscriptionDetails>))]
        object SyncContacts(string uID, Contact[] contacts, bool? all);

        #endregion

        /// <summary>
        /// Gets the contact avatar timestamp on the file system.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <param name="contacts">A array of type 'Contact' containing the contacts.</param>
        /// <returns>A list containing user's contacts those are application users.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetContactsTS", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<ContactAvatarTimestamp> GetContactAvatarTimestamp(string uID, string contacts);

    }

}
