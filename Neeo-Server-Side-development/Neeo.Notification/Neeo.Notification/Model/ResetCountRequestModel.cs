using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neeo.Notification.Model
{
    public class ResetCountRequestModel
    {
        [Required]
        public string uID { get; set; }
    }
}
