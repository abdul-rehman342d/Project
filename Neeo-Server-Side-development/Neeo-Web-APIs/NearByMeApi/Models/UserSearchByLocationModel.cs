using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.NearByMeApi.Models
{
    public class UserSearchByLocationModel : BaseModel
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public bool IsCurrentLocation { get; set; }
    }
}