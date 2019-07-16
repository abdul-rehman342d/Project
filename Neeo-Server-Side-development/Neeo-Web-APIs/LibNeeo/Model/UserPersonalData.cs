using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.Model
{
    public class UserPersonalData
    {
        
        public string username { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string interest { get; set; }
        public byte gender { get; set; }
        public int country { get; set; }
    }
}
