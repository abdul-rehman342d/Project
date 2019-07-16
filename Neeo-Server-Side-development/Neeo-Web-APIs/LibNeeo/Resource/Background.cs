using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using LibNeeo.Url;

namespace LibNeeo.Resource
{
    internal class Background : IAppResource
    {
        public string Name { get; set; }

        public ResourceType ResourceType
        {
            get
            {
                return Common.ResourceType.Background;
            }
        }

        public bool isDefault { get; set; }

        public List<string> Components { get; set; }

        public void Load(object value)
        {
            //var type = value.GetType();
            //var dtResourceInfo = Convert.ChangeType(value, type);
            if (value is DataTable)
            {
                var dtResourceInfo = (DataTable)value;
                if (dtResourceInfo.Rows.Count > 0)
                {
                    Name = dtResourceInfo.Rows[0]["name"].ToString();
                    isDefault = Convert.ToBoolean(dtResourceInfo.Rows[0]["isDefault"]);
                    Components = dtResourceInfo.Rows[0]["componentsDetails"].ToString().Split(new[] {','}).ToList();
                }
                else
                {
                    throw new ApplicationException("404");
                }
            }

        }

        public object GetComponentsUrl()
        {
            if (Components != null)
            {
                return NeeoUrlBuilder.BuildResourceUrls(this);
            }
            throw new ApplicationException("404");
        }
    }
}
