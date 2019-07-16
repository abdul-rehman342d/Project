using System;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Common;
using DAL;
using LibNeeo.Plugin;
using Logger;


namespace LibNeeo
{
    /// <summary>
    /// Handles user related all functions including directory management, profile managemenet, updating device information and offline file sharing.
    /// </summary>
    public class User
    {
        #region Data Members

        /// <summary>
        /// Specifies the user id.
        /// </summary>
        private string _userID;
        /// <summary>
        /// Specifies the profile directory.
        /// </summary>
        private static string _dirProfile;
        /// <summary>
        /// Specifies the offline file directory.
        /// </summary>
        private static string _dirOffline;
        /// <summary>
        /// Specifies the album directory.
        /// </summary>
        private static string _dirAlbum;
        /// <summary>
        /// Specifies the root path to the application direcotry.
        /// </summary>
        private static string _rootPath;
        /// <summary>
        /// 
        /// </summary>

        #endregion

        #region Constructor

        static User()
        {
            _dirProfile = ConfigurationManager.AppSettings[NeeoConstants.UserProfileDirectory].ToString();
            _dirOffline = ConfigurationManager.AppSettings[NeeoConstants.UserOfflineDirectory].ToString();
            _dirAlbum = ConfigurationManager.AppSettings[NeeoConstants.UserAlbumDirectory].ToString();
            _rootPath = ConfigurationManager.AppSettings[NeeoConstants.RootPath].ToString();
        }

        public User(string userID)
        {
            _userID = userID;
        }

        #endregion

        #region Member Functions

        #region User Directory Structure

        /// <summary>
        /// Creates user's directories based on his/her phone number. It is a static method.
        /// </summary>
        /// <param name="phoneNumber">A string containing user's phone number.</param>
        public static void CreateUserDirectoryStructure(string phoneNumber)
        {

            string userDirectoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(phoneNumber));
            if (!Directory.Exists(userDirectoryPath))
            {
                DirectoryInfo dirInfo = Directory.CreateDirectory(userDirectoryPath);
                dirInfo.CreateSubdirectory(_dirProfile);
                dirInfo.CreateSubdirectory(_dirOffline);
                dirInfo.CreateSubdirectory(_dirAlbum);
            }
            else
            {
                if (!Directory.Exists(Path.Combine(userDirectoryPath, _dirProfile)))
                {
                    DirectoryInfo dirInfo = Directory.CreateDirectory(Path.Combine(userDirectoryPath, _dirProfile));
                }
                if (!Directory.Exists(Path.Combine(userDirectoryPath, _dirOffline)))
                {
                    DirectoryInfo dirInfo = Directory.CreateDirectory(Path.Combine(userDirectoryPath, _dirOffline));
                }
                if (!Directory.Exists(Path.Combine(userDirectoryPath, _dirAlbum)))
                {
                    DirectoryInfo dirInfo = Directory.CreateDirectory(Path.Combine(userDirectoryPath, _dirAlbum));
                }
            }
        }

