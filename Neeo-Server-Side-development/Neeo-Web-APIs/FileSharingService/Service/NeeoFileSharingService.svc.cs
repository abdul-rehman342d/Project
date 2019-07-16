using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using LibNeeo;
using LibNeeo.IO;
using LibNeeo.MediaSharing;
using LibNeeo.MUC;
using LibNeeo.Url;
using Logger;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.Transactions;
using System.Web;

namespace FileSharingService
{
    /// <summary>
    /// Gives implementation of the service resources/ objects defined in the interface <see cref="INeeoFileSharingService"/>.
    /// </summary>
    public class NeeoFileSharingService : INeeoFileSharingService
    {
        private bool _logRequestResponse =
        Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);

        #region Profile

        /// <summary>
        /// Updates user information into the database and the profile picture.
        /// </summary>
        /// <param name="userID">A string containing phone number as user id.</param>
        /// <param name="name">A string containing the name of the user.</param>
        /// <param name="fileData">A base64 encoded string containing the file data.</param>
        /// <returns>
        /// true if information is successfully updated; otherwise, false.
        /// </returns>
        public bool UpdateUserInformation(string userID, string name, string fileData)
        {
            userID = (userID != null) ? userID.Trim() : userID;
            name = (name != null) ? name.Trim() : name;
            fileData = (fileData != null) ? fileData.Trim() : fileData;
            bool isUpdated = false;
            ulong temp = 0;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> userID : " + userID + ", name : " + name);
            }

            #endregion

            //#region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(userID, keyFromClient))
            //{
            //#endregion

