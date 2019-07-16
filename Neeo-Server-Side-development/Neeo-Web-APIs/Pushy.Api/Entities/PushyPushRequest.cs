using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pushy.Api.Entities
{
    public class PushyPushRequest
    {
        public object data;
        public string[] registration_ids;

        public PushyPushRequest(object payload, string[] deviceTokens)
        {
            data = payload;
            registration_ids = deviceTokens;
        }
    }
}
