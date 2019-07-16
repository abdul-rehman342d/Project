using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Logger;
using PowerfulPal.Neeo.DashboardAPI.Filter;
using PowerfulPal.Neeo.DashboardAPI.Utilities;

namespace PowerfulPal.Neeo.DashboardAPI.Controllers
{
    [UserAuthentication]
    [RoutePrefix("api/v2/dashboard")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class V2DashboardController : ApiController
    {
        [HttpGet]
        [Route("statistics")]
        [ActionName("statistics")]
        public HttpResponseMessage GetAllRegisterUserStatistics()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, NeeoStatistics.GetCountryBasedUserStatistics());
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, applicationException.Message);
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Opertaion failed due to some internal error");
            }
        }
    }
}
