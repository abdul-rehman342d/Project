using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using Common;
using System.Configuration;

namespace PowerfulPal.Neeo.NearByMeApi.Filter
{
    public class AuthorizationFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionExecutedContext)
        {
            IEnumerable<string> headerValues = actionExecutedContext.Request.Headers.GetValues("Autherize");
            string token = headerValues.FirstOrDefault();
            RemoteServiceCall remoteCall = new RemoteServiceCall();
            //Parameters
            string baseUrl = ConfigurationManager.AppSettings["authenticationBaseUrl"];
            string postUrl = ConfigurationManager.AppSettings["authenticationPostUrl"];
           
            dynamic result = remoteCall.restApiCall(baseUrl, postUrl, RType.POST, new
            {
                uID = "",
                key = "",
                token = token,
            });
            string output = Convert.ToString(result.username);
            if (output == null || output=="")
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
            else
            {
                
            }
        }
    }
}