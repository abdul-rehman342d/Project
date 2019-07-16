using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace LibNeeo.Resource
{
    public abstract class AppResourceFactory 
    {
        public static IAppResource GetResource(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Background:
                    return new Background();
                case ResourceType.Sticker:
                    throw new NotImplementedException();
                    return null;
                default:
                    // Unreachable part
                    return null;
            }
        }
        //public IAppResource GetAppResource(ResourceType type)
        //{
        //    switch (type)
        //    {
        //        case ResourceType.Background:
        //            return new Background();
        //        case ResourceType.Sticker:
        //            throw new NotImplementedException();
        //            return null;
        //        default:
        //            // Unreachable part
        //            return null;
        //    }
        //}
    }
}
