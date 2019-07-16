using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using LibNeeo.Resource;

namespace LibNeeo.Url
{
    public static class NeeoUrlBuilder
    {
        /// <summary>
        /// Generates link for the file with required parameters.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="avatarTimeStamp">An unsigned long integer containing the avatar time stamp.</param>
        /// <param name="requiredDimension">An unsigned integer specifying the required dimensions.</param>
        /// <remarks>Dimensions should be greater than 0 and less than the actual dimension of the file. If dimension is 0 or greater than the actual dimension of the image, image with actual dimensions will be returned.</remarks>
        /// <returns>A string containing the url for getting user avatar.</returns>
        public static string BuildAvatarUrl(string userID, ulong avatarTimeStamp, uint requiredDimension)
        {
            string fileServerUrl = ConfigurationManager.AppSettings[NeeoConstants.FileServerUrl];
            string fileStorePort = ConfigurationManager.AppSettings[NeeoConstants.FileStorePort];
            string avatarHandler = ConfigurationManager.AppSettings[NeeoConstants.AvatarHandler];
            string webProtocol = ConfigurationManager.AppSettings[NeeoConstants.WebProtocol];
            string uIDQString = "?uid=" + userID;
            string imgTimeStampQString = "&ts=" + avatarTimeStamp;
            string imgRequiredDimensionQString = "&dim=" + requiredDimension;
            string url = webProtocol + "://" + fileServerUrl + (NeeoUtility.IsNullOrEmpty(fileStorePort) == false ? (":" + fileStorePort + "/") : "/") + avatarHandler + uIDQString + imgTimeStampQString;

            if (requiredDimension > 0)
            {
                url = url + imgRequiredDimensionQString;
            }
            return url;
        }

        /// <summary>
        /// Generates link for the file with required parameters.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="fileName">A string containing the file name.</param>
        /// <param name="fileClassfication">An enum that is specifying the file classification.</param>
        /// <param name="requiredDimension">An unsigned integer specifying the required dimensions.</param>
        /// <param name="senderID">A string containing sender user id. Required only getting url of offline file.</param>
        /// <remarks>Dimensions should be greater than 0 and less than the actual dimension of the file. If dimension is 0 or greater than the actual dimension of the image, image with actual dimensions will be returned.</remarks>
        /// <returns>A string containing the url for getting the file.</returns>
        public static string BuildFileUrl(string fileServer, string fileName, FileCategory fileClassfication, MediaType mediaType)
        {
            string fileStorePort = ConfigurationManager.AppSettings[NeeoConstants.FileStorePort];
            string imageHandler = ConfigurationManager.AppSettings[NeeoConstants.ImageHandler];
            string webProtocol = ConfigurationManager.AppSettings[NeeoConstants.WebProtocol];
            string idQString = "?id=" + fileName;
            string fileClassificationQString = "&fc=" + fileClassfication.ToString("D");
            string mediaTypeQString = "&mt=" + mediaType.ToString("D");
            string signatureQString = "&sig=" + NeeoUtility.GenerateSignature(fileName + fileClassfication.ToString("D") + mediaType.ToString("D"));
            string url = webProtocol + "://" + fileServer + (NeeoUtility.IsNullOrEmpty(fileStorePort) == false ? (":" + fileStorePort + "/") : "/") + imageHandler + idQString + fileClassificationQString + mediaTypeQString + signatureQString;
            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public static string BuildResumableUploadUrl(string sessionID)
        {
            string webProtocol = ConfigurationManager.AppSettings[NeeoConstants.WebProtocol];
            string apiUrl = ConfigurationManager.AppSettings[NeeoConstants.ResumableApiUrl];
            string url = webProtocol + "://" + apiUrl + "/media/upload?type=resumable&sessionid=" + sessionID;
            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public static object BuildResourceUrls(IAppResource resource)
        {
            var resourceUrlDictionary = new Dictionary<string, string>();
            string webProtocol = ConfigurationManager.AppSettings[NeeoConstants.WebProtocol];
            string apiUrl = ConfigurationManager.AppSettings[NeeoConstants.ResumableApiUrl];
            foreach (var item in resource.Components)
            {
                string url = webProtocol + "://" + apiUrl + "/resource/" + (int) resource.ResourceType + "/" +
                             resource.Name.ToLower() + "/" + (resource.isDefault ? 1 : 0) +"/"+ item;
                resourceUrlDictionary.Add(Path.GetFileNameWithoutExtension(item), url);
            }
            return resourceUrlDictionary;
        }
    }
}
