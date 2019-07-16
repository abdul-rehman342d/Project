using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using DAL;
using LibNeeo.MUC;
using Twilio;


namespace LibNeeo.IO
{
    /// <summary>
    /// Manages files for Neeo.
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly static ProfileDirectory Profile = new ProfileDirectory();
        /// <summary>
        /// 
        /// </summary>
        private readonly static GroupDirectory Group = new GroupDirectory();
        /// <summary>
        /// 
        /// </summary>
        private readonly static SharedDirectory Shared = new SharedDirectory();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CreateUserDirectory(string name)
        {
            return Profile.CreateDirectory(name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static void DeleteUserDirectory(string name)
        {
            Profile.DeleteDirectory(name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool UserDirectoryExists(string name)
        {
            return Profile.Exists(name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileCategory"></param>
        /// <returns></returns>
        public static bool Save(File file, FileCategory fileCategory)
        {
            string path = "";
            switch (fileCategory)
            {
                case FileCategory.Profile:
                    if (!Profile.Exists(file.Info.Name, out path))
                    {
                        throw new ApplicationException(CustomHttpStatusCode.InvalidUser.ToString("D"));
                    }
                    file.Info.FullPath = path;
                    break;

                case FileCategory.Shared:
                    file.Info.FullPath = Shared.GetDirectoryPath(file.Info.FullPath, file.Info.Name, file.Info.MediaType);
                    Shared.CreateDirectory(file.Info.FullPath);
                    break;

                case FileCategory.Group:
                    file.Info.FullPath = Group.CreateDirectory(file.Info.Name);
                    break;
            }

            File.Save(file);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileCategory"></param>
        /// <returns></returns>
        public static bool Delete(string fileName, FileCategory fileCategory)
        {
            string path = "";
            bool isDeleted = false;
            switch (fileCategory)
            {
                case FileCategory.Profile:
                    path = Path.Combine(Profile.GetDirectoryPath(fileName),
                        MediaUtility.AddFileExtension(fileName, MediaType.Image));
                    if (File.Exists(path))
                    {
                        isDeleted = File.Delete(path);
                    }
                    else
                    {
                        isDeleted = true;
                    }

                    break;

                case FileCategory.Shared:
                    throw new NotImplementedException();
                    break;

                case FileCategory.Group:

                    path = Path.Combine(Group.GetDirectoryPath(fileName),
                        MediaUtility.AddFileExtension(fileName, MediaType.Image));
                    if (File.Exists(path))
                    {
                        isDeleted = File.Delete(path);
                    }
                    else
                    {
                        isDeleted = true;
                    }
                    break;
            }
            return isDeleted;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileCategory"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static File GetFile(string fileName, FileCategory fileCategory, MediaType mediaType)
        {
            string path = "";
            switch (fileCategory)
            {
                case FileCategory.Profile:
                    path = Profile.GetDirectoryPath(fileName);
                    break;

                case FileCategory.Shared:
                    Network.Server server = new Network.Server()
                    {
                        LocalIP = "127.0.0.1",
                        Name = "Current Server"
                    };
                    path = Shared.GetDirectoryPath(server.GetServerNetworkPath(), fileName, mediaType);
                    break;

                case FileCategory.Group:
                    path = Group.GetDirectoryPath(fileName);
                    break;
            }
            return File.GetFile(fileName, path);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileCategory"></param>
        /// <returns></returns>
        public static bool IsExist(string fileName, FileCategory fileCategory, MediaType mediaType = MediaType.Image)
        {
            bool isExist = false;
            switch (fileCategory)
            {
                case FileCategory.Profile:
                    isExist = File.Exists(Path.Combine(Profile.GetDirectoryPath(fileName), MediaUtility.AddFileExtension(fileName, mediaType)));
                    break;

                case FileCategory.Shared:
                    Network.Server server = new Network.Server()
                   {
                       LocalIP = "127.0.0.1",
                       Name = "Current Server"
                   };
                    isExist = File.Exists(Path.Combine(Shared.GetDirectoryPath(server.GetServerNetworkPath(), fileName, mediaType), MediaUtility.AddFileExtension(fileName, mediaType)));
                    break;

                case FileCategory.Group:

                    isExist = File.Exists(Path.Combine(Group.GetDirectoryPath(fileName), MediaUtility.AddFileExtension(fileName, mediaType)));
                    break;
            }
            return isExist;
        }

    }
}
