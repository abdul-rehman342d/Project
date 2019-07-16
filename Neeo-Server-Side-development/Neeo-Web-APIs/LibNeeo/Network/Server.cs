using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.Network
{
    public class Server
    {
        public string Name { get; set; }
        public string LocalIP { get; set; }
        public string LiveDomain { get; set; }

        public string GetServerNetworkPath()
        {
            const string networkPathPrefix = "\\\\";
            return networkPathPrefix + LocalIP;
        }
    }
}
