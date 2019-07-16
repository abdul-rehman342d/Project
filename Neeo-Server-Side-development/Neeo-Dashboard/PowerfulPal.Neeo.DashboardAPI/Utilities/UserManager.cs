using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Logger;
using PowerfulPal.Neeo.DashboardAPI.Cryptography;
using PowerfulPal.Neeo.DashboardAPI.DAL;
using PowerfulPal.Neeo.DashboardAPI.Models;

namespace PowerfulPal.Neeo.DashboardAPI.Utilities
{
    public class UserManager
    {
        public void CreateUser(User user)
        {
            try
            {
                var dbManager = new DbManager();
                dbManager.RegisterUser(user.UserName, HashCreator.Create(user.Password + HashCreator.Key + user.UserName));
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("User creation failed");
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                DataTable allUser = new DbManager().GetAllUser();
                if (allUser.Rows.Count > 0)
                {
                    var list = new List<User>();
                    foreach (DataRow row in allUser.Rows)
                    {
                        var item = new User();
                        item.UID = Convert.ToInt32(row["uID"]);
                        item.UserName = row["userName"].ToString();
                        item.IsActive = Convert.ToBoolean(row["isActive"]);
                        list.Add(item);
                    }
                    return list;
                }
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Opertaion failed due to some internal error");
            }
            return null;
        }

        public void DeleteUser(User user)
        {
            if (!user.UserName.Equals("webadmin"))
            {
                try
                {
                    var dbManager = new DbManager();
                    dbManager.DeleteUser(user.UserName);
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    throw new ApplicationException("User deletion failed");
                }
            }
        }

        public void UpdatePassword(User user)
        {
            try
            {
                var dbManager = new DbManager();
                dbManager.UpdatePassword(user.UserName, HashCreator.Create(user.Password + HashCreator.Key + user.UserName));
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Password updating failed");
            }
        }

        public void UpdateUserActivateState(User user)
        {
            try
            {
                var dbManager = new DbManager();
                dbManager.UpdateStatus(user.UserName, user.IsActive, "");
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Opertaion failed due to some internal error");
            }
        }
    }
}