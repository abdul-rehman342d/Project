using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common.Controllers;
using LibNeeo.NearByMe;

namespace PowerfulPal.Neeo.AdministrationApi.Controllers
{
    [RoutePrefix("v1/country")]
    public class CountryController : NeeoApiController
    {
        [Route("")]
        [HttpPost]
        public HttpResponseMessage Get()
        {
            throw new NotImplementedException();
        }
    }
}
