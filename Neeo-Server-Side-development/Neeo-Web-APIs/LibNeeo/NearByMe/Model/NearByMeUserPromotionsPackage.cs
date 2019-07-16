using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.NearByMe
{
    public class NearByMeUserPromotionsPackage
    {
        public int packageId { get; set; }
        public int promotionId { get; set; }
        public DateTime runUntill { get; set; }
    }
}
