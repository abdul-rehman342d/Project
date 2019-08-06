using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.ComponentModel;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Common;
using Common.Models;
using DAL;
using LibNeeo.IO;
using LibNeeo.Plugin;
using LibNeeo.Network;
using LibNeeo.Voip;
using Logger;
using Newtonsoft.Json;
using Contact = Common.Contact;
using Directory = System.IO.Directory;
using LibNeeo.Model;
using PowerfulPal.Sms;

namespace LibNeeo
{
    /// <summary>
    /// Handles user related all functions including directory management, profile managemenet, updating device information and offline file sharing.
    /// </summary>
    public class NeeoUser
    {
        #region Data Members
        /// <summary>
        /// Specifies the file transfer supported version.
        /// </summary>
        private static string _fileTransferSupportedVersions;
        /// <summary>
        /// Specifies the invitation sender key.
        /// </summary>
        private static string _invitationSenderKey;
        /// <summary>
        /// Specifies the user id.
        /// </summary>
        private string _userID;
        /// <summary>
        /// 
        /// </summary>
        public string UserID
        {
            get
            {
                return _userID;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UserJid
        {
            get
            {
                return NeeoUtility.ConvertToJid(_userID);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Presence PresenceStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int OfflineMsgCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DeviceToken { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IMTone ImTone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CallingTone CallingTone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DevicePlatform DevicePlatform { get; set; }

        public PushNotificationSource PnSource { get; set; }

        #endregion

        #region Constructor

        public NeeoUser(string userID)
        {
            _userID = userID;
        }

        #endregion

        #region Member Functions

        #region User Device Token Management

        /// <summary>
        /// Updates user's device token in the xmpp database and on voip server.
        /// </summary>
        /// <param name="applicationVersion">A string containing the application version.</param>
        /// <param name="deviceInfo">An object of DeviceInfo class that contains the device inforamtion.</param>
        /// <param name="isUpdatingDToken">A boolean, if true,shows the user device token is going to update, otherwise false.</param>
        /// <returns>true if device token is successfully updated; otherwise false;</returns>
        public bool UpdateUserDeviceInfo(string applicationVersion, DeviceInfo deviceInfo, bool isUpdatingDToken)
        {
            DbManager dbManager = new DbManager();
            try
            {
                if (isUpdatingDToken)
                {
                    if (dbManager.StartTransaction())
                    {

                        if (dbManager.UpdateUserDeviceInfo(_userID, null, applicationVersion, deviceInfo, true, false))
                        {
                            //NeeoVoipApi.UpdateUserAccount(_userID, null,
                            //    deviceInfo.DeviceToken == "-1" ? PushEnabled.False : PushEnabled.True,
                            //    UserStatus.NotSpecified);

                            NeeoVoipApi.UpdateUserAccount(_userID, null,
                                PushEnabled.True,
                                UserStatus.NotSpecified);
                            dbManager.CommitTransaction();
                            return true;
                        }
                        throw new ApplicationException(CustomHttpStatusCode.InvalidUser.ToString("D"));
                    }
                    return false;
                }
                if (dbManager.UpdateUserDeviceInfo(_userID, null, applicationVersion, deviceInfo, false, false))
                {
                    return true;
                }
                throw new ApplicationException(CustomHttpStatusCode.InvalidUser.ToString("D"));
            }
            catch (ApplicationException applicationException)
            {
                if (isUpdatingDToken)
                {
                    dbManager.RollbackTransaction();
                }
                throw;
            }
            catch (Exception exception)
            {
                if (isUpdatingDToken)
                {
                    dbManager.RollbackTransaction();
                }
                throw;
            }
        }

        #endregion

        #region User Profile and Shared Files
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool UpdateUserProfile(string name, LibNeeo.IO.File file)
        {
            bool isCompleted = false;
            if (!UpdateUsersDisplayName(name))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidUser);
            }
            else if (file == null)
            {
                isCompleted = true;
            }
            else
            {

                file.Info = new NeeoFileInfo() { Name = _userID, Creator = _userID, MediaType = MediaType.Image, MimeType = MimeType.ImageJpeg };
   
                if (FileManager.Save(file, FileCategory.Profile))
                {
                    isCompleted = true;
                }
            }
            return isCompleted;
        }

        /// <summary>
        /// Updates the display name of ther user in the database.
        /// </summary>
        /// <param name="name">A string containing user name.</param>
        /// <returns>true if user's display name is successfully updated; otherwise false;</returns>
        internal bool UpdateUsersDisplayName(string name)
        {
            UserService.UpdateUser(_userID, null, name, null);
            return true;
        }

        /// <summary>
        /// Checks the avatar state of the user. It also gives back the avatar creation timestamp and avatar path.
        /// </summary>
        /// <param name="avatarTimeStamp">An unsigned long integer containing the provided avatar timestamp.</param>
        /// <param name="fileCreationTimeStamp">An unsigned long integer gives back the avatar creation timestamp.Use "out" keyword with it.</param>
        /// <param name="filePath">An string gives back the avatar path.Use "out" keyword with it.</param>
        /// <returns>It returns the avatar state.</returns>
        public AvatarState GetAvatarState(ulong avatarTimeStamp, out ulong fileCreationTimeStamp, out NeeoFileInfo filePath)
        {
            fileCreationTimeStamp = 0;
            filePath = null;
            var file = FileManager.GetFile(_userID, FileCategory.Profile, MediaType.Image);
            if (file != null)
            {
                fileCreationTimeStamp = NeeoUtility.GetTimeStamp(file.Info.CreationTimeUtc);
                filePath = file.Info;

                if (fileCreationTimeStamp == avatarTimeStamp)
                {
                    return AvatarState.NotModified;
                }
                return AvatarState.Modified;
            }
            return AvatarState.NotExist;
        }

        /// <summary>
        /// Gets the list of the avatar timestamp on the file system for the user's provided contacts.
        /// </summary>
        /// <param name="contacts">A string containing the ',' separated contacts.</param>
        /// <returns>A list containing user's contacts avatar timestamp.</returns>
        public List<ContactAvatarTimestamp> GetContactsAvatarTimestamp(string contacts)
        {
            char[] delimeter = new[] { ',' };
            ulong avatarTimestamp = 0;
            NeeoFileInfo fileInfo = null;
            List<ContactAvatarTimestamp> contactlList = new List<ContactAvatarTimestamp>();
            var contactsArray = contacts.Split(delimeter);
            if (contactsArray.Length > 0)
            {
                for (int i = 0; i < contactsArray.Length; i++)
                {
                    contactsArray[i] = contactsArray[i].Trim();
                    NeeoUser contact = new NeeoUser(contactsArray[i]);
                    AvatarState avatarState = contact.GetAvatarState(0, out avatarTimestamp, out fileInfo);
                    ContactAvatarTimestamp contactAvatarTimestamp = new ContactAvatarTimestamp()
                    {
                        Ph = contactsArray[i],
                        Ts = avatarTimestamp
                    };
                    contactlList.Add(contactAvatarTimestamp);
                    avatarTimestamp = 0;
                    fileInfo = null;
                }
            }
            return contactlList;
        }

        /// <summary>
        /// Gets the list of the avatar timestamp on the file system for the user's provided contacts.
        /// </summary>
        /// <param name="contacts">A string containing the ',' separated contacts.</param>
        /// <returns>A list containing user's contacts avatar timestamp.</returns>
        public List<ContactStatus> GetContactsAvatarTimestamp(SyncData syncData)
        {
            List<ContactStatus> contactList = new List<ContactStatus>();
            if (syncData.ContactsList.Count > 0)
            {
                foreach (var contact in syncData.ContactsList)
                {
                    ulong avatarTimestamp = 0;
                    NeeoFileInfo avatarInfo = null;
                    var user = new NeeoUser(contact.PhoneNumber);
                    user.GetAvatarState(0, out avatarTimestamp, out avatarInfo);
                    var contactStatus = new ContactStatus()
                    {
                        Contact = contact,
                        AvatarTimestamp = avatarTimestamp
                    };
                    contactList.Add(contactStatus);
                }
            }
            return contactList;
        }

        /// <summary>
        /// Deletes user's profile picture stored in the user profile directory. It is a static method.
        /// </summary>
        /// <param name="userID">A string containing user id.</param>
        /// <returns>true if user's profile picture is successfully deleted; otherwise false.</returns>
        public static bool DeleteUserAvatar(string userID)
        {
            return FileManager.Delete(userID, FileCategory.Profile);
        }

        /// <summary>
        /// Deletes user's profile picture stored in the user profile directory. It is an instance method.
        /// </summary>
        /// <returns>true if user's profile picture is successfully deleted; otherwise false.</returns>
        public bool DeleteUserAvatar()
        {
            return FileManager.Delete(_userID, FileCategory.Profile);
        }

        /// <summary>
        /// Gets user's profile name.
        /// </summary>
        /// <returns>A string containing the profile name of user. </returns>
        public string GetUserProfileName()
        {
            DbManager dbManager = new DbManager();
            return (dbManager.GetUserProfileName(_userID));
        }

        /// <summary>
        /// Gets the user's avatar base on the file path of avatar.
        /// </summary>
        /// <returns>It returns Avatar if exists.</returns>
        public Stream GetAvatarStream()
        {
            LibNeeo.IO.File file = FileManager.GetFile(_userID, FileCategory.Profile, MediaType.Image);
            if (file != null)
            {
                return (LibNeeo.IO.File.GetStream(file.Info.FullPath));
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverID"></param>
        /// <returns></returns>
        public bool IsFileTransferSupported(string receiverID)
        {
            _fileTransferSupportedVersions = _fileTransferSupportedVersions == null ? ConfigurationManager.AppSettings[NeeoConstants.FileTransferSupportedVersions].ToString() : _fileTransferSupportedVersions;
            DbManager dbManager = new DbManager();
            string receiverClientAppVersion = dbManager.GetClientAppVersion(_userID, receiverID);
            return _fileTransferSupportedVersions.Contains(receiverClientAppVersion.Trim());
        }

        #endregion

        #region User Roster State Management

        /// <summary>
        /// Gets the contacts state in the user roster for the provided contacts.
        /// </summary>
        /// <param name="contacts">An array of contacts whose state has to be checked in the user roster.</param>
        /// <param name="isUpdatedAppCall">A bool if true tells to use the updated code; otherwise false.</param>
        /// <returns>A list containing the current state of the provided contacts.</returns>
        public List<ContactDetails> GetContactsRosterState(Contact[] contacts, bool isUpdatedAppCall, bool getAll)
        {
            const string invalidUser = "-1";
            const string notExists = "0";
            const string exists = "1";
            const string delimeter = ",";
            string contactsString = null;
            if (isUpdatedAppCall)
            {
                contactsString = string.Join(delimeter, contacts.Where(x => x.Ph != null).Select(x => x.Ph.Trim()));
            }
            else
            {
                contactsString = string.Join(delimeter, contacts.Where(x => x.ContactPhoneNumber != null).Select(x => x.ContactPhoneNumber.Trim()));
            }

            List<ContactDetails> lstContact = new List<ContactDetails>();
            DataTable dtContactStatus = new DataTable();
            DbManager db = new DbManager();
            dtContactStatus = db.GetContactsExistanceAndRosterStatus(_userID, contactsString, delimeter);

            if (dtContactStatus.Rows.Count > 0)
            {
                foreach (DataRow dr in dtContactStatus.Rows)
                {
                    if (getAll)
                    {
                        switch (dr["ContactRosterInfo"].ToString())
                        {
                            case invalidUser:
                                ContactDetails contact = new ContactDetails();
                                contact.ContactPhoneNumber = dr["contact"].ToString();
                                contact.IsNeeoUser = Convert.ToBoolean(dr["isNeeoUser"]);
                                contact.AvatarState = (ushort)AvatarState.NotExist;
                                contact.IsAlreadySubscribed = Convert.ToInt16(dr["contactRosterInfo"]);
                                lstContact.Add(contact);
                                break;
                            case notExists:
                            case exists:
                                ulong avatarTimestamp = 0;
                                NeeoFileInfo avatarInfo = null;
                                contact = new ContactDetails();
                                contact.ContactPhoneNumber = dr["contact"].ToString();
                                contact.IsNeeoUser = Convert.ToBoolean(dr["isNeeoUser"]);
                                NeeoUser contactUser = new NeeoUser(contact.ContactPhoneNumber);
                                contact.AvatarState = (ushort)contactUser.GetAvatarState(0, out avatarTimestamp, out avatarInfo);
                                contact.AvatarTimestamp = avatarTimestamp;
                                contact.IsAlreadySubscribed = Convert.ToInt16(dr["contactRosterInfo"]);
                                lstContact.Add(contact);
                                break;

                            default:
                                UserService.UnSubContact(_userID, dr["contactRosterInfo"].ToString(),
                                    (RosterSubscription)dr["contactSubState"]);
                                break;
                        }
                    }
                    else
                    {
                        switch (dr["ContactRosterInfo"].ToString())
                        {
                            case invalidUser:
                            case notExists:
                                //Do nothing
                                break;
                            case exists:
                                ulong avatarTimestamp = 0;
                                NeeoFileInfo avatarInfo = null;
                                ContactDetails contact = new ContactDetails();
                                contact.ContactPhoneNumber = dr["contact"].ToString();
                                NeeoUser contactUser = new NeeoUser(contact.ContactPhoneNumber);
                                contact.AvatarState = (ushort)contactUser.GetAvatarState(0, out avatarTimestamp, out avatarInfo);
                                contact.AvatarTimestamp = avatarTimestamp;
                                contact.IsAlreadySubscribed = Convert.ToInt16(dr["contactRosterInfo"]);
                                lstContact.Add(contact);
                                break;

                            default:
                                UserService.UnSubContact(_userID, dr["contactRosterInfo"].ToString(),
                                    (RosterSubscription)dr["contactSubState"]);
                                break;
                        }
                    }
                }
            }
            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "User ID : " + _userID + "- Contact List :" + JsonConvert.SerializeObject(lstContact));
            return lstContact;
        }

        /// <summary>
        /// Gets the contacts state in the user roster for the provided contacts.
        /// </summary>
        /// <param name="contacts">An array of contacts whose state has to be checked in the user roster.</param>
        /// <param name="isUpdatedAppCall">A bool if true tells to use the updated code; otherwise false.</param>
        /// <returns>A list containing the current state of the provided contacts.</returns>
        public List<ContactStatus> GetContactsState(SyncData syncData)
        {
            const string invalidUser = "-1";
            const string notExists = "0";
            const string exists = "1";
            const string delimeter = ",";
            string contactsString = null;

            contactsString = string.Join(delimeter, syncData.ContactsList.Where(x => x.PhoneNumber != null).Select(x => x.PhoneNumber.Trim()));
            var ContactList = new List<ContactStatus>();
            var dtContactStatus = new DataTable();
            var dbManager = new DbManager();
            dtContactStatus = dbManager.GetContactsExistanceAndRosterStatus(_userID, contactsString, delimeter);

            if (dtContactStatus.Rows.Count > 0)
            {
                foreach (DataRow dr in dtContactStatus.Rows)
                {
                    if (!syncData.Filtered)
                    {
                        switch (dr["ContactRosterInfo"].ToString())
                        {
                            case invalidUser:
                            case notExists:
                            case exists:
                                var contactStatus = new ContactStatus();
                                contactStatus.Contact = new Common.Models.Contact()
                                {
                                    PhoneNumber = dr["contact"].ToString()
                                };
                                contactStatus.IsNeeoUser = Convert.ToBoolean(dr["isNeeoUser"]);
                                contactStatus.IsAlreadySubscribed = Convert.ToInt16(dr["contactRosterInfo"]);
                                ContactList.Add(contactStatus);
                                break;

                            default:
                                UserService.UnSubContact(_userID, dr["contactRosterInfo"].ToString(),
                                    (RosterSubscription)dr["contactSubState"]);
                                break;
                        }
                    }
                    else
                    {
                        switch (dr["ContactRosterInfo"].ToString())
                        {
                            case invalidUser:
                            case notExists:
                            case exists:
                                if (Convert.ToBoolean(dr["isNeeoUser"]))
                                {
                                    ulong avatarTimestamp = 0;
                                    NeeoFileInfo avatarInfo = null;
                                    var contactStatus = new ContactStatus();
                                    contactStatus.Contact = new Common.Models.Contact()
                                    {
                                        PhoneNumber = dr["contact"].ToString()
                                    };
                                    var user = new NeeoUser(contactStatus.Contact.PhoneNumber);
                                    user.GetAvatarState(0, out avatarTimestamp, out avatarInfo);
                                    contactStatus.AvatarTimestamp = avatarTimestamp;
                                    contactStatus.IsAlreadySubscribed = Convert.ToInt16(dr["contactRosterInfo"]);
                                    ContactList.Add(contactStatus);
                                }
                                break;

                            default:
                                UserService.UnSubContact(_userID, dr["contactRosterInfo"].ToString(),
                                    (RosterSubscription)dr["contactSubState"]);
                                break;
                        }
                    }
                }
            }
            LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "User ID : " + _userID + "- Contact List :" + JsonConvert.SerializeObject(ContactList));
            return ContactList;
        }

        #endregion

        #region User Settings
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tone"></param>
        /// <returns></returns>
        public bool UpdateSettings(Enum tone, ToneType toneType)
        {
            DbManager db = new DbManager();
            if (db.UpdateSettings(_userID, tone, toneType))
            {
                return true;
            }
            else
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidNumber.ToString("D"));
            }
        }

        #endregion

        #region Application Invitation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uName"></param>
        /// <param name="contacts"></param>
        public void SendInvitation(string uName, string contacts, string languageCode)
        {

            string invitationText;
            string[] delimeter = { "," };
            invitationText = InvitationMessage.GetLocalizedMessage(languageCode);
            _invitationSenderKey = _invitationSenderKey ?? ConfigurationManager.AppSettings[NeeoConstants.InvitationSenderKey];
            string[] contactsArray = contacts.Split(delimeter, StringSplitOptions.None);
            ulong temp = 0;
            DbManager dbManager = new DbManager();
            string profileName = dbManager.GetUserProfileName(_userID);
            string msgBody = invitationText.Replace(_invitationSenderKey,
                (NeeoUtility.IsNullOrEmpty(uName)
                    ? (NeeoUtility.IsNullOrEmpty(profileName)
                        ? "(" + NeeoUtility.FormatAsIntlPhoneNumber(_userID) + ")"
                        : (profileName + " (" + NeeoUtility.FormatAsIntlPhoneNumber(_userID) + ")"))
                    : (uName + " (" + NeeoUtility.FormatAsIntlPhoneNumber(_userID) + ")")));



            string messageid;
            string messagestatus;
            AmazonApi amazonInstant = new AmazonApi();

            for (int i = 0; i < contactsArray.Length; i++)
            {
                if (ulong.TryParse(contactsArray[i], out temp))
                {
                    try
                    {
                        //SmsManager.SendThroughSecondaryApi(NeeoUtility.FormatAsIntlPhoneNumber(contactsArray[i]),msgBody.Replace("!", Environment.NewLine));
                        //PowerfulPal.Sms.SmsManager.GetInstance().Twilio.SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(contactsArray[i]) }, msgBody.Replace("!", Environment.NewLine), languageCode != "en");
                        SmsManager.SendThroughAmazon(contactsArray[i], msgBody.Replace("!", Environment.NewLine).ToString(), false,2);
                    }
                    catch (ApplicationException appEx)
                    {
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + " ===> uid : " +
                            _userID + " ===> contact : " + contactsArray[i] + " is invalid.");
                    }
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + " ===> uid : " +
                        _userID + " ===> contact : " + contactsArray[i] + " is invalid.");
                }
            }
        }

