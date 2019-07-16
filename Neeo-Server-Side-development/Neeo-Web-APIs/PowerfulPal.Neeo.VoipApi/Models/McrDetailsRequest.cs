using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Entities;

namespace PowerfulPal.Neeo.VoipApi.Models
{
    public class McrDetailsRequest : BaseRequest
    {
        public bool Flush { get; set; }
    }
}