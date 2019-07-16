using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common;
using LibNeeo;
using LibNeeo.MUC;
using SettingsService.Models;

namespace SettingsService.Controllers
{
    [RoutePrefix("api/Group")]
    public class GroupController : ApiController
    {
        // Post: api/Group
        [Route("userGroups")]
        public HttpResponseMessage Post([FromBody] BaseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                var groupDetails = NeeoGroup.GetUserGroupsDetails(new NeeoUser(request.Uid.Trim()));
                if (groupDetails.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, groupDetails);
                }
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt16(applicationException.Message), NeeoDictionaries.HttpStatusCodeDescriptionMapper[Convert.ToInt16(applicationException.Message)]);
            }
            catch (Exception exception)
            {
                Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().GetType(), exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