        #endregion

        #region Contact Capabilities

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetContactsAppVersion(string[] contacts)
        {
            ulong temp = 0;
            const string delimeter = ",";
            string contactString = null;
            DbManager dbManager = new DbManager();
            if (contacts.Length > 0)
            {
                var contactsList =
                    contacts.Select(item => item).Where(e => ulong.TryParse(e, out temp) == true).ToList();
                if (contactsList.Any())
                {
                    DataTable dtContactsAppVersions = dbManager.GetAppVersion(_userID,
                        contactsList.Aggregate((current, next) => current + delimeter + next));
                    if (dtContactsAppVersions.Rows.Count > 0)
                    {
                        return dtContactsAppVersions.AsEnumerable().ToDictionary<DataRow, string, string>(row => row.Field<string>(0),
                                row => row.Field<string>(1));
                    }

                    else
                    {
                        throw new ApplicationException(HttpStatusCode.NotFound.ToString("D"));
                    }
                }
                else
                {
                    throw new ApplicationException(HttpStatusCode.BadRequest.ToString("D"));
                }
            }
            else
            {
                throw new ApplicationException(HttpStatusCode.BadRequest.ToString("D"));
            }
        }

        #endregion

        #region User Verification

        /// <summary>
        /// This Method confirms whether the old hash code and newly genrated hash code are same or not.
        /// </summary>
        /// <param name="oldHash">It is old hash code of the user</param>
        /// <returns>Returns true if hash codes are same else returns false.</returns>
        public bool VerifyUser(string oldHash)
        {
            DbManager dbManager = new DbManager();
            DataTable dtUserInformation = dbManager.GetUserInformation(_userID);
            if (dtUserInformation != null)
            {
                string hashingData1 = dtUserInformation.Rows[0]["deviceVenderID"].ToString() +
                                     dtUserInformation.Rows[0]["applicationID"].ToString() + _userID;
                string hashingData2 = _userID + dtUserInformation.Rows[0]["deviceVenderID"].ToString() +
                                     dtUserInformation.Rows[0]["applicationID"].ToString();

                string newHash1 = NeeoUtility.GenerateMd5Hash(hashingData1);
                string newHash2 = NeeoUtility.GenerateMd5Hash(hashingData2);
                if (newHash1 == oldHash || newHash2 == oldHash)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            #region Commentted implementation
            //            string hashingData;
            //            DbManager dbManager = new DbManager();
            //            DataTable dtUserInformation = dbManager.GetUserInformation(_userID);
            //            if (dtUserInformation != null)
            //            {
            //                if (dtUserInformation.Rows[0]["appVersion"].ToString() == "NEEO-3.0.1")
            //                {
            //                    hashingData = _userID + dtUserInformation.Rows[0]

            //["deviceVenderID"].ToString() +
            //                                     dtUserInformation.Rows[0]["applicationID"].ToString();
            //                }
            //                else
            //                {
            //                    hashingData = dtUserInformation.Rows[0]["deviceVenderID"].ToString() +
            //                                         dtUserInformation.Rows[0]["applicationID"].ToString

            //() + _userID;

            //                }
            //                if (hashingData != null)
            //                {
            //                    string newHash = NeeoUtility.GenerateMd5Hash(hashingData);
            //                    if (newHash == oldHash)
            //                    {
            //                        return true;
            //                    }
            //                    else
            //                    {
            //                        return false;
            //                    }
            //                }
            //                else
            //                {
            //                    return false;
            //                }
            //            }
            //            else
            //            {
            //                return false;
            //            }

            #endregion
        }

        #endregion

        #region Presence

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstParticipant"></param>
        public static Presence GetPresence(string userID)
        {
            return UserPresence.GetUserPresence(NeeoUtility.ConvertToJid(userID));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstParticipant"></param>
        public void GetPresence()
        {
            this.PresenceStatus = UserPresence.GetUserPresence(NeeoUtility.ConvertToJid(_userID));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstParticipant"></param>
        public async Task GetPresenceAsync()
        {
            this.PresenceStatus = await Task.Factory.StartNew(() => UserPresence.GetUserPresence(NeeoUtility.ConvertToJid(_userID)));
        }

        #endregion

        #region Offline Message Count

        /// <summary>
        /// Resets the user's offline message count. It is an instance method.
        /// </summary>
        /// <returns>true, if count is successfully reset; otherwise false</returns>
        public bool ResetOfflineMessageCount()
        {
            DbManager dbManager = new DbManager();
            return dbManager.ResetOfflineCount(_userID, OfflineCount.Offline);
        }

        /// <summary>
        /// Resets the user's offline message count.
        /// </summary>
        /// <returns>true, if count is successfully reset; otherwise false</returns>
        public static bool ResetOfflineMessageCount(string uid)
        {
            DbManager dbManager = new DbManager();
            return dbManager.ResetOfflineCount(uid, OfflineCount.Offline);
        }

        #endregion

        #region MCR Details

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="flush"></param>
        /// <returns></returns>
        public static McrData GetMcrDetails(string userID, bool flush)
        {
            var mcrDetails = NeeoVoipApi.GetMcrDetails(userID, flush);

            if (flush == true)
            {
                DbManager dbManager = new DbManager();
                if (!dbManager.ResetOfflineCount(userID, OfflineCount.MCR))
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, " ==> " + userID + " : Mcr count reset operation failed.", System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return mcrDetails;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flush"></param>
        /// <returns></returns>
        public McrData GetMcrDetails(bool flush)
        {
            var mcrDetails = NeeoVoipApi.GetMcrDetails(_userID, flush);
            if (flush == true)
            {
                DbManager dbManager = new DbManager();
                if (!dbManager.ResetOfflineCount(_userID, OfflineCount.MCR))
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, " ==> " + _userID + " : Mcr count reset operation failed.", System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return mcrDetails;
        }

        #endregion

        #region

        public async Task<Dictionary<string, object>> GetLastSeenTimeAsync()
        {
            DbManager dbManager = new DbManager();
            return await dbManager.GetUserLastSeenTimeAsync(_userID, NeeoConstants.ResourceName).ConfigureAwait(false);
        }
        #endregion

        #endregion

        #region UpdateUserPersonalData
        private DbManager _dbManager = new DbManager();
        public async Task<bool> UpdateUserPersonalData(UserPersonalData currentUser)
        {
            
            return await System.Threading.Tasks.Task.Run(() => _dbManager.UpdateUserPersonalData(currentUser.username, currentUser.dateOfBirth, currentUser.interest, currentUser.gender,currentUser.country));
        }


        #endregion


        #region ChatBackUp

        public async Task<bool> CreateXMLChatBackup(string sender, string messagesXml)
        {
            return await System.Threading.Tasks.Task.Run(() => _dbManager.CreateXMLChatBackup(sender, messagesXml));
        }

        public async Task<object> GetXMLChatBackup(string sender)
        {
            return await System.Threading.Tasks.Task.Run(() => _dbManager.GetXMLChatBackup(sender));
        }


        #endregion


    }
}
