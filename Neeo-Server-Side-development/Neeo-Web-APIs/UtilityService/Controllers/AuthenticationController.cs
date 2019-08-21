using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UtilityService.Controllers
{
    [RoutePrefix("Authentication")]
    public class AuthenticationController : ApiController
    {
        [Route("login")]
        [HttpGet]
        public HttpResponseMessage UserLogin()
        {
            string token = TokenManager.GenerateToken("uzair");
            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [Route("ValidateToken")]
        [BasicAutherization]
        [HttpGet]
        public HttpResponseMessage ValidateToken()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Autherized User");
        }
    }
}
