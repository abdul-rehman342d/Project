using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;

namespace LibNeeo.IO
{
    public class SharedDirectory : NeeoDirectory
    {
        const string Shared = "Shared";


        public string GetDirectoryPath(string serverNetworkPath, string name, MediaType mediaType)
        {
            return Path.Combine(serverNetworkPath, Shared, mediaType.ToString("G"), MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit));
        }

        public new void CreateDirectory(string path)
        {
            if (!base.Exists(path))
            {
                base.CreateDirectory(path);
            }
        }

        public new void DeleteDirectory(string path)
        {
            //string path = Path.Combine(SharedDirectoryPath, NeeoUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit));
            if (!base.Exists(path))
            {
                base.DeleteDirectory(path);
            }
        }

        public new bool Exists(string path)
        {
            //string path = Path.Combine(SharedDirectoryPath, NeeoUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit));
            return base.Exists(path);
        }

        //public bool Exists(string name, out string path)
        //{
        //    string dirPath = Path.Combine(SharedDirectoryPath, NeeoUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit));
        //    if (base.Exists(dirPath))
        //    {
        //        path = dirPath;
        //        return true;
        //    }
        //    path = "";
        //    return false;
        //}
    }
}
