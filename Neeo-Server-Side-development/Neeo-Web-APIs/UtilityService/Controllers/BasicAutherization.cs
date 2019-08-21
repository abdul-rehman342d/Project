using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace UtilityService.Controllers
{
    public class BasicAutherization : ActionFilterAttribute
    {



        public override void OnActionExecuting(HttpActionContext actionExecutedContext)
        {
            IEnumerable<string> headerValues = actionExecutedContext.Request.Headers.GetValues("Autherize");
            string token = headerValues.FirstOrDefault();
            if (TokenManager.ValidateToken(token) == null)
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }

    }
}