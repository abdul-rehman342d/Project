using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Configuration;
using RestSharp;
using Common;
using Logger;
using System.Xml.Linq;
namespace LibNeeo.Plugin
{
    /// <summary>
    /// A wrapper that interacts with user service plugin of the XMPP server.
    /// </summary>
    public static class UserService
    {
        #region Data Members

        private static string _adminKey;
        private static string _baseUrl;
        //private static string _sslPort;
        //private static string _nonSslPort;
        private const string UserServiceUrl = "/plugins/userService/users";
        private const string Password = "&password={password}";
        private const string Name = "&name={name}";
        private const string Email = "&email={email}";
        private const string ItemJid = "&item_jid={item_jid}";
        private const string Subscription = "&subscription={subscription}";

        #endregion

        #region Member Functions

        #region User Related Methods

        /// <summary>
        /// Adds a user on the XMPP server
        /// </summary>
        /// <param name="username">A string containing the username for user registration.</param>
        /// <param name="password">A string containing the password for account.</param>
        /// <param name="name">A string containing the name for the account.</param>
        /// <param name="email">A string containing the user's email id.</param>
        public static void AddUser(string username, string password, string name, string email)
        {
            LoadVariables();
            RestRequest request = new RestRequest(UserServiceUrl);
            request.AddHeader("Content-Type", "application/xml");
            request.AddHeader("Authorization", _adminKey);
            request.Method = Method.POST;
            request.RequestFormat = DataFormat.Xml;
            string RequestBody = string.Format(@"<?xml version='1.0' encoding='UTF-8' standalone='yes'?> <user><username>{0}</username><password>{1}</password><name>{2}</name><email>{3}</email></user>", username, password, name, email);
            request.AddParameter("application/xml", RequestBody, ParameterType.RequestBody);
            ExecuteRequest(request);
        }

        /// <summary>
        /// Updates an existing user on the XMPP server.
        /// </summary>
        /// <param name="userID">A string containing the username.</param>
        /// <param name="password">[Optional].A string containing the password. Pass null if not required. </param>
        /// <param name="name">[Optional].A string containing the name for the account. Pass null if not required.</param>
        /// <param name="email">[Optional].A string containing the user's email id. Pass null if not required.</param>
        public static void UpdateUser(string userID, string password, string name, string email)
        {
            LoadVariables();

            string url = UserServiceUrl + "/" + userID;
            RestRequest request = new RestRequest(url);
            request.Method = Method.PUT;
            request.AddHeader("Content-Type", "application/xml");
            request.AddHeader("Authorization", _adminKey);
            string RequestBody = string.Format(@"<?xml version='1.0' encoding='UTF-8' standalone='yes'?> <user>");

            if (NeeoUtility.IsNullOrEmpty(userID))
            {
                throw new ApplicationException(HttpStatusCode.BadRequest.ToString("D"));
            }

            RequestBody = RequestBody + "<username>" + userID + "</username>";

            if (password != null)
            {
                RequestBody = RequestBody + "<password>" + password + "</password>";
            }

            if (name != null)
            {
                RequestBody = RequestBody + "<name>" + name + "</name>";
            }

            if (email != null)
            {
                RequestBody = RequestBody + "<email>" + email + "</email>";
            }
            RequestBody = RequestBody + "</user>";
            request.AddParameter("application/xml", RequestBody, ParameterType.RequestBody);

            ExecuteRequest(request);
        }


        /// <summary>
        /// Deletes the user from the XMPP server
        /// </summary>
        /// <param name="userID">A string containing the username.</param>
        public static void DeleteUser(string userID)
        {
            LoadVariables();
            string url = UserServiceUrl + "/" + userID;
            RestRequest request = new RestRequest(url);
            request.AddHeader("Authorization", _adminKey);
            request.Method = Method.DELETE;
            ExecuteRequest(request);
        }

        #endregion

