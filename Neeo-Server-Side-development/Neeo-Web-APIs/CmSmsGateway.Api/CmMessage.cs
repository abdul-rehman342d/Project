using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmSmsGateway.Api
{
    public class CmMessage
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public ContentType ContentType { get; set; }
        public string Body { get; set; }
        public ushort MinimumNumberofMessageParts { get; set; }
        public ushort MaximumNumberofMessageParts { get; set; }
    }
}
