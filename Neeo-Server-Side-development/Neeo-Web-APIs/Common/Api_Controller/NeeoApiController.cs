using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Logger;
using Newtonsoft.Json;

namespace Common.Controllers
{
    public class NeeoApiController : ApiController
    {
        [NonAction]
        protected HttpResponseMessage SetCustomResponseMessage(object responseData, HttpStatusCode status)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, responseData);
            response.StatusCode = status;
            if (NeeoDictionaries.HttpStatusCodeDescriptionMapper.ContainsKey((int)status))
            {
                response.ReasonPhrase = NeeoDictionaries.HttpStatusCodeDescriptionMapper[(int)status];
            }
            return response;
        }

        [NonAction]
        protected void LogRequest(object value)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]))
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Request ===> " + JsonConvert.SerializeObject(value));
            }
        }
    }
}
