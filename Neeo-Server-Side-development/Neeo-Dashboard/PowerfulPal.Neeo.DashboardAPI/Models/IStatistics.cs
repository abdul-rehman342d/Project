using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public interface IStatistics
    {
        int PreviousCountBeforeHistory { get; }
        int TotalCount { get; }
        RecentCountryStatsHistory RecentHistory { get; set; }
    }
}
