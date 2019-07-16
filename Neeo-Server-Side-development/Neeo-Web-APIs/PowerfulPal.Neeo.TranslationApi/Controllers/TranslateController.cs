using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using Common.Controllers;
using Newtonsoft.Json;
using PowerfulPal.Neeo.SpamApi.GoogleApi;
using PowerfulPal.Neeo.SpamApi.Models;

namespace PowerfulPal.Neeo.SpamApi
{
    [RoutePrefix("translate/v1")]
    public class TranslateController : NeeoApiController
    {
        public async Task<HttpResponseMessage> Get(CancellationToken cancellationToken)
        {
            var key = ConfigurationManager.AppSettings[NeeoConstants.GoogleTranslationKey];
            try
            {
                var googleApi = new GoogleApiWrapper(key);
                var translationResponse = await Task.Run(()=> googleApi.GetTranslatedText(Request.RequestUri.Query), cancellationToken);
                return Request.CreateResponse((HttpStatusCode)translationResponse["StatusCode"], translationResponse["Result"]);
            }
            catch (ApplicationException appException)
            {
                return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt32(appException.Message), "");
            }
            catch (Exception exception)
            {
                Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
