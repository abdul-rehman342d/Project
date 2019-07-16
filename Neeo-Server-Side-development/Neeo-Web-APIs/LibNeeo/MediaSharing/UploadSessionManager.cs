using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DAL;
using LibNeeo.IO;

namespace LibNeeo.MediaSharing
{
    public class UploadSessionManager
    {
        public static async Task<UploadSession> CreateSessionAsync(NeeoFileInfo uploadingFileInfo)
        {
            var uploadSession = new UploadSession(uploadingFileInfo);
            uploadSession.SessionID = Guid.NewGuid().ToString("N");
            var dbManager = new DbManager();
            if (await dbManager.InsertUploadSessionAsync(uploadSession.SessionID, uploadSession.FileInfo.Name, uploadSession.FileInfo.Creator,
                (ushort)uploadSession.FileInfo.MediaType, (ushort)uploadSession.FileInfo.MimeType, uploadSession.FileInfo.FullPath,
                uploadSession.CreationDate, uploadSession.FileInfo.Length, uploadSession.FileInfo.Hash))
            {
                return uploadSession;
            }
            return null;
        }

        public static async Task<bool> UpdateSessionAsync(UploadSession uploadSession)
        {
            var dbManager = new DbManager();
            return await dbManager.UpdateUploadSessionAsync(uploadSession.SessionID, uploadSession.FileInfo.Name, uploadSession.FileInfo.Creator,
                (ushort)uploadSession.FileInfo.MediaType, (ushort)uploadSession.FileInfo.MimeType, uploadSession.FileInfo.FullPath,
                uploadSession.CreationDate, uploadSession.FileInfo.Length, uploadSession.FileInfo.Hash);
        }

        public static async Task<UploadSession> ValidateSessionAsync(string sessionID)
        {
            UploadSession uploadSession = null;
            var dbManager = new DbManager();
            var sessionDetails = await dbManager.GetUploadSessionAsync(sessionID);
            if (sessionDetails.Rows.Count > 0)
            {
                uploadSession = sessionDetails.AsEnumerable().Select(dr => new UploadSession()
                {
                    SessionID = dr.Field<string>("sessionID"),
                    CreationDate = dr.Field<DateTime>("creationDate"),
                    FileInfo = new NeeoFileInfo()
                    {
                        Length = dr.Field<long>("size"),
                        MediaType = (MediaType)dr.Field<byte>("mediaType"),
                        MimeType = (MimeType)dr.Field<byte>("mimeType"),
                        Name = dr.Field<string>("fileID"),
                        Hash = dr.Field<string>("hash"),
                        FullPath = dr.Field<string>("fullPath"),
                        Creator = dr.Field<string>("username")
                    }
                }).FirstOrDefault();
            }
            return uploadSession;
        }

        public static async Task<bool> DeleteSessionAsync(string sessionID)
        {
            var dbManager = new DbManager();
            return await dbManager.DeleteUploadSessionAsync(sessionID);
        }
    }
}
 