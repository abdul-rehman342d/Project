using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace LibNeeo.Resource
{
    public interface IAppResource
    {
        string Name { get; set; }
        ResourceType ResourceType { get; }
        bool isDefault { get; set; }
        List<string> Components { get; set; }

        void Load(object value);
        object GetComponentsUrl();
    }

}