        /// <summary>
        /// Deletes user's directories based on his/her phone number. It is a static method.
        /// </summary>
        /// <param name="phoneNumber">A string containing user's phone number.</param>
        public static void DeleteUserDirectoryStructure(string phoneNumber)
        {
            try
            {
                string userDirectoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(phoneNumber));
                if (Directory.Exists(userDirectoryPath))
                {
                    Directory.Delete(userDirectoryPath, true);
                }

            }
            catch (UnauthorizedAccessException unAuthExp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, unAuthExp.Message, unAuthExp);
            }
        }

        /// <summary>
        /// Deletes user's directories based on his/her phone number. It is an instance method.
        /// </summary>
        /// <param name="phoneNumber">A string containing user's phone number.</param>
        public void DeleteUserDirectoryStructure()
        {
            try
            {
                string userDirectoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID));
                if (Directory.Exists(userDirectoryPath))
                {
                    Directory.Delete(userDirectoryPath, true);
                }
            }
            catch (UnauthorizedAccessException unAuthExp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, unAuthExp.Message, unAuthExp);
            }
        }

        /// <summary>
        /// Determines the existance of user's directories based on his/her phone number. It is a static method.
        /// </summary>
        /// <param name="phoneNumber">A string containing user's phone number.</param>
        /// <returns>true if user's directories exists; otherwise false.</returns>
        public static bool UserDirectoryStructureExists(string phoneNumber)
        {
            string userDirectoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(phoneNumber));
            return Directory.Exists(userDirectoryPath);
        }

        #endregion

        #region User Device Token

        /// <summary>
        /// Updates user's device token in the database.
        /// </summary>
        /// <param name="deviceToken">A string containing the user's device token.</param>
        /// <returns>true if device token is successfully updated; otherwise false;</returns>
        public bool UpdateUserDeviceToken(string deviceToken)
        {
            DbManager dbManager = new DbManager();
            return dbManager.UpdateUserDeviceToken(_userID, deviceToken);
        }

        #endregion

        #region Profile

        /// <summary>
        /// Updates the display name of ther user in the database.
        /// </summary>
        /// <param name="name">A string containing user name.</param>
        /// <returns>true if user's display name is successfully updated; otherwise false;</returns>
        public bool UpdateUsersDisplayName(string name)
        {
            DbManager dbManager = new DbManager();
            return dbManager.UpdateUsersDisplayName(_userID, name);
        }

        /// <summary>
        /// Updates user's profile picture stores in the user's profile directory.
        /// </summary>
        /// <param name="fileData">A string containing base64-encoded file data.</param>
        /// <param name="senderID">A string containing sender user id. It is required only while uploading offline file.</param>
        /// <returns>true if user's profile picture is successfully updated; otherwise false. </returns>
        public string UploadImageFile(string fileData, FileClassfication fileClassfication, string senderID)
        {
            string result = null;
            string fileName = null;
            string directoryPath = null;
            switch (fileClassfication)
            {
                case FileClassfication.Profile:
                    fileName = NeeoUtility.ConvertToFileName(_userID);
                    directoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirProfile);
                    if (Directory.Exists(directoryPath))
                    {
                        result = File.SaveFile(fileName, fileData, directoryPath).ToString();
                    }
                    else
                    {
                        throw new ApplicationException(CustomHttpStatusCode.InvalidUser.ToString("D"));
                    }
                    break;
                case FileClassfication.Offline:
                    string timeStamp = NeeoUtility.GetTimeStamp(DateTime.UtcNow).ToString();
                    fileName = NeeoUtility.GetFileNameInOfflineFileFormat(senderID, _userID, timeStamp);
                    directoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirOffline);
                    if (Directory.Exists(directoryPath))
                    {
                        if (File.SaveFile(fileName, fileData, directoryPath))
                        {
                            result = NeeoUtility.GenerateImageUrl(_userID, timeStamp, fileClassfication, 0, senderID);
                        }
                    }
                    else
                    {
                        throw new ApplicationException(CustomHttpStatusCode.InvalidUser.ToString("D"));
                    }
                    break;
                case FileClassfication.Album:
                    fileName = NeeoUtility.GetTimeStamp(DateTime.UtcNow).ToString();
                    directoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirAlbum);
                    if (Directory.Exists(directoryPath))
                    {
                        result = File.SaveFile(NeeoUtility.ConvertToFileName(fileName), fileData, directoryPath).ToString();
                    }
                    else
                    {
                        throw new ApplicationException(CustomHttpStatusCode.InvalidUser.ToString("D"));
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Gets the path of the user's profile picture if it exists.
        /// </summary>
        /// <returns>path of the file if it exists; otherwise empty string will be returned.</returns>
        public string GetAvatar()
        {
            string fileName = NeeoUtility.ConvertToFileName(_userID);
            string userDirectoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirProfile);
            if (Directory.Exists(userDirectoryPath))
            {
                return File.GetFilePath(fileName, userDirectoryPath);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="avatarTimeStamp"></param>
        /// <param name="fileCreationTimeStamp"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public AvatarState GetAvatarState(ulong avatarTimeStamp, out ulong fileCreationTimeStamp, out string filePath)
        {
            string userDirectoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirProfile);
            if (Directory.Exists(userDirectoryPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(userDirectoryPath);
                FileInfo fileInfo = dirInfo.GetFiles().SingleOrDefault(x => x.Name == NeeoUtility.ConvertToFileName(_userID));
                if (fileInfo != null)
                {
                    fileCreationTimeStamp = NeeoUtility.GetTimeStamp(fileInfo.CreationTimeUtc);
                    filePath = fileInfo.FullName;

                    if (fileCreationTimeStamp == avatarTimeStamp)
                    {
                        return AvatarState.NotModified;
                    }
                    else
                    {
                        return AvatarState.Modified;
                    }
                }
                else
                {
                    fileCreationTimeStamp = 0;
                    filePath = null;
                    return AvatarState.NotExist;
                }
            }
            else
            {
                fileCreationTimeStamp = 0;
                filePath = null;
                return AvatarState.NotExist;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="avatarTimeStamp"></param>
        /// <returns></returns>
        public AvatarState GetAvatarState(ulong avatarTimeStamp)
        {
            string userDirectoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirProfile);
            if (Directory.Exists(userDirectoryPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(userDirectoryPath);
                FileInfo fileInfo = dirInfo.GetFiles().SingleOrDefault(x => x.Name == NeeoUtility.ConvertToFileName(_userID));
                if (fileInfo != null)
                {
                    if (NeeoUtility.GetTimeStamp(fileInfo.CreationTimeUtc) == avatarTimeStamp)
                    {
                        return AvatarState.NotModified;
                    }
                    else
                    {
                        return AvatarState.Modified;
                    }
                }
                else
                {
                    return AvatarState.NotExist;
                }
            }
            else
            {
                return AvatarState.NotExist;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileClassfication"></param>
        /// <returns></returns>
        public string GetFileState(string fileName, FileClassfication fileClassfication)
        {
            if (fileClassfication == FileClassfication.Offline)
            {
                string userDirectoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirOffline);
                if (Directory.Exists(userDirectoryPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(userDirectoryPath);
                    FileInfo fileInfo = dirInfo.GetFiles().SingleOrDefault(x => x.Name == fileName);
                    if (fileInfo != null)
                    {
                        return fileInfo.FullName;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else if (fileClassfication == FileClassfication.Album)
            {
                string userDirectoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirAlbum);
                if (Directory.Exists(userDirectoryPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(userDirectoryPath);
                    FileInfo fileInfo = dirInfo.GetFiles().SingleOrDefault(x => x.Name == fileName);
                    if (fileInfo != null)
                    {
                        return fileInfo.FullName;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }



        /// <summary>
        /// Deletes user's profile picture stored in the user profile directory. It is a static method.
        /// </summary>
        /// <param name="userID">A string containing user id.</param>
        /// <returns>true if user's profile picture is successfully deleted; otherwise false.</returns>
        public static bool DeleteFile(string userID, FileClassfication fileClassfication, string fileName)
        {
            bool result = false;
            string directoryPath = null;
            switch (fileClassfication)
            {
                case FileClassfication.Profile:
                    directoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(userID), _dirProfile);
                    break;

                case FileClassfication.Offline:
                    directoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(userID), _dirOffline);
                    break;

                case FileClassfication.Album:
                    directoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(userID), _dirAlbum);
                    break;
            }

            if (Directory.Exists(directoryPath))
            {
                result = File.DeleteFile(fileName, directoryPath);
            }
            else
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidUser.ToString("D"));
            }
            return result;
        }

        /// <summary>
        /// Deletes user's profile picture stored in the user profile directory. It is an instance method.
        /// </summary>
        /// <returns>true if user's profile picture is successfully deleted; otherwise false.</returns>
        public bool DeleteFile(FileClassfication fileClassfication, string fileName)
        {
            bool result = false;
            string directoryPath = null;
            switch (fileClassfication)
            {
                case FileClassfication.Profile:
                    directoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirProfile);
                    break;

                case FileClassfication.Offline:
                    directoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirOffline);
                    break;

                case FileClassfication.Album:
                    directoryPath = Path.Combine(_rootPath, NeeoUtility.GetDirectoryHierarchy(_userID), _dirAlbum);
                    break;
            }

            if (Directory.Exists(directoryPath))
            {
                result = File.DeleteFile(fileName, directoryPath);
            }
            else
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidUser.ToString("D"));
            }
            return result;
        }

        #endregion

        #region Offline File Sharing

        /// <summary>
        /// Uploads file to the File server.
        /// </summary>
        /// <param name="fileData">A string containing base64-encoded file data.</param>
        /// <returns>A string containing the file id of the currently uploaded file.</returns>
        public string UploadOfflineFile(string fileData)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Deletes file from the File Server.
        /// </summary>
        /// <param name="fileID">A string containing the file id.</param>
        /// <returns>true if the file with the specified file id is successfully deleted; otherwise false.</returns>
        public bool DeleteOfflineFile(string fileID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the path of the file specified with file id.
        /// </summary>
        /// <param name="fileID">A string containing the file id.</param>
        /// <returns>path of the file if it exists; otherwise empty string will be returned.</returns>
        public string GetOfflineFile(string fileID)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region User Roster State
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contacts"></param>
        /// <returns></returns>
        public List<ContactDetails> GetContactsRosterState(Contact[] contacts)
        {
            const string invalidUser = "-1";
            const string notExists = "0";
            const string exists = "1";
            const string delimeter = ",";

            string contactsString = string.Join(delimeter, contacts.Where(x => x.ContactPhoneNumber != null).Select(x => x.ContactPhoneNumber));
            List<ContactDetails> lstContact = new List<ContactDetails>();
            DataTable dtContactStatus = new DataTable();
            DbManager db = new DbManager();
            dtContactStatus = db.GetContactsExistanceAndRosterStatus(_userID, contactsString, delimeter);

            if (dtContactStatus.Rows.Count > 0)
            {
                foreach (DataRow dr in dtContactStatus.Rows)
                {
                    switch (dr["ContactRosterInfo"].ToString())
                    {
                        case invalidUser:
                        case notExists:
                        case exists:
                            ContactDetails contact = new ContactDetails();
                            contact.ContactPhoneNumber = dr["contact"].ToString();
                            contact.IsNeeoUser = Convert.ToBoolean(dr["isNeeoUser"]);
                            if (contact.IsNeeoUser == false)
                            {
                                contact.AvatarState = (int)AvatarState.NotExist;
                            }
                            else
                            {
                                ulong avatarTimeStamp =
                                    contacts.Where(x => x.ContactPhoneNumber == contact.ContactPhoneNumber)
                                        .Select(x => x.AvatarTimeStamp).FirstOrDefault();
                                User currentContact = new User(contact.ContactPhoneNumber);
                                contact.AvatarState = (ushort)currentContact.GetAvatarState(avatarTimeStamp);
                            }
                            contact.IsAlreadySubscribed = Convert.ToInt16(dr["contactRosterInfo"]);
                            lstContact.Add(contact);
                            break;

                        default:
                            UserService.UnSubContact(_userID, dr["contactRosterInfo"].ToString(), (RosterSubscription)dr["contactSubState"]);
                            break;
                    }
                }
            }
            return lstContact;
        }

        #endregion

        #endregion
    }
}
