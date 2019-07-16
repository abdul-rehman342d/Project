using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neeo.Nexmo
{
    public class NexmoResponse
    {
        public string Messagecount { get; set; }
        public List<Message> Messages { get; set; }
    }
}
