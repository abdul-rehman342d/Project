using Common;
using Common.Controllers;
using Common.Models;
using LibNeeo.NearByMe;
using LibNeeo.Search;
using PowerfulPal.Neeo.NearByMeApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PowerfulPal.Neeo.NearByMeApi.Controllers
{
    /// <summary>
    /// Neeo user search api.
    /// </summary>
    [RoutePrefix("api/v1/user-search")]
    public class UserSearchController : NeeoApiController
    {
        /// <summary>
        /// Search the user by name
        /// </summary>
        /// <param name="model">A object containing search criteria</param>
        /// <returns>List of searched users</returns>
        [Route("find-by-name")]
        [HttpPost]
        public HttpResponseMessage GetNearByUser([FromBody]UserSearchModel model)
        {
            try
            {
                LogRequest(model);

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                List<SearchedUser> searchedUsers = new NeeoSearch().FindUserByName(model.UId, model.SearchText, model.Latitude, model.Longitude, model.IsCurrentLocation);

                return Request.CreateResponse(HttpStatusCode.OK, new { SearchedUsers = searchedUsers });
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
