using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common.Controllers;

namespace PowerfulPal.Neeo.AdministrationApi.Controllers
{
    [RoutePrefix("v1/admin")]
    public class AdministrationController : NeeoApiController
    {
        [Route("activation-code/send")]
        [HttpPost]
        public HttpResponseMessage SendActivationCode()
        {
            throw new NotImplementedException();
        }

        [Route("account/register")]
        [HttpPost]
        public HttpResponseMessage RegisterAccount()
        {
            throw new NotImplementedException();
        }

        [Route("account/verify")]
        [HttpPost]
        public HttpResponseMessage VerifyAccount()
        {
            throw new NotImplementedException();
        }


    }
}
