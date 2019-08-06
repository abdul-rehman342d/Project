using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.SMS
{
    public class SMSLog
    {
        public string messageBody { get; set; }
        public string receiver { get; set; }
        public bool isResend { get; set; }
        public bool isRegenerate { get; set; }
        public string vendorMessageId { get; set; }
        public short messageType { get; set; }
        public string appKey { get; set; }
        public string status { get; set; }
    }
}
