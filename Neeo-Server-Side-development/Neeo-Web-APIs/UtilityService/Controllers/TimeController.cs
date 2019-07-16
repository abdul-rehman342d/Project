using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Common;
using UtilityService.Models;

namespace UtilityService.Controllers
{
    public class TimeController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET","POST")]
        public HttpResponseMessage GetCurrentTime()
        {
            TimeResponse response = new TimeResponse() {Time = DateTime.UtcNow.ToString(NeeoConstants.TimestampFormat)};
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
