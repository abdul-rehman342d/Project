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
using System.Web;
using System.Web.Http;


namespace PowerfulPal.Neeo.NearByMeApi.Controllers.Lookups
{
    /// <summary>
    /// This near by me service for Neeo Messenger.
    /// </summary>
     [RoutePrefix("api/v1/Country")]
    public class CountryController : NeeoApiController
    {
        NearByMeCountriesManager nearByMePromotionCountry = new NearByMeCountriesManager();

        /// <summary>
        ///  This API Gets  near-by-me-promotion Against Requested  getAllCountries"
        /// </summary>
        /// <returns>Show that all Country details</returns>
        [HttpGet]
        [Route("GetAllCountries")]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async  Task<HttpResponseMessage> GetAllCountries()
        {
            try
            {
                LogRequest("GetAllCountries");
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                List<Country> countries = await System.Threading.Tasks.Task.Run(() => nearByMePromotionCountry.GetAllCountries());
                return Request.CreateResponse(HttpStatusCode.OK, countries);
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


        [HttpGet]
        [Route("GetCountryByCode/{countryCode}")]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetCountryByCode(string countryCode)
        {
            try
            {
                LogRequest("GetCountryByCode");
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                string prePath = System.Configuration.ConfigurationManager.AppSettings["countryFlagsPath"];

                Country country = await System.Threading.Tasks.Task.Run(() => nearByMePromotionCountry.GetCountryByCode(countryCode, prePath));
                return Request.CreateResponse(HttpStatusCode.OK, country);
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
