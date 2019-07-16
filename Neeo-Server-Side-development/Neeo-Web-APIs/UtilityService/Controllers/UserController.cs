using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using LibNeeo;
using LibNeeo.IO;
using Common.Entities;
using Logger;

namespace UtilityService.Controllers
{
    public class UserController : ApiController
    {
        [Route("api/user/lastseen")]
        [ActionName("LastSeen")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetLastSeen(BaseRequest request)
        {
            
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                NeeoUser user = new NeeoUser(request.Uid.Trim());
                var result = await user.GetLastSeenTimeAsync();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt16(applicationException.Message), NeeoDictionaries.HttpStatusCodeDescriptionMapper[Convert.ToInt16(applicationException.Message)]);
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                   System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }  
        }
    }
}
