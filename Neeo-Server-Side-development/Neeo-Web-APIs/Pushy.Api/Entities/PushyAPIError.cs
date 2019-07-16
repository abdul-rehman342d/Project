using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pushy.Api.Entities
{
    public class PushyAPIError
    {
        public string error;

        public PushyAPIError(string error)
        {
            this.error = error;
        }
    }
}
