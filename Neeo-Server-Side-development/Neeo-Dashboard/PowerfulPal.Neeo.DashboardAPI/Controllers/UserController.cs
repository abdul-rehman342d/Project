using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using PowerfulPal.Neeo.DashboardAPI.DAL;
using PowerfulPal.Neeo.DashboardAPI.Filter;
using PowerfulPal.Neeo.DashboardAPI.Models;
using PowerfulPal.Neeo.DashboardAPI.Session;
using PowerfulPal.Neeo.DashboardAPI.Utilities;

namespace PowerfulPal.Neeo.DashboardAPI.Controllers
{

    [RoutePrefix("api/v2/user")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("login")]
        [ActionName("login")]
        public HttpResponseMessage Login([FromBody]User user)
        {
            if (!ModelState.IsValid || user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request!");
            }
            if (Utility.IsNullOrEmpty(user.Password))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request!");
            }
            try
            {
                var userRepository = new UserRepository();
                var authenticationDetails = userRepository.Login(user);
                if (authenticationDetails != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, authenticationDetails);
                    return response;
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid username or password!");
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }

        [HttpGet]
        [Route("login")]
        [ActionName("login")]
        [UserAuthentication]
        public HttpResponseMessage Login()
        {
            try
            {
                var userRepository = new UserRepository();
                var authenticationDetails = userRepository.Login(Request.Headers.Authorization.Parameter);
                if (authenticationDetails != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, authenticationDetails);
                    return response;
                }
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }

        [HttpPut]
        [Route("active")]
        [ActionName("active")]
        [UserAuthentication]
        public HttpResponseMessage UpdateActivateState([FromBody]User user)
        {
            //HttpContext.Current.Session["userid"] = user.UserName;
            if (!ModelState.IsValid || user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request!");
            }
            try
            {
                var userManager = new UserManager();
                userManager.UpdateUserActivateState(user);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }

        [HttpPut]
        [Route("password")]
        [ActionName("password")]
        [UserAuthentication]
        public HttpResponseMessage UpdatePassword([FromBody]User user)
        {
            if (!ModelState.IsValid || user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request!");
            }
            if (Utility.IsNullOrEmpty(user.Password))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request!");
            }
            try
            {
                var userManager = new UserManager();
                userManager.UpdatePassword(user);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }

        //[HttpPost]
        //public bool UpdateLastActivity([FromBody] TokenRequest tokenObj)
        //{
        //    var dbManager = new DbManager();
        //    if (dbManager.UpdateLastActivity(tokenObj.token).Equals(1))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        [HttpPut]
        [Route("{UserName}/logout")]
        [ActionName("logout")]
        [UserAuthentication]
        public HttpResponseMessage Logout([FromUri]User user)
        {
            if (!ModelState.IsValid || user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request!");
            }
            try
            {
                var userRepository = new UserRepository();
                userRepository.Logout(user, Request.Headers.Authorization.Parameter.Trim());
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }

        [HttpPost]
        [Route("register")]
        [ActionName("register")]
        [UserAuthentication]
        public HttpResponseMessage Register([FromBody]User user)
        {
            if (!ModelState.IsValid || user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request!");
            }
            try
            {
                var userManager = new UserManager();
                userManager.CreateUser(user);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }

        [HttpGet]
        [Route("all")]
        [ActionName("all")]
        [UserAuthentication]
        public HttpResponseMessage GetAllUser()
        {
            try
            {
                var userManager = new UserManager();
                var userList = userManager.GetAllUsers();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, userList);
                return response;
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }

        [HttpDelete]
        [Route("{UserName}")]
        [UserAuthentication]
        public HttpResponseMessage DeleteUser([FromUri]User user)
        {
            if (!ModelState.IsValid || user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad request!");
            }
            try
            {
                var userManager = new UserManager();
                userManager.DeleteUser(user);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }

        [HttpDelete]
        [Route("{UserName}/login/history")]
        [ActionName("clear-login-history")]
        [UserAuthentication]
        public HttpResponseMessage DeleteLoginHistory([FromUri]User user)
        {
            try
            {
                var userRepository = new UserRepository();
                userRepository.DeleteLoginHistory(user);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }

        [HttpGet]
        [Route("{UserName}/login/history")]
        [ActionName("login-history")]
        [UserAuthentication]
        public HttpResponseMessage GetLoginHistory([FromUri]User user)
        {
            try
            {
                var userRepository = new UserRepository();
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, userRepository.GetLoginHistory(user));
                return response;
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
        }
    }
}
