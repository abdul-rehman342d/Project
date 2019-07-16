using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using Logger;
using Newtonsoft.Json;
using PowerfulPal.Neeo.DashboardAPI.DAL;
using PowerfulPal.Neeo.DashboardAPI.Models;


namespace PowerfulPal.Neeo.DashboardAPI.Utilities
{
    public class NeeoStatistics
    {
        private static readonly DbManager DbManager = new DbManager();
        private static readonly string FilePath = HttpContext.Current.Server.MapPath(@"\CountryData\CountryList.json");

        public static UserStatistics GetCountryBasedUserStatistics()
        {
            DateTime lastSyncTime = Utility.ConvertLocalToUtc(Convert.ToDateTime(DbManager.GetLastSyncTime()));
            
            var userStats = new UserStatistics(lastSyncTime);
            try
            {
                DataTable dtAllRegisteredUsers = DbManager.GetAllRegisteredUsers();
                UpdateActiveUsersCount(lastSyncTime, userStats);
                var countryCodeDictionary = new Dictionary<string, string>();
                
                if (File.Exists(FilePath))
                {
                    string fileContent = File.ReadAllText(FilePath);
                    if (!Utility.IsNullOrEmpty(fileContent))
                    {
                        countryCodeDictionary =
                               JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);
                    }
                }
                else
                {
                    countryCodeDictionary = new Dictionary<string, string>();
                }

                if (dtAllRegisteredUsers.Rows.Count > 0)
                {
                    int countryCode = 0;
                    for (int i = 0; i < dtAllRegisteredUsers.Rows.Count; i++)
                    {
                        //try
                        //{
                            var devicePlatform = (DevicePlatform)Convert.ToInt16(dtAllRegisteredUsers.Rows[i]["devicePlatform"]);
                            var creationDate = Convert.ToDateTime(dtAllRegisteredUsers.Rows[i]["creationDate"]);
                            countryCode = Utility.GetCountry(dtAllRegisteredUsers.Rows[i]["username"].ToString());

                            if (userStats.CountryStats.ContainsKey(countryCode))
                            {
                                var countryStats = userStats.CountryStats[countryCode];
                                UpdateCountryStatisticsForPlatform(devicePlatform, creationDate, countryStats);
                            }
                            else
                            {
                                var countryStats = new CountryStatistics(lastSyncTime);

                                if (countryCodeDictionary.ContainsKey(countryCode.ToString()))
                                {
                                    countryStats.CountryName = countryCodeDictionary[countryCode.ToString()];
                                }
                                else
                                {
                                    countryStats.CountryName = Utility.GetCountryNameOnline(countryCode.ToString(),
                                        FilePath, countryCodeDictionary);
                                }
                                UpdateCountryStatisticsForPlatform(devicePlatform, creationDate, countryStats);
                                userStats.CountryStats.Add(countryCode, countryStats);
                            }
                        //}
                        //catch (ApplicationException applicationException)
                        //{
                        //}
                        //catch (Exception exception)
                        //{
                        //}
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Opertaion failed due to some internal error");
            }

            return userStats;
        }

        private static void UpdateCountryStatisticsForPlatform(DevicePlatform devicePlatform, DateTime creationDate, CountryStatistics countryStats)
        {
            UserWeeklyStatistics( devicePlatform, creationDate,  countryStats);
            UserMonthlyStatistics( devicePlatform, creationDate,  countryStats);
            UserQuarterlyStatistics(devicePlatform, creationDate, countryStats);
        }
        private static void UserWeeklyStatistics(DevicePlatform devicePlatform, DateTime creationDate, CountryStatistics countryStats)
        {
            if (devicePlatform == DevicePlatform.Android)
            {
                bool isMatched = false;
                countryStats.Android.TotalCount += 1;
                for (int x = 0; x < 7; x++)
                {
                    if (creationDate.ToString("d") == countryStats.Android.RecentHistory.WeeklyHistory[x].Date)
                    {
                        countryStats.Android.RecentHistory.WeeklyHistory[x].TotalCount += 1;
                        countryStats.RecentHistory.WeeklyHistory[x].TotalCount += 1;
                        isMatched = true;
                        break;
                    }
                }
                if (!isMatched)
                {
                    countryStats.Android.PreviousCountBeforeHistory += 1;
                }
            }
            else if (devicePlatform == DevicePlatform.iOS)
            {
                bool isMatched = false;
                countryStats.Ios.TotalCount += 1;
                for (int x = 0; x < 7; x++)
                {
                    if (creationDate.ToString("d") == countryStats.Ios.RecentHistory.WeeklyHistory[x].Date)
                    {
                        countryStats.Ios.RecentHistory.WeeklyHistory[x].TotalCount += 1;
                        countryStats.RecentHistory.WeeklyHistory[x].TotalCount += 1;
                        isMatched = true;
                        break;
                    }
                }
                if (!isMatched)
                {
                    countryStats.Ios.PreviousCountBeforeHistory += 1;
                }
            }
        }
       
        private static void UserMonthlyStatistics(DevicePlatform devicePlatform, DateTime creationDate, CountryStatistics countryStats)
        {
            if (creationDate >= countryStats.RecentHistory.MonthlyCounts.LowerBoundDate && creationDate <= countryStats.RecentHistory.MonthlyCounts.UpperBoundDate)
            {
                if (devicePlatform == DevicePlatform.Android)
                {

                    countryStats.Android.RecentHistory.MonthlyCounts.TotalCount += 1;
                    countryStats.RecentHistory.MonthlyCounts.TotalCount += 1;


                }
                else if (devicePlatform == DevicePlatform.iOS)
                {
                    countryStats.Ios.RecentHistory.MonthlyCounts.TotalCount += 1;
                    countryStats.RecentHistory.MonthlyCounts.TotalCount += 1;
                }
            }

        }

        private static void UserQuarterlyStatistics(DevicePlatform devicePlatform, DateTime creationDate, CountryStatistics countryStats)
        {
            if (creationDate >= countryStats.RecentHistory.QuarterlyCounts.LowerBoundDate && creationDate <= countryStats.RecentHistory.QuarterlyCounts.UpperBoundDate)
            {
                if (devicePlatform == DevicePlatform.Android)
                {

                    countryStats.Android.RecentHistory.QuarterlyCounts.TotalCount += 1;
                    countryStats.RecentHistory.QuarterlyCounts.TotalCount += 1;


                }
                else if (devicePlatform == DevicePlatform.iOS)
                {
                    countryStats.Ios.RecentHistory.QuarterlyCounts.TotalCount += 1;
                    countryStats.RecentHistory.QuarterlyCounts.TotalCount += 1;
                }
            }
        }
        
        
        private static void UpdateUserStatistics(DevicePlatform devicePlatform, DateTime creationDate, CountryStatistics countryStats)
        {
            if (devicePlatform == DevicePlatform.Android)
            {
                bool isMatched = false;
                countryStats.Android.TotalCount += 1;
                for (int x = 0; x < 7; x++)
                {
                    if (creationDate.ToString("d") == countryStats.Android.RecentHistory.WeeklyHistory[x].Date)
                    {
                        countryStats.Android.RecentHistory.WeeklyHistory[x].TotalCount += 1;
                        countryStats.RecentHistory.WeeklyHistory[x].TotalCount += 1;
                        isMatched = true;
                        break;
                    }
                }
                if (!isMatched)
                {
                    countryStats.Android.PreviousCountBeforeHistory += 1;
                }
            }
            else if (devicePlatform == DevicePlatform.iOS)
            {
                bool isMatched = false;
                countryStats.Ios.TotalCount += 1;
                for (int x = 0; x < 7; x++)
                {
                    if (creationDate.ToString("d") == countryStats.Ios.RecentHistory.WeeklyHistory[x].Date)
                    {
                        countryStats.Ios.RecentHistory.WeeklyHistory[x].TotalCount += 1;
                        countryStats.RecentHistory.WeeklyHistory[x].TotalCount += 1;
                        isMatched = true;
                        break;
                    }
                }
                if (!isMatched)
                {
                    countryStats.Ios.PreviousCountBeforeHistory += 1;
                }
            }
        }

        private static void UpdateActiveUsersCount(DateTime lastSyncTime, UserStatistics userStatistics)
        {
            string upperTimeLimit = Utility.ConvertToUnixTimestamp(lastSyncTime).ToString("D15");
            string lowerTimeLimit = Utility.ConvertToUnixTimestamp(lastSyncTime.AddHours(-24)).ToString("D15");
            DataTable activeUsersCountTable = DbManager.GetActiveUsersCount(upperTimeLimit, lowerTimeLimit);
            if (activeUsersCountTable.Rows.Count > 0)
            {
                userStatistics.Android.Last24HrActiveUsers =
                    activeUsersCountTable.AsEnumerable()
                        .Where(row => row.Field<string>("devicePlatform") == "Android")
                        .Select(y => y.Field<int>("Count"))
                        .Single();
                userStatistics.Ios.Last24HrActiveUsers =
                    activeUsersCountTable.AsEnumerable()
                        .Where(row => row.Field<string>("devicePlatform") == "IOS")
                        .Select(y => y.Field<int>("Count"))
                        .Single();
            }
        }
    }
}