using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// A static class that holds all the dictionaries used in the project.
    /// </summary>
    public static class NeeoDictionaries
    {
        /// <summary>
        /// A static dictionary containing the descriptions of http status codes.
        /// </summary>
        public static Dictionary<int, NeeoFileServer> FileServersDictionary;

        /// <summary>
        /// A static dictionary containing the descriptions of http status codes.
        /// </summary>
        public static Dictionary<int, string> HttpStatusCodeDescriptionMapper = new Dictionary<int, string>()
        {
            {304, "Not Modified."},
            {400, "Bad request."},
            {404, "Not Found."},
            {406, "Not Acceptable."},
            {530, "The given number is invalid."},
            {531, "An exception is thrown by SMS API."},
            {532, "Database transaction failed."},
            {533, "Database operation failed."},
            {534, "Request arguments are invalid."},
            {535, "File system exception occured."},
            {536, "File data is not valid (not 64 base encoding)."},
            {537, "File format is not same."},
            {538, "Server connection failed."},
            {539, "Server internal error."},
            {540, "Unknown error occured."},
            {541, "User has been blocked."},
            {542, "Invalid user."},
            {543, "Incompatible application version."},
            {544, "OS is incompatible."},
            {545, "App version is incompatible."},
            {546, "File transfer is not supported."},
            {547, "Invalid language code."}
        };

        static NeeoDictionaries()
        {
            LoadFileServersDictionary();
        }

        private static void LoadFileServersDictionary()
        {
            char[] serverDelimeter = new char[]{';'};
            char[] serverNameIPdelimeter = new char[]{':'};
            FileServersDictionary = new Dictionary<int, NeeoFileServer>();
            int counter = 0;
            string fileServerString = ConfigurationManager.AppSettings[NeeoConstants.FileServers];
            if (fileServerString != null)
            {
                var fileServers = fileServerString.Split(serverDelimeter, StringSplitOptions.None);
                if (fileServers.Length > 0)
                {
                    foreach (var fileServer in fileServers)
                    {
                        var server = fileServer.Split(serverNameIPdelimeter);
                        if (server.Length > 1)
                        {
                            FileServersDictionary.Add(counter,
                                new NeeoFileServer() {Name = server[0], LocalIP = server[0], LiveIP = server[1]});
                            counter ++;
                        }
                    }
                }
            }
        }
    }
}
