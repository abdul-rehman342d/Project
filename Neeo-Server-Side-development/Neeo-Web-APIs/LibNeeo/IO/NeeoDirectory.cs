using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using Common;

namespace LibNeeo.IO
{
    public class NeeoDirectory
    {
        public static string RootDirectory
        {
            get
            {
                string path = ConfigurationManager.AppSettings[NeeoConstants.RootPath];
                if (NeeoUtility.IsNullOrEmpty(path))
                {
                    throw new DirectoryNotFoundException();
                }
                return path;
            }
        }

        internal void CreateDirectory(string path)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        internal void DeleteDirectory(string path)
        {
            System.IO.Directory.Delete(path);
        }

        internal bool Exists(string path)
        {
            return System.IO.Directory.Exists(path);
        }
    }
}
