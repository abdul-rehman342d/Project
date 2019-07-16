using Common;
using Common.Controllers;
using LibNeeo;
using LibNeeo.Model;
using LibNeeo.NearByMe;
using PowerfulPal.Neeo.NearByMeApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


namespace PowerfulPal.Neeo.NearByMeApi.Controllers
{
    /// <summary>
    /// This near by me service for Neeo Messenger.
    /// </summary>
    [RoutePrefix("api/v1/user")]
    public class UserController : NeeoApiController
    {
       
        /// <summery>
        /// this api Update a existing pacakege for near by me permotion
        /// </summery>
        /// <param name="currentUser">
        /// Field Name: packageId , Data Type: int, Description: mandatory parameter
        /// Field Name: locationId , Data Type: int, Description: mandatory parameter
        /// Field Name: description , Data Type: string, Description: mandatory parameter
        /// Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: enabled , Data Type: Boolean, Description: mandatory parameter
        /// Field Name: isDeleted , Data Type: Boolean, Description: mandatory parameter
        /// </param>
        /// <remarks>Response: On succesfully execution this api return 200 status code</remarks>
        [HttpPut]
        [Route("UpdateUserPersonalData")]
        public async Task<HttpResponseMessage> UpdateUserPersonalData([FromBody]UserPersonalData model)
        {
            try
            {
                LogRequest(model);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                NeeoUser currentUser = new NeeoUser(model.username);
                bool operationCompleted = await System.Threading.Tasks.Task.Run(() => currentUser.UpdateUserPersonalData(model));
                return Request.CreateResponse(HttpStatusCode.OK);
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
