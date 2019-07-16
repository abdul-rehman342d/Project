using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Logger;
using PowerfulPal.Neeo.DashboardAPI.Cryptography;
using PowerfulPal.Neeo.DashboardAPI.DAL;
using PowerfulPal.Neeo.DashboardAPI.Models;

namespace PowerfulPal.Neeo.DashboardAPI.Utilities
{
    public class UserRepository
    {
        public AuthenticationDetails Login(User user)
        {
            try
            {
                var authenticationDetails = new AuthenticationDetails();
                var dbManager = new DbManager();
                authenticationDetails.Username = user.UserName;
                authenticationDetails.AuthKey = dbManager.UserLogin(user.UserName, HashCreator.Create(user.Password + HashCreator.Key + user.UserName));
                if (!Utility.IsNullOrEmpty(authenticationDetails.AuthKey))
                {
                    authenticationDetails.LastSyncTime = Utility.ConvertLocalToUtc(Convert.ToDateTime(dbManager.GetLastSyncTime())).ToString(AppConstants.DateTimeFormat);
                    authenticationDetails.LastLoginTime = dbManager.GetLastLoginTime(user.UserName);
                    return authenticationDetails;
                }
                return null;
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Opertaion failed due to some internal error");
            }
        }

        public AuthenticationDetails Login(string authKey)
        {
            try
            {
                var authenticationDetails = new AuthenticationDetails();
                var dbManager = new DbManager();
                
                var userInfoDataTable = dbManager.GetUserInfo(authKey);
                if (userInfoDataTable.Rows.Count > 0)
                {
                    authenticationDetails.AuthKey = authKey;
                    authenticationDetails.Username = userInfoDataTable.Rows[0]["username"].ToString();
                    authenticationDetails.LastSyncTime = Utility.ConvertLocalToUtc(Convert.ToDateTime(userInfoDataTable.Rows[0]["lastSyncTime"])).ToString(AppConstants.DateTimeFormat);
                    authenticationDetails.LastLoginTime = userInfoDataTable.Rows[0]["lastLoginTime"].ToString();
                    return authenticationDetails;
                }
                return null;
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Opertaion failed due to some internal error");
            }
        }

        public void Logout(User user, string authKey)
        {
            try
            {
                var dbManager = new DbManager();
                if (dbManager.LogOut(user.UserName, authKey).Equals(1))
                {
                    return;
                }
                throw new ApplicationException("Opertaion failed due to some internal error");
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Opertaion failed due to some internal error");
            }
        }

        public void DeleteLoginHistory(User user)
        {
            try
            {
                var dbManager = new DbManager();
                dbManager.ClearLoginData(user.UserName);
                //    return "1";
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Opertaion failed due to some internal error");
            }
        }

        public List<string> GetLoginHistory(User user)
        {
            try
            {
                DataTable loginHistoryDataTable = new DbManager().GetLoginHistory(user.UserName);
                    if (loginHistoryDataTable.Rows.Count > 0)
                    {
                        var loginHistoryList =
                            loginHistoryDataTable.AsEnumerable().Select(row => row.Field<DateTime>("loginTime").ToString()).ToList();
                        //foreach (DataRow row in loginHistoryDataTable.Rows)
                        //{
                        //    item = new Dictionary<string, object>();
                        //    foreach (DataColumn column in loginHistoryDataTable.Columns)
                        //    {
                        //        item.Add(column.ColumnName, row[column]);
                        //    }
                        //    list.Add(item);
                        //}
                        return loginHistoryList;
                    }
                    return null;
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Opertaion failed due to some internal error");
            }
        }

    }
}