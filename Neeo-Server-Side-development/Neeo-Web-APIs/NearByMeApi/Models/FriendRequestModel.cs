using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.NearByMeApi.Models
{
    public class FriendRequestModel: BaseModel
    {
        [Required]
        public string FriendUId { get; set; }
    }
}