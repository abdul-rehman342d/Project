using Common;
using Common.Controllers;
using Compression;
using LibNeeo.NearByMe;
using System;
using System.Collections.Generic;
using System.Data;
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
     [RoutePrefix("api/v1/near-by-me-promotion-package")]
    public class NearByMePromotionPacakgeController : NeeoApiController
    {

        NearByMePackageManager nearByMePromotionPacakge = new NearByMePackageManager();
        NearByMeCountriesManager nearByMePromotionCountry = new NearByMeCountriesManager();
        /// <summery>
        /// this api create a new pacakege for near by me permotion
        /// </summery>
        /// <param name="pacakage">
            /// Field Name: locationId , Data Type: int, Description: mandatory parameter
            /// Field Name: description , Data Type: string, Description: mandatory parameter
            /// Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
            /// Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
            /// Field Name: enabled , Data Type: Boolean, Description: mandatory parameter
            /// Field Name: isDeleted , Data Type: Boolean, Description: mandatory parameter
        /// </param>
        /// <remarks>Response: On succesfully execution this api return 200 status code</remarks>
        [HttpPost]
        [Route("CreatePromotionPackage")]
        public async Task<HttpResponseMessage> InsertNearByMePromotionPacakage([FromBody]NearByMePromotionPackage pacakage)
        {
            try
            {
                LogRequest(pacakage);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                bool operationCompleted = await System.Threading.Tasks.Task.Run(() => nearByMePromotionPacakge.InsertNearByMePacakage(pacakage));
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

        /// <summery>
        /// this api Update a existing pacakege for near by me permotion
        /// </summery>
        /// <param name="pacakage">
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
        [Route("UpdatePromotionPackage")]
        public async Task<HttpResponseMessage> UpdateNearByMePromotionPacakage([FromBody]NearByMePromotionPackage pacakage)
        {
            try
            {
                LogRequest(pacakage);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                bool operationCompleted = await System.Threading.Tasks.Task.Run(() => nearByMePromotionPacakge.UpsertNearByMePacakage(pacakage));
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
        
        /// <summary>
        ///  This API Gets  near-by-me-promotion Against Requested  allPromotionPacakages"
        /// </summary>
        /// <returns> Show that all Promotion Pacakages list </returns>
        [HttpGet]
        [Route("GetAllPromotionPackages")]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetNearByMePromotionPacakages()
        {
            try
            {
              
                LogRequest("GetNearByMePromotionPacakages");
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                List<NearByMePromotionPackage> pacakages = await System.Threading.Tasks.Task.Run(() => nearByMePromotionPacakge.GetNearByMePromotionPackages());
                return Request.CreateResponse(HttpStatusCode.OK, pacakages);
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

        /// <summary>
        /// This API Gets  near-by-me-promotion Against Requested  promotionPacakagesByPackage"
        /// </summary>
        /// <param name="packageId">The string containing the user phone number</param>
        /// <returns>Show that users Promotion Pacakages list</returns>
        [HttpGet]
        [Route("GetPromotionPackageById/{packageId}")]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetNearByMePromotionPacakagesById(int packageId)
        {
            try
            {
                LogRequest("packageId:"+packageId);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                List<NearByMePromotionPackage> pacakages = await System.Threading.Tasks.Task.Run(() => nearByMePromotionPacakge.GetNearByMePromotionPackagesById(packageId));
                return Request.CreateResponse(HttpStatusCode.OK, pacakages);
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

        /// <summary>
        /// This API Gets  near-by-me-promotion Against Requested  promotionPacakagesByCountry"
        /// </summary>
        /// <param name="countryId">The int containing the country id</param>
        /// <returns>Show that Country wise Promotion Pacakages list</returns>
        [HttpGet]
        [Route("GetPromotionPackagesByCountry/{countryId}")]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetNearByMePromotionPackagesByCountry(int countryId)
        {
            try
            {
                LogRequest("countryId:" + countryId);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                List<NearByMePromotionPackage> pacakages = await System.Threading.Tasks.Task.Run(() =>  nearByMePromotionPacakge.GetNearByMePromotionPackagesByCountry(countryId));
                return Request.CreateResponse(HttpStatusCode.OK, pacakages);
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