            if (NeeoUtility.IsNullOrEmpty(userID) || !ulong.TryParse(userID, out temp))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else if (NeeoUtility.IsNullOrEmpty(name))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else
            {
                NeeoUser user = new NeeoUser(userID);
                try
                {
                    isUpdated = user.UpdateUserProfile(name, new LibNeeo.IO.File() { Data = fileData  });
                    #region old impl
                    //using (TransactionScope scope = new TransactionScope())
                    //{
                    //    if (!user.UpdateUsersDisplayName(name))
                    //    {
                    //        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidUser);
                    //    }
                    //    else if (NeeoUtility.IsNullOrEmpty(fileData))
                    //    {
                    //        isCompleted = true;
                    //        scope.Complete();
                    //    }
                    //    else
                    //    {
                    //        LibNeeo.IO.File file = new LibNeeo.IO.File()
                    //        {
                    //            Data = fileData,
                    //            FileOwner = userID,
                    //            MediaType = MediaType.Image,
                    //            MimeType = MimeType.Image_jpeg
                    //        };
                    //        if (user.SaveFile(file, FileCategory.Profile))
                    //        {
                    //            isCompleted = true;
                    //            scope.Complete();
                    //        }
                    //    }
                    //}
                    #endregion
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                }
            }
            return isUpdated;
            //}
            //else
            //{
            //   NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //    return false;
            //}
        }

        #region Release 3
        /// <summary>
        /// Delete the user's profile picture from the server.
        /// </summary>
        /// <param name="uID">A string containing phone number as user id.</param>
        /// <returns>true if the profile picture is successfully deleted from the server; otherwise, false.</returns>
        public bool DeleteAvatar(string uID)
        {
            uID = (uID != null) ? uID.Trim() : uID;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "Request ===> userID : " + uID);
            }

            #endregion

            #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            #endregion

                if (!NeeoUtility.IsNullOrEmpty(uID))
                {
                    try
                    {
                        NeeoUser user = new NeeoUser(uID);
                        if (user.DeleteUserAvatar())
                        {
                            return true;
                        }
                        else
                        {
                            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.FileSystemException);
                            return false;
                        }
                    }
                    catch (ApplicationException appExp)
                    {
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                    }
                    catch (Exception exp)
                    {
                        LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                    }
                    return false;
                }
                else
                {
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
                    return false;
                }
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //    return false;
            //}
        }

        #endregion

        /// <summary>
        /// Gets the user's avatar info base on the previous time stamp of the avatar.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="timeStamp">An integer containing the time stamp that has to be matched with the existing image. It is optional.</param>
        /// <param name="requiredDimension">An integer specifying the dimension of the image required. It is optional.</param>
        /// <returns>A object containing the avatar details if the provided time-stamp does not match; otherwise, not modified http status code.</returns>
        public ImageInfo GetAvatar(string userID, ulong timeStamp, uint requiredDimension)
        {
            userID = (userID != null) ? userID.Trim() : userID;
            ImageInfo imageInfo = null;
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> userID : " + userID + ", timeStamp : " + timeStamp +
                    ", requiredDimension : " + requiredDimension);
            }

            #endregion

            // #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(userID, keyFromClient))
            //{
            //    #endregion

            ulong temp = 0;
            userID = userID.Trim();

            if (NeeoUtility.IsNullOrEmpty(userID) || !ulong.TryParse(userID, out temp))
            {
                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.BadRequest);
            }
            else
            {
                userID = HttpUtility.UrlEncode(userID);
                NeeoUser user = new NeeoUser(userID);
                NeeoFileInfo fileInfo = null;
                ulong avatarTimeStamp;
                switch (user.GetAvatarState(timeStamp, out avatarTimeStamp, out fileInfo))
                {
                    case AvatarState.NotExist:
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
                        break;
                    case AvatarState.Modified:
                        imageInfo = new ImageInfo();
                        imageInfo.ImageUrl = NeeoUrlBuilder.BuildAvatarUrl(userID, timeStamp, requiredDimension);
                        imageInfo.TimeStamp = avatarTimeStamp;

                        #region log user request and response

                        /***********************************************
                        To log user response
                        ***********************************************/
                        if (_logRequestResponse)
                        {
                            LogManager.CurrentInstance.InfoLogger.LogInfo(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                                System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                                "Response ===> imageInfo : " + JsonConvert.SerializeObject(imageInfo));
                        }

                        #endregion

                        break;
                    case AvatarState.NotModified:
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotModified);
                        break;
                    default:
                        break;
                }
            }
            return imageInfo;
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //    return imageInfo;
            //}

        }

        /// <summary>
        /// Gets the user's avatar base on the previous time stamp of the avatar.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="timeStamp">An integer containing the time stamp that has to be matched with the existing image. It is optional.</param>
        /// <param name="requiredDimension">An integer specifying the dimension of the image required. It is optional.</param>
        public void GetUserAvatar(string userID, ulong timeStamp, uint requiredDimension)
        {
            userID = (userID != null) ? userID.Trim() : userID;


            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> userID : " + userID + ", timeStamp : " + timeStamp +
                    ", requiredDimension : " + requiredDimension);
            }

            #endregion
            //        #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(userID, keyFromClient))
            //{
            //    #endregion

            ulong temp = 0;

            if (NeeoUtility.IsNullOrEmpty(userID) || !ulong.TryParse(userID, out temp))
            {
                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.BadRequest);
            }
            else
            {
                userID = HttpUtility.UrlEncode(userID);
                NeeoUser user = new NeeoUser(userID);
                NeeoFileInfo fileInfo = null;
                ulong avatarTimeStamp;
                switch (user.GetAvatarState(timeStamp, out avatarTimeStamp, out fileInfo))
                {
                    case AvatarState.NotExist:
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
                        break;
                    case AvatarState.Modified:
                        string url = NeeoUrlBuilder.BuildAvatarUrl(userID, timeStamp, requiredDimension);
                        RedirectServiceToUrl(url, avatarTimeStamp);
                        break;
                    case AvatarState.NotModified:
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotModified);
                        break;
                }
            }
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //}

        }

        /// <summary>
        /// Gets the user's name base on the User ID.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <returns>It returns the profile name.</returns>
        public string GetProfileName(string uID)
        {

            uID = (uID != null) ? uID.Trim() : uID;
            string result = null;

            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> userID : " + uID);
            }

            #endregion

            //     #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //    #endregion

            ulong temp = 0;
            uID = uID.Trim();
            try
            {
                if (NeeoUtility.IsNullOrEmpty(uID) || !ulong.TryParse(uID, out temp))
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.BadRequest);
                }
                else
                {
                    NeeoUser user = new NeeoUser(uID);
                    result = user.GetUserProfileName();
                }
            }
            catch (ApplicationException appExp)
            {
                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
            }
            return result;
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);
            //    return result;
            //}
        }

        /// <summary>
        /// Gets the user's name and avatar base on the User ID.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <returns>It returns the avatar and name in header.</returns>
        public Stream SyncProfile(string uID)
        {
            uID = (uID != null) ? uID.Trim() : uID;
            Stream resultStream = null;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> userID : " + uID);
            }

            #endregion

            //    #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //    #endregion


            ulong temp = 0;
            uID = uID.Trim();

            if (NeeoUtility.IsNullOrEmpty(uID) || !ulong.TryParse(uID, out temp))
            {
                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.BadRequest);
            }
            else
            {
                NeeoUser user = new NeeoUser(uID);

                try
                {
                    SetHeaderForSyncProfile(user.GetUserProfileName());
                    resultStream = user.GetAvatarStream();
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                }
            }
            return resultStream;
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode) HttpStatusCode.Unauthorized);
            //    return resultStream;
            //}
        }
        #endregion

        #region Shared Media

        /// <summary>
        /// Checks the file transfer support on receiver side.
        /// </summary>
        /// <param name="sUID">A string containing the sender user id.</param>
        /// <param name="rUID">A string containing the receiver user id.</param>
        public void CheckSupport(string sUID, string rUID)
        {
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "CheckSupport -- Request ===> senderID : " + sUID + ", receiverID : " + rUID);
            }

            #endregion
            ulong temp = 0;
            sUID = (sUID != null) ? sUID.Trim() : sUID;
            rUID = (rUID != null) ? rUID.Trim() : rUID;
            // #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(sUID, keyFromClient))
            //{
            //    #endregion

            if (NeeoUtility.IsNullOrEmpty(sUID) && !ulong.TryParse(sUID, out temp) && NeeoUtility.IsNullOrEmpty(rUID) && !ulong.TryParse(rUID, out temp))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else
            {
                try
                {
                    NeeoUser senderUser = new NeeoUser(sUID);
                    if (!senderUser.IsFileTransferSupported(rUID))
                    {
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.FileTransferNotSupported);
                    }
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
            }
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="data"></param>
        /// <param name="mimeType"></param>
        /// <param name="recipientCount"></param>
        /// <returns></returns>
        public string UploadFile(string uID, string fID, string data, ushort mimeType, ushort recipientCount)
        {
            ulong temp = 0;
            string resultUrl = null;
            uID = (uID != null) ? uID.Trim() : uID;
            fID = (fID != null) ? fID.Trim() : fID;
            fID = (fID != null) ? fID.Trim() : fID;


            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> senderID : " + uID + ", mimeType : " + mimeType.ToString() + ", fID : " + fID + ", recipientCount : " + recipientCount.ToString());
            }

            #endregion

            //      #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //    #endregion

            if (NeeoUtility.IsNullOrEmpty(uID) && !ulong.TryParse(uID, out temp) && !Enum.IsDefined(typeof(MimeType), mimeType) && (recipientCount == 0 || recipientCount >= 255) &&
                NeeoUtility.IsNullOrEmpty(data))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else
            {
                if (!NeeoUtility.IsNullOrEmpty(fID))
                {
                    Guid resultingGuid;
                    if (!Guid.TryParse(fID, out resultingGuid))
                    {
                        LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "fID : " + fID + " is not parseable.");
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
                        return resultUrl;
                    }
                    fID = resultingGuid.ToString("N").ToLower();
                }

                NeeoUser sender = new NeeoUser(uID);
                try
                {
                    LibNeeo.IO.File file = new LibNeeo.IO.File() { Info = new NeeoFileInfo() { Creator = uID, MediaType = MediaType.Image, MimeType = (MimeType)mimeType, Name = fID }, Data = data };
                    if (SharedMedia.Save(sender, file, FileCategory.Shared, recipientCount))
                    {
                        resultUrl = file.Info.Url;
                    }
                    #region log user request and response

                    /***********************************************
                    To log user response
                    ***********************************************/
                    if (_logRequestResponse)
                    {
                        LogManager.CurrentInstance.InfoLogger.LogInfo(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "Response ===> " + resultUrl);
                    }

                    #endregion
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                }
            }
            return resultUrl;
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);
            //    return resultUrl;
            //}
        }

        /// <summary>
        /// Checks the already uploaded file whether it still exist or not. If exists, its shared date-time and number of recipient information are updated.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="fileID">A string containing the file id.</param>
        /// <param name="recipientCount">An unsigned integer value containing the recipient count.</param>
        /// <returns>If true, file exists and information is updated; otherwise false.</returns>
        public bool ShareFile(string uID, string fID, ushort recipientCount)
        {
            uID = (uID != null) ? uID.Trim() : uID;
            fID = (fID != null) ? fID.Trim() : fID;
            bool result = false;
            ulong temp = 0;
            string resultUrl = null;

            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> senderID : " + uID + ", recipientCount : " + recipientCount.ToString());
            }

            #endregion
            // #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //    #endregion



            if (NeeoUtility.IsNullOrEmpty(uID) && !ulong.TryParse(uID, out temp) &&
                (recipientCount == 0 || recipientCount >= 255) && NeeoUtility.IsNullOrEmpty(fID))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else
            {
                NeeoUser user = new NeeoUser(uID);
                try
                {
                    result = SharedMedia.Share(user, fID, recipientCount);
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                       System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                }
            }
            return result;
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);
            //    return result;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="fileID"></param>
        public void Acknowledgement(string uID, string fileID)
        {
            ulong temp = 0;
            uID = (uID != null) ? uID.Trim() : uID;
            fileID = (fileID != null) ? fileID.Trim() : fileID;
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> senderID : " + uID + ", fileID : " + fileID);
            }

            #endregion
            //#region Verify User
            //  var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //  string keyFromClient = request.Headers["key"];
            //  if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //  {
            //      #endregion

            if (NeeoUtility.IsNullOrEmpty(uID) && !ulong.TryParse(uID, out temp) && NeeoUtility.IsNullOrEmpty(fileID))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else
            {
                NeeoUser user = new NeeoUser(uID);
                try
                {
                    if (!SharedMedia.UpdateDownloadCount(user, fileID))
                    {
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
                    }
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
            }
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //}
        }

        #endregion

        #region Group Icon

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="data"></param>
        /// <param name="gID"></param>
        public void UploadGroupIcon(string uID, string data, string gID)
        {
            ulong temp = 0;
            uID = (uID != null) ? uID.Trim() : uID;

            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> senderID : " + uID + ", groupID : " + gID);
            }

            #endregion
            // #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //    #endregion


            if (NeeoUtility.IsNullOrEmpty(uID) || !ulong.TryParse(uID, out temp) ||
                NeeoUtility.IsNullOrEmpty(data) ||
                NeeoUtility.IsNullOrEmpty(gID))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else
            {
                gID = gID.ToLower();
                try
                {
                    var file = new LibNeeo.IO.File()
                    {
                        Info = new NeeoFileInfo()
                    {
                        Creator = uID,
                        MimeType = MimeType.ImageJpeg,
                        MediaType = MediaType.Image
                    },
                        Data = data,
                    };
                    if (!NeeoGroup.SaveGroupIcon(uID, gID.ToLower(), file))
                    {
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                    }
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                }
            }
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode) HttpStatusCode.Unauthorized);

            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="gID"></param>
        public void GetGroupIcon(string uID, string gID)
        {
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> senderID : " + uID + ", groupID : " + gID);
            }

            #endregion

            //  #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //    #endregion

            ulong temp = 0;


            if (NeeoUtility.IsNullOrEmpty(uID) || !ulong.TryParse(uID, out temp) ||
                NeeoUtility.IsNullOrEmpty(gID))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else
            {
                uID = uID.Trim();
                gID = gID.ToLower();
                try
                {
                    if (NeeoGroup.GroupIconExists(gID.ToLower()))
                    {
                        string url =
                            NeeoUrlBuilder.BuildFileUrl(ConfigurationManager.AppSettings[NeeoConstants.FileServerUrl], gID,
                                FileCategory.Group, MediaType.Image);
                        RedirectServiceToUrl(url);
                    }
                    else
                    {
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
                    }
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                }
            }
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode) HttpStatusCode.Unauthorized);

            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="gID"></param>
        /// <param name="reqType"></param>
        public void GetGroupIcon(string uID, string gID, string reqType = "GET")
        {
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> senderID : " + uID + ", groupID : " + gID);
            }

            #endregion

            //      #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //    #endregion
            ulong temp = 0;

            if (NeeoUtility.IsNullOrEmpty(uID) || !ulong.TryParse(uID, out temp) ||
                NeeoUtility.IsNullOrEmpty(gID))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else
            {
                uID = uID.Trim();
                gID = gID.ToLower();
                try
                {
                    if (!NeeoGroup.GroupIconExists(gID))
                    {
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
                    }
                    else
                    {
                        string url =
                            NeeoUrlBuilder.BuildFileUrl(ConfigurationManager.AppSettings[NeeoConstants.FileServerUrl], gID,
                                FileCategory.Group, MediaType.Image);
                        RedirectServiceToUrl(url);
                    }
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                }
            }
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="gID"></param>
        public void DeleteGroupIcon(string uID, string gID)
        {
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> senderID : " + uID + ", groupID : " + gID);
            }

            #endregion

            //#region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //    #endregion

            ulong temp = 0;

            if (NeeoUtility.IsNullOrEmpty(uID) || !ulong.TryParse(uID, out temp) ||
                NeeoUtility.IsNullOrEmpty(gID))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            else
            {
                uID = uID.Trim();
                gID = gID.ToLower();
                try
                {
                    NeeoGroup.DeleteGroupIcon(groupID: gID, userID: uID);
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                }
            }
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode) HttpStatusCode.Unauthorized);

            //}
        }

        #endregion

        #region Service Private Methods

        /// <summary>
        /// Redirects a service call to url specified in <paramref name="url"/>
        /// </summary>
        /// <param name="url">A string containing the url on which request has to be redirected.</param>
        private void RedirectServiceToUrl(string url, ulong avatarTimeStamp = 0)
        {
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
            if (avatarTimeStamp != 0)
            {
                response.Headers.Add("ts", avatarTimeStamp.ToString());
            }
            response.StatusCode = System.Net.HttpStatusCode.Redirect;
            response.Location = url;
        }

        /// <summary>
        /// Sets the header for syncProfile 
        /// </summary>
        /// <param name="name">A string containing User name.</param>
        private void SetHeaderForSyncProfile(string name)
        {
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
            response.Headers.Add("name", name);
        }

        #endregion
    }
}
