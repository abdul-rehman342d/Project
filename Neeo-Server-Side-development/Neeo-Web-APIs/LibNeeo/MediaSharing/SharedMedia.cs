using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using Common;
using DAL;
using LibNeeo.IO;
using LibNeeo.Network;
using LibNeeo.Url;
using Logger;
using File = LibNeeo.IO.File;

namespace LibNeeo.MediaSharing
{
    /// <summary>
    /// 
    /// </summary>
    public class SharedMedia
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="file"></param>
        /// <param name="fileCategory"></param>
        /// <param name="recipientCount"></param>
        /// <returns></returns>
        public static bool Save(NeeoUser user, File file, FileCategory fileCategory, ushort recipientCount = 0)
        {
            bool isOperationCompleted = false;
            file.Info.Name = NeeoUtility.IsNullOrEmpty(file.Info.Name) ? Guid.NewGuid().ToString("N") : file.Info.Name;
            var server = FileServerManager.GetInstance().SelectServer();
            DbManager dbManager = new DbManager();
            file.Info.FullPath = server.GetServerNetworkPath();

            try
            {
                //file.Info.CreationTimeUtc = DateTime.UtcNow;
                FileManager.Save(file, FileCategory.Shared);
                if (dbManager.StartTransaction())
                {
                    if (dbManager.InsertSharedFileInformation(file.Info.Name, file.Info.Creator, Convert.ToUInt16(file.Info.MediaType), Convert.ToUInt16(file.Info.MimeType),
                        Path.Combine(file.Info.FullPath, file.Info.FullName), file.Info.CreationTimeUtc, recipientCount, file.Info.Length, file.Info.Hash))
                    {
                        file.Info.Url = NeeoUrlBuilder.BuildFileUrl(server.LiveDomain, file.Info.Name, FileCategory.Shared, file.Info.MediaType);
                        dbManager.CommitTransaction();
                        isOperationCompleted = true;
                    }
                    else
                    {
                        dbManager.RollbackTransaction();
                    }
                }
            }
            catch (ApplicationException appException)
            {
                dbManager.RollbackTransaction();
                throw;
            }
            catch (Exception exception)
            {
                dbManager.RollbackTransaction();
                LogManager.CurrentInstance.ErrorLogger.LogError(MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                throw;
            }
            return isOperationCompleted;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="file"></param>
        /// <param name="fileCategory"></param>
        /// <param name="recipientCount"></param>
        /// <param name="isOverloaded"></param>
        /// <returns></returns>
        public static bool Save(NeeoUser user, File file, FileCategory fileCategory, ushort recipientCount = 0, bool isOverloaded = true)
        {
            bool isOperationCompleted = false;
            file.Info.Name = NeeoUtility.IsNullOrEmpty(file.Info.Name) ? Guid.NewGuid().ToString("N") : file.Info.Name;
            var server = FileServerManager.GetInstance().SelectServer();
            DbManager dbManager = new DbManager();
            file.Info.FullPath = server.GetServerNetworkPath();

            try
            {
                //file.Info.CreationTimeUtc = DateTime.UtcNow;
                FileManager.Save(file, FileCategory.Shared);
                if (dbManager.StartTransaction())
                {
                    if (dbManager.InsertSharedFileInformation(file.Info.Name, file.Info.Creator, Convert.ToUInt16(file.Info.MediaType), Convert.ToUInt16(file.Info.MimeType),
                        Path.Combine(file.Info.FullPath, file.Info.FullName), file.Info.CreationTimeUtc, recipientCount, file.Info.Length, file.Info.Hash))
                    {
                        file.Info.Url = NeeoUrlBuilder.BuildFileUrl(server.LiveDomain, file.Info.Name, FileCategory.Shared, file.Info.MediaType);
                        dbManager.CommitTransaction();
                        isOperationCompleted = true;
                    }
                    else
                    {
                        dbManager.RollbackTransaction();
                    }
                }
            }
            catch (ApplicationException appException)
            {
                dbManager.RollbackTransaction();
                throw;
            }
            catch (Exception exception)
            {
                dbManager.RollbackTransaction();
                LogManager.CurrentInstance.ErrorLogger.LogError(MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                throw new ApplicationException(CustomHttpStatusCode.ServerInternalError.ToString("D"));
            }
            return isOperationCompleted;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static File GetMedia(string fileID, MediaType mediaType)
        {
            return FileManager.GetFile(fileID, FileCategory.Shared, mediaType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static bool IsExist(string fileID, MediaType mediaType)
        {
            return FileManager.IsExist(fileID, FileCategory.Shared);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fID"></param>
        /// <param name="recipientCount"></param>
        /// <returns></returns>
        public static bool Share(NeeoUser user, string fileID, ushort recipientCount)
        {
            DbManager dbManager = new DbManager();
            if (dbManager.UpdateFileSharedDateWithRecipientCount(user.UserID, fileID, recipientCount))
            {
                return true;
            }
            throw new ApplicationException(HttpStatusCode.NotFound.ToString("D"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public static bool UpdateDownloadCount(NeeoUser user, string fileID)
        {
            DbManager dbManager = new DbManager();
            return dbManager.UpdateSharedFileDownloadCount(user.UserID, fileID);
        }
        /// <summary>
        /// Deletes the expired files from file servers.
        /// </summary>
        public static void DeleteExpiredFiles()
        {
            const string delimeter = ",";
            const string expiryPeriod = "expiryPeriod";
            DbManager dbManager = new DbManager();
            
            DataTable dtExpiredFiles = dbManager.GetExpiredFile(DateTime.UtcNow, Convert.ToInt16(ConfigurationManager.AppSettings[expiryPeriod]));
            string recordIDs = "";

            foreach (DataRow dr in dtExpiredFiles.Rows)
            {
                try
                {
                    if (File.Delete(dr["fullPath"].ToString()))
                    {
                        recordIDs += dr["id"].ToString() + ",";
                    }
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                }
            }
            if (recordIDs != string.Empty)
            {
                dbManager.DeleteSharedFile(recordIDs, delimeter);
            }
        }
    }
}
