using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class UserStatistics : IStatistics
    {
        private PlatformStatistics _android;
        private PlatformStatistics _ios;
        public RecentCountryStatsHistory _recentHistory;

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

        public int Last24HrActiveUsers
        {
            get
            {
                return Android.Last24HrActiveUsers + Ios.Last24HrActiveUsers;
            }
        }

        public RecentCountryStatsHistory RecentHistory
        {
            get
            {
                for (int i = 0; i < _recentHistory.WeeklyHistory.Count; i++)
                {
                    _recentHistory.WeeklyHistory[i].TotalCount = _android.RecentHistory.WeeklyHistory[i].TotalCount + _ios.RecentHistory.WeeklyHistory[i].TotalCount;
                }
                _recentHistory.MonthlyCounts.TotalCount = _android.RecentHistory.MonthlyCounts.TotalCount + _ios.RecentHistory.MonthlyCounts.TotalCount;
                _recentHistory.QuarterlyCounts.TotalCount = _android.RecentHistory.QuarterlyCounts.TotalCount + _ios.RecentHistory.QuarterlyCounts.TotalCount;
                return _recentHistory;
            }
            set
            {
                _recentHistory = value;
            }

        }
        public PlatformStatistics Android
        {
            get
            {
                _android.TotalCount = CountryStats.Sum(x => x.Value.Android.TotalCount);
                _android.PreviousCountBeforeHistory = CountryStats.Sum(x => x.Value.Android.PreviousCountBeforeHistory);
                for (int i = 0; i < _android.RecentHistory.WeeklyHistory.Count; i++)
                {
                    _android.RecentHistory.WeeklyHistory[i].TotalCount = CountryStats.Sum(x => x.Value.Android.RecentHistory.WeeklyHistory[i].TotalCount);
                }
                _android.RecentHistory.MonthlyCounts.TotalCount = CountryStats.Sum(x => x.Value.Android.RecentHistory.MonthlyCounts.TotalCount);
                _android.RecentHistory.QuarterlyCounts.TotalCount = CountryStats.Sum(x => x.Value.Android.RecentHistory.QuarterlyCounts.TotalCount);
                return _android;
            }
            set
            {
                _android = value;
            }
        }
        public PlatformStatistics Ios
        {
            get
            {
                _ios.TotalCount = CountryStats.Sum(x => x.Value.Ios.TotalCount);
                _ios.PreviousCountBeforeHistory = CountryStats.Sum(x => x.Value.Ios.PreviousCountBeforeHistory);
                for (int i = 0; i < _ios.RecentHistory.WeeklyHistory.Count; i++)
                {
                    _ios.RecentHistory.WeeklyHistory[i].TotalCount = CountryStats.Sum(x => x.Value.Ios.RecentHistory.WeeklyHistory[i].TotalCount);
                }
                _ios.RecentHistory.MonthlyCounts.TotalCount = CountryStats.Sum(x => x.Value.Ios.RecentHistory.MonthlyCounts.TotalCount);
                _ios.RecentHistory.QuarterlyCounts.TotalCount = CountryStats.Sum(x => x.Value.Ios.RecentHistory.QuarterlyCounts.TotalCount);
                return _ios;
            }
            set
            {
                _ios = value;
            }
        }
        public Dictionary<int, CountryStatistics> CountryStats { get; set; }

        public UserStatistics(DateTime lastSyncTime)
        {
            Android = new PlatformStatistics(lastSyncTime, true);
            Ios = new PlatformStatistics(lastSyncTime, true);
            RecentHistory = new RecentCountryStatsHistory(lastSyncTime);
            CountryStats = new Dictionary<int, CountryStatistics>();
        }
    }
}