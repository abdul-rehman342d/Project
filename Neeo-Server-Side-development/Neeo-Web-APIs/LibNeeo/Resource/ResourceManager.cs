using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DAL;

namespace LibNeeo.Resource
{
    public class ResourceManager
    {
        public static IAppResource GetResourceInfo(string resourceId, ResourceType resourceType)
        {
            var appResource = AppResourceFactory.GetResource(resourceType); 
            var dbManager = new DbManager();
            var dtResourceInfo = dbManager.GetResourceInfo(resourceId);
            appResource.Load(dtResourceInfo);
            return appResource;
        }
    }
}
