using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class History
    {
        public string Date { get; set; }
        public string Caption { get; set; }
        public int TotalCount { get; set; }
    }
}