        #region Roster Related Methods
        /// <summary>
        /// Unsubscrip the contact based on its current subscription state.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="contactID">A string containing contact id, who is going to be unsubscribed from ther user roster.</param>
        /// <param name="currentSubscription">An enum specifying the current subscription state of the contact.</param>
        public static void UnSubContact(string userID, string contactID, RosterSubscription currentSubscription)
        {
            LoadVariables();
            const string actionType = "update_roster";
            string url = UserServiceUrl;
            RestClient client = new RestClient(_baseUrl);

            if (currentSubscription == RosterSubscription.Both)
            {
                RestRequest userRequest = SetupRequestWithRosterParameters(url, actionType, userID, NeeoUtility.ConvertToJid(contactID), RosterSubscription.From);
                RestRequest contactRequest = SetupRequestWithRosterParameters(url, actionType, contactID, NeeoUtility.ConvertToJid(userID), RosterSubscription.To);

                ExecuteRequest(userRequest);
                ExecuteRequest(contactRequest);
            }
            else
            {
                RestRequest userRequest = SetupRequestWithRosterParameters(url, actionType, userID, NeeoUtility.ConvertToJid(contactID), RosterSubscription.None);
                RestRequest contactRequest = SetupRequestWithRosterParameters(url, actionType, contactID, NeeoUtility.ConvertToJid(userID), RosterSubscription.None);

                ExecuteRequest(userRequest);
                ExecuteRequest(contactRequest);
            }
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Loads the required variables for further processing.
        /// </summary>
        private static void LoadVariables()
        {
            if (_adminKey == null)
            {
                _adminKey = ConfigurationManager.AppSettings[NeeoConstants.AdminKey].ToString();
            }

            if (_baseUrl == null)
            {
                _baseUrl = ConfigurationManager.AppSettings[NeeoConstants.XmppBaseUrl].ToString();
            }

            //_sslPort = ConfigurationManager.AppSettings[NeeoConstants.sslPort].ToString();
            //_nonSslPort = ConfigurationManager.AppSettings[NeeoConstants.nonSslPort].ToString();
        }

        /// <summary>
        /// Sets up request with basic parameter.
        /// </summary>
        /// <param name="url">A string containing the server url.</param>
        /// <param name="actionType">A string specifying the action to be performed.</param>
        /// <param name="userID">A string containing the user id of the user.</param>
        /// <returns>A request object to be processed based on the given parameters.</returns>
        private static RestRequest SetupRequestWithBasicParameters(string url, string actionType, string userID)
        {
            RestRequest request = new RestRequest(url);
            request.AddUrlSegment("type", actionType);
            request.AddUrlSegment("secret", _adminKey);
            request.AddUrlSegment("username", userID);
            return request;
        }

        /// <summary>
        /// Sets up request with parameters required for roster manipulation.
        /// </summary>
        /// <param name="url">A string containing the server url.</param>
        /// <param name="actionType">A string specifying the action to be performed.</param>
        /// <param name="userID">A string containing the user id of the user.</param>
        /// <param name="contactJid">A string containing the contact's jid whose subscription has to be modified in the user's roster. </param>
        /// <param name="subscription">An enum specifying the subscription to be set across the given contact <paramref name="contactJid"/></param>
        /// <returns>A request object to be processed based on the given parameters.</returns>
        private static RestRequest SetupRequestWithRosterParameters(string url, string actionType, string userID, string contactJid, RosterSubscription subscription)
        {
            RestRequest request = new RestRequest(url + "/" + userID + "/roster/" + contactJid);
            request.Method = Method.PUT;
            request.AddHeader("Content-Type", "application/xml");
            request.AddHeader("Authorization", _adminKey);
            string RequestBody = string.Format(@"<?xml version='1.0' encoding='UTF-8' standalone='yes'?> <rosterItem> <jid>{0}</jid><subscriptionType>{1}</subscriptionType></rosterItem>",contactJid, (int) subscription);
            request.AddParameter("application/xml", RequestBody, ParameterType.RequestBody);
            return request;
        }

        /// <summary>
        /// Executes the request and manipulates the response.
        /// </summary>
        /// <param name="request">An object containing the request to be processed.</param>
        private static void ExecuteRequest(RestRequest request)
        {
            RestClient client = new RestClient(_baseUrl);
            var response = client.Execute(request);
            if (response.ErrorException == null)
            {
                if (!(response.StatusCode == HttpStatusCode.Forbidden ||
                      response.StatusCode == HttpStatusCode.Unauthorized) || response.StatusCode == HttpStatusCode.BadRequest)
                {

                    if (!(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(response.Content);
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            doc.DocumentElement.InnerText);
                        if (doc.DocumentElement.InnerText.Contains(UserServiceStatus.UserNotFoundException))
                        {
                            throw new ApplicationException(CustomHttpStatusCode.InvalidUser.ToString("D"));
                        }
                        else
                        {
                            throw new ApplicationException(CustomHttpStatusCode.ServerInternalError.ToString("D"));
                        }


                    }
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        response.StatusCode.ToString());
                    throw new ApplicationException(CustomHttpStatusCode.ServerInternalError.ToString("D"));
                }
            }
            else
            {
                //log response.ErrorMessage
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.ErrorMessage);
                throw new ApplicationException(CustomHttpStatusCode.ServerConnectionError.ToString("D"));
            }
        }

        #endregion

        #endregion
    }
}