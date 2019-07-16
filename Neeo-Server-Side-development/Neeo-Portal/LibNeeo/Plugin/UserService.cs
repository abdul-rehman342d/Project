using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Configuration;
using RestSharp;
using Common;
using Logger;

namespace LibNeeo.Plugin
{
    /// <summary>
    /// A wrapper that interacts with user service plugin of the XMPP server.
    /// </summary>
    public static class UserService
    {
        #region Data Members

        private static string _adminKey;
        private static string _baseURL;
        //private static string _sslPort;
        //private static string _nonSslPort;
        private const string _userService = "/plugins/userService/userservice?type={type}&secret={secret}&username={username}";
        private const string _password = "&password={password}";
        private const string _name = "&name={name}";
        private const string _email = "&email={email}";
        private const string _itemJID = "&item_jid={item_jid}";
        private const string _subscription = "&subscription={subscription}";
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
            string actionType = "add";
            string url = _userService + _password + _name + _email;
            RestRequest request = new RestRequest(url);
            request.AddUrlSegment("type", actionType);
            request.AddUrlSegment("secret", _adminKey);
            request.AddUrlSegment("username", username);
            request.AddUrlSegment("password", password);
            request.AddUrlSegment("name", name);
            request.AddUrlSegment("email", email);

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
            string actionType = "update";
            string url = _userService;
            RestClient client = new RestClient(_baseURL);
            RestRequest request = SetupRequestWithBasicParameters(url, actionType, userID);

            if (password != null)
            {
                request.Resource = url + _password;
                request.AddUrlSegment("password", password);
            }

            if (name != null)
            {
                request.Resource = url + _name;
                request.AddUrlSegment("name", name);
            }

            if (email != null)
            {
                request.Resource = url + _email;
                request.AddUrlSegment("email", email);
            }

            ExecuteRequest(request);
        }


        /// <summary>
        /// Deletes the user from the XMPP server
        /// </summary>
        /// <param name="userID">A string containing the username.</param>
        public static void DeleteUser(string userID)
        {
            LoadVariables();
            string actionType = "delete";
            string url = _userService;
            RestClient client = new RestClient(_baseURL);
            RestRequest request = SetupRequestWithBasicParameters(url, actionType, userID);

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
            string actionType = "update_roster";
            string url = _userService;
            RestClient client = new RestClient(_baseURL);

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

            if (_baseURL == null)
            {
                _baseURL = ConfigurationManager.AppSettings[NeeoConstants.XmppBaseUrl].ToString();
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
            RestRequest request = new RestRequest(url + _itemJID + _subscription);
            request.AddUrlSegment("type", actionType);
            request.AddUrlSegment("secret", _adminKey);
            request.AddUrlSegment("username", userID);
            request.AddUrlSegment("item_jid", contactJid);
            request.AddUrlSegment("subscription", subscription.ToString("D"));
            return request;
        }

        /// <summary>
        /// Executes the request and manipulates the response.
        /// </summary>
        /// <param name="request">An object containing the request to be processed.</param>
        private static void ExecuteRequest(RestRequest request)
        {
            RestClient client = new RestClient(_baseURL);
            var response = client.Execute(request);
            if (response.ErrorException == null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response.Content);
                if (doc.DocumentElement.InnerText != "ok")
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, doc.DocumentElement.InnerText);
                    throw new ApplicationException(CustomHttpStatusCode.ServerError.ToString("D"));
                }
            }
            else
            {
                //log response.ErrorMessage
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.ErrorMessage);
                throw new ApplicationException(CustomHttpStatusCode.ServerConnectionError.ToString("D"));
            }
        }

        #endregion

        #endregion
    }
}