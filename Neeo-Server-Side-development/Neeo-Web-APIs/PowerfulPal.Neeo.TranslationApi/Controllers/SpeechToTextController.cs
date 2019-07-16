using Common;
using Common.Controllers;
using Google.Cloud.Speech.V1;
using LibNeeo.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace PowerfulPal.Neeo.TranslationApi.Controllers
{
    //[RoutePrefix("speech-to-text/v1")]
    public class SpeechToTextController : NeeoApiController
    {
        //[Route("{langCode}")]
        public async Task<HttpResponseMessage> Post(CancellationToken cancellationToken)
        {
            byte[] fileBytes = null;
            string langCode = "en";
            string outputString = string.Empty;

            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"E:\Personal Projects\Neeo\Speech Key\Speech To Text-aa0103dc657d.json");

            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            MultipartMemoryStreamProvider provider = await Request.Content.ReadAsMultipartAsync(new MultipartMemoryStreamProvider());

            foreach (HttpContent content in provider.Contents)
            {
                if (content.Headers.ContentType == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (content.Headers.ContentDisposition.FileName == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                string contentType = content.Headers.ContentType.ToString();
                string sourceExtension = content.Headers.ContentDisposition.FileName.Split(new char[] { '.' }).Last().Split(new char[] { '"' }).First();

                //if (!MimeTypeMapping.ValidateMimeType(contentType))
                //{
                //    return Request.CreateResponse(HttpStatusCode.BadRequest);
                //}

                fileBytes = await content.ReadAsByteArrayAsync();
            }

            try
            {
                var speech = SpeechClient.Create();
                var response = speech.Recognize(new RecognitionConfig()
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                    SampleRateHertz = 16000,
                    LanguageCode = langCode,
                    EnableAutomaticPunctuation = true,
                    Model = "default"
                }, RecognitionAudio.FromFile(@"C:\Users\Zohaib Hanif\Pictures\16k_16PCM_eng.mp3"));

                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        outputString = alternative.Transcript;
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, outputString);
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
