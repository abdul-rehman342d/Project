using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using FileSharingService;

namespace FileSharingService
{
    /// <summary>
    /// Contains the signatures of  method/resources exposed by the FileSharing service.
    /// </summary>
    [ServiceContract]
    public interface INeeoFileSharingService
    {
        /// <summary>
        /// Updates user information into the database and the profile picture.
        /// </summary>
        /// <param name="userID">A string containing phone number as user id.</param>
        /// <param name="name">A string containing the name of the user.</param>
        /// <param name="fileData">A base64 encoded string containing the file data.</param>
        /// <returns>true if information is successfully updated; otherwise, false.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/UpdateProfileInformation", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool UpdateUserInformation(string userID, string name, string fileData);

        /// <summary>
        /// Delete the user's profile picture from the server.
        /// </summary>
        /// <param name="userID">A string containing phone number as user id.</param>
        /// <returns>true if the profile picture is successfully deleted from the server; otherwise, false.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/DeleteAvatar", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool DeleteAvatar(string uID);

        /// <summary>
        /// Gets the user's avatar info base on the previous time stamp of the avatar.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="timeStamp">An integer containing the time stamp that has to be matched with the existing image. It is optional.</param>
        /// <param name="requiredDimension">An integer specifying the dimension of the image required. It is optional.</param>
        /// <returns>A object containing the avatar details if the provided time-stamp does not match; otherwise, not modified http status code.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetAvatar?uid={userID}&ts={timeStamp}&dim={requiredDimension}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        ImageInfo GetAvatar(string userID, ulong timeStamp,uint requiredDimension);

        /// <summary>
        /// Gets the user's avatar info base on the previous time stamp of the avatar.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="timeStamp">An integer containing the time stamp that has to be matched with the existing image. It is optional.</param>
        /// <param name="requiredDimension">An integer specifying the dimension of the image required. It is optional.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetUserAvatar?uid={userID}&ts={timeStamp}&dim={requiredDimension}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        void GetUserAvatar(string userID, ulong timeStamp, uint requiredDimension);
        # region SyncProfile
        /// <summary>
        /// Gets the user's name and avatar info base on the ID of the User.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <returns>It returns the avatar and name in header of user.</returns>
        //[OperationContract]
        //[WebInvoke(UriTemplate = "/SyncProfile", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        //Stream SyncProfile(string uID);
        #endregion

        /// <summary>
        /// Gets the user's name base on the ID of User.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <returns>It returns the profile name of user.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetProfileName", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetProfileName(string uID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/UploadFile", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string UploadFile(string uID, string fID , string data, ushort mimeType, ushort recipientCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="fileID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/acknowledgement", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        void Acknowledgement(string uID, string fileID);

        /// <summary>
        /// Checks the already uploaded file whether it still exist or not. If exists, its shared date-time and number of recipient information are updated.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="fileID">A string containing the file id.</param>
        /// <param name="recipientCount">An unsigned integer value containing the recipient count.</param>
        /// <returns>If true, file exists and information is updated; otherwise false.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/ShareFile", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool ShareFile(string uID, string fID, ushort recipientCount);

        /// <summary>
        /// Checks the file transfer support on receiver side.
        /// </summary>
        /// <param name="sUID">A string containing the sender user id.</param>
        /// <param name="rUID">A string containing the receiver user id.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/CheckSupport", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        void CheckSupport(string sUID, string rUID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="data"></param>
        /// <param name="gID"></param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/Group/UploadIcon", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UploadGroupIcon(string uID, string data, string gID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="gID"></param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/Group/GetIcon", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        void GetGroupIcon(string uID, string gID);

       /// <summary>
       /// 
       /// </summary>
       /// <param name="uID"></param>
       /// <param name="gID"></param>
       /// <param name="reqType"></param>
        [OperationContract(Name = "GetGrpIcon")]
        [WebInvoke(UriTemplate = "/Group/GetIcon?type={reqType}&uid={uID}&gid={gID}", Method = "GET")]
        void GetGroupIcon(string uID, string gID, string reqType = "GET");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="gID"></param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/Group/DeleteIcon", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        void DeleteGroupIcon(string uID, string gID);

    }


    
}
