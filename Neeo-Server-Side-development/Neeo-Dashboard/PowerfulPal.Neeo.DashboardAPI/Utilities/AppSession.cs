using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Logger;
using PowerfulPal.Neeo.DashboardAPI.DAL;

namespace PowerfulPal.Neeo.DashboardAPI.Utilities
{
    public class AppSession
    {
        public static bool Validate(string authKey)
        {
            try
            {
                var dbManager = new DbManager();
                if (dbManager.UpdateLastActivity(authKey).Equals(1))
                {
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw new ApplicationException("Password updating failed");
            }
        }
    }
}