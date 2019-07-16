using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class StatisticsWithTimeSpan
    {
        public DateTime UpperBoundDate{get;set;}
        public DateTime LowerBoundDate { get; set; }
        public int TotalCount { get; set; }

        public StatisticsWithTimeSpan(DateTime lastSyncTime,int DifferenceInDays)
        {
            UpperBoundDate = lastSyncTime.Subtract(new TimeSpan(1, 0, 0, 0));
            LowerBoundDate = lastSyncTime.Subtract(new TimeSpan(DifferenceInDays, 0, 0, 0));

        }
    }

 
}