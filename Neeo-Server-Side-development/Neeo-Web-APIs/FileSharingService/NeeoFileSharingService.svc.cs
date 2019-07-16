using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Configuration;
using System.Text;
using System.Net;
using System.Transactions;
using System.Web;
using Common;
using LibNeeo;
using DAL;
using Logger;
using Newtonsoft.Json;

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
        /// <returns>true if information is successfully updated; otherwise, false.</returns>
        public bool UpdateUserInformation(string userID, string name, string fileData)
        {
            bool isCompletedSuccessfully = false;

            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "Request ===> userID : " + userID + ", name : " + name +
                    ", fileData : " + fileData);
            }

            #endregion

            if (!NeeoUtility.IsNullOrEmpty(userID))
            {
                if (name != null)
                {
                    NeeoUser user = new NeeoUser(userID);
                    try
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            if (user.UpdateUsersDisplayName(name))
                            {
                                if (!NeeoUtility.IsNullOrEmpty(fileData))
                                {
                                    File file = new File()
                                    {
                                        FileName = NeeoUtility.ConvertToFileName(userID),
                                        Data = fileData,
                                        FileOwner = userID,
                                        MediaType = MediaType.Image,
                                        MimeType = MimeType.Image_jpeg
                                    };
                                    if (user.UploadFile(file, FileClassfication.Profile))
                                    {
                                        isCompletedSuccessfully = true;
                                        scope.Complete();
                                    }
                                }
                                else
                                {
                                    isCompletedSuccessfully = true;
                                    scope.Complete();
                                }
                            }
                            else
                            {
                                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidUser);
                            }
                        }
                        return isCompletedSuccessfully;
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
                    return isCompletedSuccessfully;
                }
                else
                {
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
                }
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            return isCompletedSuccessfully;
        }

        /// <summary>
        /// Delete the user's profile picture from the server.
        /// </summary>
        /// <param name="userID">A string containing phone number as user id.</param>
        /// <returns>true if the profile picture is successfully deleted from the server; otherwise, false.</returns>
        public bool DeleteAvatar(string userID)
        {
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "Request ===> userID : " + userID);
            }

            #endregion

            if (!NeeoUtility.IsNullOrEmpty(userID))
            {
                try
                {
                    NeeoUser user = new NeeoUser(userID);
                    if (user.DeleteUserAvatar(NeeoUtility.ConvertToFileName(userID)))
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
        }

        ///// <summary>
        ///// Gets profile picture of the user specified with user id.
        ///// </summary>
        ///// <param name="userID">A string containing the user id.</param>
        //public void GetAvatar(string userID)
        //{
        //    User user = new User(userID);
        //    ulong avatarTimeStamp = 0;
        //    switch (user.GetAvatarState(avatarTimeStamp))
        //    {
        //        case AvatarState.NotExist:
        //            NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
        //            break;
        //        case AvatarState.Modified:
        //            string url = NeeoUtility.GenerateFileLink(RequestType.Profile, userID, userID);
        //            RedirectServiceToUrl(url);
        //            break;
        //        case AvatarState.NotModified:
        //            NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotModified);
        //            break;
        //    } 
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <param name="timeStamp"></param>
        //public void GetAvatar(string userID, ulong timeStamp)
        //{
        //    if (timeStamp.ToString().Length > 7)
        //    {
        //        User user = new User(userID);
        //        ulong avatarTimeStamp;
        //        switch (user.GetAvatarState(timeStamp))
        //        {
        //            case AvatarState.NotExist:
        //                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
        //                break;
        //            case AvatarState.Modified:
        //                string url = NeeoUtility.GenerateFileLink(RequestType.Profile, userID, userID);
        //                RedirectServiceToUrl(url);
        //                break;
        //            case AvatarState.NotModified:
        //                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotModified);
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.BadRequest);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <param name="width"></param>
        ///// <param name="height"></param>
        //public void GetAvatar(string userID, ushort width, ushort height)
        //{
        //    //if (width > 128 && height > 128 && width == height)
        //    //{
        //    //    User user = new User(userID);
        //    //    if (AvatarState.Exists == user.GetAvatarState())
        //    //    {
        //    //        string url = NeeoUtility.GenerateFileLink(RequestType.Profile, userID, userID);
        //    //        RedirectServiceToUrl(url);
        //    //    }
        //    //    else
        //    //    {
        //    //        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode) HttpStatusCode.BadRequest);
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.BadRequest);
        //    //}
        //}

        /// <summary>
        /// Gets the user's avatar info base on the previous time stamp of the avatar.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="timeStamp">An integer containing the time stamp that has to be matched with the existing image. It is optional.</param>
        /// <param name="requiredDimension">An integer specifying the dimension of the image required. It is optional.</param>
        /// <returns>A object containing the avatar details if the provided time-stamp does not match; otherwise, not modified http status code.</returns>
        public ImageInfo GetAvatar(string userID, ulong timeStamp, uint requiredDimension)
        {
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "Request ===> userID : " + userID + ", timeStamp : " + timeStamp +
                    ", requiredDimension : " + requiredDimension);
            }

            #endregion

            if (!NeeoUtility.IsNullOrEmpty(userID))
            {
                userID = HttpUtility.UrlEncode(userID);
                NeeoUser user = new NeeoUser(userID);
                string filePath = "";
                ulong avatarTimeStamp;
                switch (user.GetAvatarState(timeStamp, out avatarTimeStamp, out filePath))
                {
                    case AvatarState.NotExist:
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
                        return null;

                    case AvatarState.Modified:
                        ImageInfo imageInfo = new ImageInfo();
                        imageInfo.ImageUrl = NeeoUtility.GenerateAvatarUrl(userID, timeStamp, requiredDimension);
                        imageInfo.TimeStamp = avatarTimeStamp;

                        #region log user request and response

                        /***********************************************
                        To log user response
                        ***********************************************/
                        if (_logRequestResponse)
                        {
                            LogManager.CurrentInstance.InfoLogger.LogInfo(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                                "Response ===> imageInfo : " + JsonConvert.SerializeObject(imageInfo));
                        }

                        #endregion

                        return imageInfo;

                    case AvatarState.NotModified:
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotModified);
                        return null;

                    default:
                        return null;
                }
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.BadRequest);
                return null;
            }
        }

        /// <summary>
        /// Gets the user's avatar base on the previous time stamp of the avatar.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="timeStamp">An integer containing the time stamp that has to be matched with the existing image. It is optional.</param>
        /// <param name="requiredDimension">An integer specifying the dimension of the image required. It is optional.</param>
        public void GetUserAvatar(string userID, ulong timeStamp, uint requiredDimension)
        {
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "Request ===> userID : " + userID + ", timeStamp : " + timeStamp +
                    ", requiredDimension : " + requiredDimension);
            }

            #endregion

            if (!NeeoUtility.IsNullOrEmpty(userID))
            {
                userID = HttpUtility.UrlEncode(userID);
                NeeoUser user = new NeeoUser(userID);
                string filePath = "";
                ulong avatarTimeStamp;
                switch (user.GetAvatarState(timeStamp, out avatarTimeStamp, out filePath))
                {
                    case AvatarState.NotExist:
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
                        break;
                    case AvatarState.Modified:
                        string url = NeeoUtility.GenerateAvatarUrl(userID, timeStamp, requiredDimension);
                        RedirectServiceToUrl(url, avatarTimeStamp);
                        break;
                    case AvatarState.NotModified:
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotModified);
                        break;
                }
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.BadRequest);
            }
        }

        #endregion

        #region File Sharing

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="data"></param>
        /// <param name="mimeType"></param>
        /// <param name="recipientCount"></param>
        /// <returns></returns>
        public string UploadFile(string uID,  string data, ushort mimeType, ushort recipientCount)
        {
            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "Request ===> senderID : " + uID + ", fileData : " + data);
            }

            #endregion

            string result = null;
            
           
            if (!NeeoUtility.IsNullOrEmpty(uID) && Enum.IsDefined(typeof(MimeType),mimeType) && (recipientCount > 0 && recipientCount < 255) &&
                !NeeoUtility.IsNullOrEmpty(data))
            {
                NeeoUser sender = new NeeoUser(uID);
               
                try
                {
                    File file = new File() {Data = data, FileOwner = uID, MediaType = MediaType.Image, MimeType = (MimeType)mimeType};
                    if (sender.UploadFile(file, FileClassfication.SharedFile, recipientCount))
                    {
                        result =  file.FileUrl;
                    }

                    #region log user request and response

                    /***********************************************
                    To log user response
                    ***********************************************/
                    if (_logRequestResponse)
                    {
                        LogManager.CurrentInstance.InfoLogger.LogInfo(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "Response ===> " + result);
                    }

                    #endregion

                    return result;
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
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="fileID"></param>
        public void Acknowledgement(string uID, string fileID)
        {
            if (!NeeoUtility.IsNullOrEmpty(uID) && !NeeoUtility.IsNullOrEmpty(fileID))
            {
                NeeoUser user = new NeeoUser(uID);
                try
                {
                    if (!user.UpdateSharedFileDownloadCount(fileID))
                    {
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.NotFound);
                    }
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
        }

        #endregion

        #region Service Private Methods

        /// <summary>
        /// Redirects a service call to url specified in <paramref name="url"/>
        /// </summary>
        /// <param name="url">A string containing the url on which request has to be redirected.</param>
        private void RedirectServiceToUrl(string url, ulong avatarTimeStamp)
        {
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
            response.Headers.Add("ts", avatarTimeStamp.ToString());
            response.StatusCode = System.Net.HttpStatusCode.Redirect;
            response.Location = url;
        }

        #endregion
    }
}
