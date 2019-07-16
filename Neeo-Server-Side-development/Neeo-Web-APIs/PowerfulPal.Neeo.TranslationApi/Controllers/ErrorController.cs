using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PowerfulPal.Neeo.SpamApi
{
    public class ErrorController : ApiController
    {
        [AcceptVerbs("POST","GET","PUT","DELETE")]
        [ActionName("405")]
        public HttpResponseMessage MethodNotAllowed405()
        {
            return Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Method not allowed");
        }
    }
}
