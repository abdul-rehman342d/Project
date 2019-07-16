using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Cors;
using System.Web.Http.Cors;
using System.Web.ModelBinding;
using PowerfulPal.Neeo.DashboardAPI.Models;
using System.Data;

namespace PowerfulPal.Neeo.DashboardAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DashboardController : ApiController
    {
        [HttpGet]
        public string test()
        {
            return "this is test string from server";
        }

        [HttpPost]
        public HttpResponseMessage UserLogin([FromBody] LoginRequest loginRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string sessionKey = IsAuthenticated(loginRequest);
                    if (!Utility.IsNullOrEmpty(sessionKey))
                    {
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, sessionKey);
                        return response;
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid user!");

                    }
                }
                catch (Exception)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Request failed!");

                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request!");
            }
        }


        [HttpPost]
        public string UpdateUserStatus([FromBody] UpdateUserRequest updateRequest)
        {
            try
            {
                DbManager manager = new DbManager();
                return manager.UpdateStatus(updateRequest.userName, updateRequest.isActive, updateRequest.authKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public string UpdatePassword([FromBody] RequestWithUsernameAndPassword requestParameters)
        {
            try
            {
                DbManager manager = new DbManager();
                return manager.UpdatePassword(requestParameters.username, this.GenrateAuthKey(requestParameters), requestParameters.authKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public bool UpdateLastActivity([FromBody] TokenRequest tokenObj)
        {
            var dbManager = new DbManager();
            if (dbManager.UpdateLastActivity(tokenObj.token).Equals(1))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        public bool LogOut([FromBody] TokenRequest tokenObj)
        {
            var dbManager = new DbManager();
            if (dbManager.LogOut(tokenObj.token).Equals(1))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        public string RegisterUser([FromBody] RequestWithUsernameAndPassword requestParameters)
        {
            try
            {
                DbManager manager = new DbManager();
                return manager.RegisterUser(requestParameters.username, this.GenrateAuthKey(requestParameters), requestParameters.authKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public string GetUserName([FromBody] TokenRequest tokenObj)
        {
            var dbManager = new DbManager();
            return dbManager.GetUserName(tokenObj.token);
        }

        [HttpGet]
        public object GetLoginHistory(string id)
        {
            try
            {
                DataTable loginHistory = new DbManager().GetLoginHistory(id);
                if (loginHistory.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                    Dictionary<string, object> item = null;
                    foreach (DataRow row in loginHistory.Rows)
                    {
                        item = new Dictionary<string, object>();
                        foreach (DataColumn column in loginHistory.Columns)
                        {
                            item.Add(column.ColumnName, row[column]);
                        }
                        list.Add(item);
                    }
                    return list;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        public string GetLastLoginTime(string id)
        {
            DbManager manager = new DbManager();
            return manager.GetLastLoginTime(id);
        }

        [HttpGet]
        public string GetLastSyncTime()
        {
            DbManager manager = new DbManager();
            return manager.GetLastSyncTime();
        }

        [HttpGet]
        public string GetAllDownloadCount()
        {
            return CountryCount.GetAllDownloadCount();
        }

        [HttpGet]
        public string GetIOSDownloadCount()
        {
            return CountryCount.GetIOSCount();
        }

        [HttpGet]
        public string GetAndroidDownloadCount()
        {
            return CountryCount.GetAndroidCount();
        }

        [HttpGet]
        public object GetAllUser()
        {
            try
            {
                DataTable allUser = new DbManager().GetAllUser();
                if (allUser.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                    Dictionary<string, object> item = null;
                    foreach (DataRow row in allUser.Rows)
                    {
                        item = new Dictionary<string, object>();
                        foreach (DataColumn column in allUser.Columns)
                        {
                            item.Add(column.ColumnName, row[column]);
                        }
                        list.Add(item);
                    }
                    return list;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpGet]
        public string DeleteUser(string id)
        {

            if (!id.Equals("webadmin"))
            {
                try
                {
                    DbManager manager = new DbManager();
                    return manager.DeleteUser(id);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        [HttpGet]
        public string ClearLoginHistory(string id)
        {
            try
            {
                new DbManager().ClearLoginData(id);
                return "1";
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET api/dashboard
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET api/dashboard/5
        public string GetValue(int id)
        {
            return "value";
        }

        // POST api/dashboard
        public void Post([FromBody] string value)
        {
        }

        // PUT api/dashboard/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/dashboard/5
        public void Delete(int id)
        {
        }

        private string IsAuthenticated(LoginRequest loginCredentials)
        {
            const string secretKey = "@%^N#3OP@L~!$";
            string authKey = Utility.GenerateMd5Hash(loginCredentials.password + secretKey + loginCredentials.username);
            var dbManager = new DbManager();
            string sessionkey = dbManager.UserLogin(loginCredentials.username, authKey);
            if (!Utility.IsNullOrEmpty(sessionkey))
            {
                return sessionkey;
            }
            else
            {
                return null;
            }
        }

        private string GenrateAuthKey(RequestWithUsernameAndPassword loginCredentials)
        {
            return Utility.GenerateMd5Hash(loginCredentials.password + "@%^N#3OP@L~!$" + loginCredentials.username);
        }



    }
}