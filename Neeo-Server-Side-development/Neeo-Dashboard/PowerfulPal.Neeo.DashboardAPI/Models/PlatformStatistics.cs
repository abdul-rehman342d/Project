using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class PlatformStatistics : IStatistics
    {

        public RecentCountryStatsHistory RecentHistory { get; set; }
        public int TotalCount { get; set; }
        public int PreviousCountBeforeHistory { get; set; }
        public int Last24HrActiveUsers { get; set; }
       

        [JsonIgnore]
        private bool SerializeActiveUsersInLast24Hours { get; set; }

        public bool ShouldSerializeActiveUsersInLast24Hours()
        {
            return SerializeActiveUsersInLast24Hours;
        }

        public PlatformStatistics(DateTime lastSyncTime, bool serializeOptionalProperties = false)
        {

            RecentHistory = new RecentCountryStatsHistory(lastSyncTime);
            SerializeActiveUsersInLast24Hours = serializeOptionalProperties;

        }
    }
}