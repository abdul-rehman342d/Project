using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;
using LibNeeo;
using Logger;
using Newtonsoft.Json;

namespace SyncingService
{
    /// <summary>
    /// Gives implementation of the service resources/ objects defined in the interface <see cref="ISyncingService"/>.
    /// </summary>
    public class NeeoSyncingService : ISyncingService
    {
        private bool _logRequestResponse =
            Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);

        #region Contact Sycning

        #region Iphone 1.x.x, Android 1.x.x
        /*They will be deprecated in future.*/

        /// <summary>
        /// Lookup the user's contacts on the server for existance.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="contacts">A array of type 'Contact' containing the contacts.</param>
        /// <returns>A list containing user's contacts those are application users.</returns>
        public List<ContactDetails> SyncUpContacts(string userID, Contact[] contacts)
        {
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Request ===> User ID : " + userID + "- Contact List :" + JsonConvert.SerializeObject(contacts));
            }

            #endregion

            if (!NeeoUtility.IsNullOrEmpty(userID) && contacts != null)
            {
                if (contacts.Length > 0)
                {
                    NeeoUser user = new NeeoUser(userID);
                    try
                    {
                        var response = user.GetContactsRosterState(contacts, false, true);

                        #region log user request and response

                        /***********************************************
                         To log user response
                         ***********************************************/
                        if (_logRequestResponse)
                        {
                            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Response==> User ID : " + userID + "- Contact List :" + JsonConvert.SerializeObject(response));
                        }

                        #endregion

                        return response;
                    }
                    catch (ApplicationException appExp)
                    {
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode) (Convert.ToInt32(appExp.Message)));
                    }
                    catch (Exception exp)
                    {
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                    }
                }
                else
                {
                    return new List<ContactDetails>();
                }
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            return null;
        }

        #endregion

        #region Iphone 2.0.0, Android x.x.x

        /// <summary>
        /// Lookup the user's contacts on the server for existance. It is a wrapping method with short parameter name.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <param name="contacts">A array of type 'Contact' containing the contacts.</param>
        /// <returns>A list containing user's contacts those are application users.</returns>
        public object SyncContacts(string uID, Contact[] contacts, bool? all)
        {
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Request ===> User ID : " + uID + "- Contact List :" + JsonConvert.SerializeObject(contacts));
            }

            #endregion
            if (!NeeoUtility.IsNullOrEmpty(uID) && contacts != null)
            {
                bool getAll = all ?? true;
                if (contacts.Length > 0)
                {
                    NeeoUser user = new NeeoUser(uID);
                    try
                    {
                        var result = user.GetContactsRosterState(contacts, true, getAll);
                        

                        #region log user request and response

                        /***********************************************
                         To log user response
                         ***********************************************/
                        if (_logRequestResponse)
                        {
                            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Response==> User ID : " + uID + "- Contact List :" + JsonConvert.SerializeObject(result));
                        }

                        #endregion

                        if (getAll)
                        {
                            return result
                            .ConvertAll(
                                ContactDetailsToContactStateMapping);
                        }
                        else
                        {
                            return result
                                .ConvertAll(
                                ContactDetailsToContactSubscriptionMapping);
                        }
                    }
                    catch (ApplicationException appExp)
                    {
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode) (Convert.ToInt32(appExp.Message)));
                    }
                    catch (Exception exp)
                    {
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                    }
                }
                else
                {
                    return new List<ContactDetails>();
                }
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            return new List<ContactDetails>();
        }

        private static ContactState ContactDetailsToContactStateMapping(ContactDetails contactDetails)
        {
            return new ContactState()
            {
                Ph = contactDetails.ContactPhoneNumber,
                IsUsr = contactDetails.IsNeeoUser,
                IsSub = contactDetails.IsAlreadySubscribed
            };
        }

        private static ContactSubscriptionDetails ContactDetailsToContactSubscriptionMapping(ContactDetails contactDetails)
        {
            return new ContactSubscriptionDetails()
            {
                Ph = contactDetails.ContactPhoneNumber,
                Ts = contactDetails.AvatarTimestamp,
                IsSub = contactDetails.IsAlreadySubscribed
            };
        }

        #endregion


        #endregion

        #region Contact Avatar Timestamp

        /// <summary>
        /// Gets avatar timestamps details for the user's provided contacts.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <param name="contacts">A string containing the ',' separated contacts.</param>
        /// <returns>A list containing user's contacts avatar timestamp.</returns>
        public List<ContactAvatarTimestamp> GetContactAvatarTimestamp(string uID, string contacts)
        {
            #region log user request and response

            /***********************************************
                    To log user response
            ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Response==> User ID : " + uID + "- Contact List :" + contacts);
            }

            #endregion
            if (!NeeoUtility.IsNullOrEmpty(uID) && !NeeoUtility.IsNullOrEmpty(contacts))
            {
                NeeoUser user = new NeeoUser(uID.Trim());
                var result = user.GetContactsAvatarTimestamp(contacts);
                #region log user request and response

                    /***********************************************
                    To log user response
                    ***********************************************/
                    if (_logRequestResponse)
                    {
                        LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Response==> User ID : " + uID + "- Contact List :" + JsonConvert.SerializeObject(result));
                    }

                #endregion
                return result;
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            return null;
        }
        #endregion
        
    }
}
