using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace LibNeeo.IO
{
    public class GroupDirectory : NeeoDirectory
    {
        const string Group = "Group";

        private string GroupDirectoryPath
        {
            get
            {
                return Path.Combine(RootDirectory, Group);
            }
        }

        public string GetDirectoryPath(string name)
        {
            return Path.Combine(GroupDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit));
        }

        public new string CreateDirectory(string name)
        {
            string path = Path.Combine(GroupDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit));
            if (!base.Exists(path))
            {
                base.CreateDirectory(path);
            }
            return path;
        }

        public new void DeleteDirectory(string name)
        {
            string path = Path.Combine(GroupDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit));
            if (!base.Exists(path))
            {
                base.DeleteDirectory(path);
            }
        }

        public new bool Exists(string name)
        {
            string path = Path.Combine(GroupDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit));
            return base.Exists(path);
        }

        public bool Exists(string name, out string path)
        {
            string dirPath = Path.Combine(GroupDirectoryPath, MediaUtility.GetHierarchicalPath(name, NeeoConstants.HierarchyLevelLimit));
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
