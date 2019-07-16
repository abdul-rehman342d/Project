using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace PowerfulPal.Neeo.NearByMeApi.Models
{
    public class NearByMePromotion
    {
        public int promotionId { get; set; }
        public string username { get; set; }
        public string description { get; set; }
        public Byte status { get; set; }
        public DateTime runUntill { get; set; }
        public Byte audienceMaxAge { get; set; }
        public Byte audienceMinAge { get; set; }
        public int packageId { get; set; }
        public string locations { get; set; }
        public Byte audienceGender { get; set; }
        public string audienceInterests { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }


    }
}