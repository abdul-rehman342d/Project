using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace LibNeeo.IO
{
    public class ProfileDirectory : NeeoDirectory
    {
        const string Profile = "Profile";

        private static string ProfileDirectoryPath
        {
            get
            {
                return Path.Combine(RootDirectory, Profile);
                //return RootDirectory;
            }
        }

        public string GetDirectoryPath(string name)
        {
            return Path.Combine(ProfileDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit), name, Profile);
        }

        public new string CreateDirectory(string name)
        {
            string path = Path.Combine(ProfileDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit), name, Profile);
            if (!base.Exists(path))
            {
                base.CreateDirectory(path);
            }
            return path;
        }

        public new void DeleteDirectory(string name)
        {
            string path = Path.Combine(ProfileDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit), name, Profile);
            if (base.Exists(path))
            {
                base.DeleteDirectory(path);
            }
        }

        public new bool Exists(string name)
        {
            string path = Path.Combine(ProfileDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit), name, Profile);
            return base.Exists(path);
        }

        public bool Exists(string name, out string path)
        {
            string dirPath = Path.Combine(ProfileDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit), name, Profile);
            if (base.Exists(dirPath))
            {
                path = dirPath;
                return true;
            }
            path = "";
            return false;
        }

            
    }
}
