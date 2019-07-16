using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class CountryStatistics : IStatistics
    {
        public string CountryName { get; set; }
        public PlatformStatistics Android { get; set; }
        public PlatformStatistics Ios { get; set; }
        public RecentCountryStatsHistory RecentHistory{get;set;}

        public int PreviousCountBeforeHistory
        {
            get
            {
                return Android.PreviousCountBeforeHistory + Ios.PreviousCountBeforeHistory;
            }
        }
        public int TotalCount
        {
            get
            {
                return Android.TotalCount + Ios.TotalCount;
            }
        }

        
        public CountryStatistics(DateTime lastSyncTime)
        {
            Android = new PlatformStatistics(lastSyncTime);
            Ios = new PlatformStatistics(lastSyncTime);
            RecentHistory = new RecentCountryStatsHistory(lastSyncTime);
      
        }
    }
}