using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Logger;
using Newtonsoft.Json;
using System.Web;

namespace LibNeeo.Network
{
    public sealed class FileServerManager
    {
        private static readonly FileServerManager Instance = new FileServerManager();
        private List<Server> _servers = new List<Server>();
        private const string ServerListPath = "/ServerDetails/ServerDetails.json";

        private FileServerManager()
        {
            LoadServerList();
        }

        private void LoadServerList()
        {
            string filePath = HttpContext.Current.Server.MapPath(ServerListPath);

            if (_servers.Count == 0)
            {
                if (File.Exists(filePath))
                {
                    StreamReader reader = new StreamReader(filePath);
                    var serverDetails = reader.ReadToEnd();
                    reader.Close();
                    _servers = JsonConvert.DeserializeObject<List<Server>>(serverDetails);
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + " ===> File does not exist @ path:" + filePath);
                    throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
                }
            }
        }

        public static FileServerManager GetInstance()
        {
           
            return Instance;
        }

        public Server SelectServer()
        {
            Random random = new Random();
            int serverIndex = random.Next(0, _servers.Count);
            return _servers[serverIndex];
        }
    }
}
