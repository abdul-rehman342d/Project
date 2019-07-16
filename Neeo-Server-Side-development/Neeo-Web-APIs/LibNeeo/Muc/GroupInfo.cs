using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LibNeeo.MUC
{
    public class GroupInfo
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Owner { get; set; }
        public string Creator { get; set; }
        public string[] Participants { get; set; }
        public string CreationDate { get; set; }
    }
}
