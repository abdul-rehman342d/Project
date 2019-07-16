using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class RecentCountryStatsHistory
    {
        public HistoryCollection WeeklyHistory { get; set; }
        public StatisticsWithTimeSpan MonthlyCounts{ get; set; }
        public StatisticsWithTimeSpan QuarterlyCounts{ get; set; }


        public RecentCountryStatsHistory(DateTime lastSyncTime)
        {
            WeeklyHistory = new HistoryCollection(lastSyncTime);
            MonthlyCounts = new StatisticsWithTimeSpan(lastSyncTime, 30);
            QuarterlyCounts = new StatisticsWithTimeSpan(lastSyncTime, 90);
        }

    }
}