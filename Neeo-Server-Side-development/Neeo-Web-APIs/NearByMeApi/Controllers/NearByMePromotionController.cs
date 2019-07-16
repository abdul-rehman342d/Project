using Common;
using Common.Controllers;
using Compression;
using LibNeeo.NearByMe;
using LibNeeo.NearByMe.Model;
using Microsoft.AspNetCore.Http;
using PowerfulPal.Neeo.NearByMeApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;


namespace PowerfulPal.Neeo.NearByMeApi.Controllers
{
    /// <summary>
    /// This near by me service for Neeo Messenger.
    /// </summary>
    [RoutePrefix("api/v1/near-by-me-promotion")]
    public class NearByMePromotionController : NeeoApiController
    {
      readonly  NearByMePromotionManager nearByMePromotionManager = new NearByMePromotionManager();

        /// <summery>
        /// this api Create a new Promotion
        /// </summery>
        /// <param name="promotion">
        /// Field Name: username , Data Type: string, Description: mandatory parameter
        /// Field Name: description , Data Type: string, Description: mandatory parameter
        /// Field Name: status , Data Type: Byte, Description: mandatory parameter
        /// Field Name: runUntill , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: audienceMaxAge , Data Type: Byte, Description: mandatory parameter
        /// Field Name: audienceMinAge , Data Type: Byte, Description: mandatory parameter
        /// Field Name: packageId , Data Type: int, Description: mandatory parameter
        /// Field Name: locations , Data Type: string, Description: mandatory parameter
        /// Field Name: audienceGender , Data Type: Byte, Description: mandatory parameter
        /// Field Name: audienceInterests , Data Type: string, Description: mandatory parameter
        /// Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: ImagesXml , Data Type: Collection of Images, Description: each Image comprises of following fields
        ///         Field Name: imageCaption , Data Type: string, Description: mandatory parameter
        ///         Field Name: imagePath , Data Type: string, Description: mandatory parameter
        ///         Field Name: promotionId , Data Type: int, Description: mandatory parameter
        ///         Field Name: featuredImage , Data Type: Boolean, Description: mandatory parameter
        ///         Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
        ///         Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: PromotionPackagesXml , Data Type: Collection of Packages, Description: each Packages comprises of following fields
        ///         Field Name: packageId , Data Type: int, Description: mandatory parameter
        ///         Field Name: promotionId , Data Type: int, Description: mandatory parameter
        ///         Field Name: runUntill , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: FilesViewModel , Data Type: Collection of IFormFile, Description: mandatory parameter
        /// </param>
        /// <remarks>Response: On succesfully execution this api return 200 status code</remarks>
        //[Route("CreatePromotion")]
        //[HttpPost]
        //public async Task<HttpResponseMessage> AddNearByMePromotion([FromBody]LibNeeo.NearByMe.NearByMePromotion promotion)
        //{
        //    try
        //    {
        //        LogRequest(promotion);
        //        if (!ModelState.IsValid)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //        }
        //        string path = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["PromotionImagesPath"]);
        //        bool operationCompleted = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.InsertNearByMePromotion(promotion, path));
        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    catch (ApplicationException applicationException)
        //    {
        //        return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt16(applicationException.Message), NeeoDictionaries.HttpStatusCodeDescriptionMapper[Convert.ToInt16(applicationException.Message)]);
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().GetType(), exception.Message, exception);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //}

        /// <summery>
        /// This api save images.
        /// </summery>
        /// <param name="files">
        /// Field Name: files , Data Type: Collection of IFormFile, Description: mandatory parameter
        /// </param>
        /// <remarks>Response: On succesfully execution this api return 200 status code and images list with name and path</remarks>
   

        [Route("uploadImage")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadImage()
        {
            try
            {
                var httpContext = HttpContext.Current;
                List<FileDetails> images = new List<FileDetails>();
                // Check for any uploaded file  
                if (httpContext.Request.Files.Count > 0)
                {
                    //Loop through uploaded files  
                    for (int i = 0; i < httpContext.Request.Files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                        if (httpPostedFile != null /*&& httpPostedFile.ContentType==*/)
                        {
                            // Construct file save path
                            string newfileName = httpPostedFile.FileName.Remove(httpPostedFile.FileName.Length - 4, 4) + "_u_" + Guid.NewGuid() + ".jpg";
                            var fileSavePath = Path.Combine(HostingEnvironment.MapPath("~"+ConfigurationManager.AppSettings["PromotionImagesPath"]), newfileName);
                            var returnpath = ConfigurationManager.AppSettings["imagesBaseUrl"] + ConfigurationManager.AppSettings["PromotionImagesPath"] + newfileName;
                            // Save the uploaded file  
                            httpPostedFile.SaveAs(fileSavePath);

                            FileDetails currentImage = new FileDetails();
                            currentImage.Name = httpPostedFile.FileName;
                            currentImage.Path = returnpath;
                            images.Add(currentImage);
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK,images);
                }
                return Request.CreateResponse(HttpStatusCode.LengthRequired);


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
        /// this api Create a new Promotion with Path
        /// </summery>
        /// <param name="promotion">
        /// Field Name: username , Data Type: string, Description: mandatory parameter
        /// Field Name: description , Data Type: string, Description: mandatory parameter
        /// Field Name: status , Data Type: Byte, Description: mandatory parameter
        /// Field Name: runUntill , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: audienceMaxAge , Data Type: Byte, Description: mandatory parameter
        /// Field Name: audienceMinAge , Data Type: Byte, Description: mandatory parameter
        /// Field Name: packageId , Data Type: int, Description: mandatory parameter
        /// Field Name: locations , Data Type: string, Description: mandatory parameter
        /// Field Name: audienceGender , Data Type: Byte, Description: mandatory parameter
        /// Field Name: audienceInterests , Data Type: string, Description: mandatory parameter
        /// Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: ImagesXml , Data Type: Collection of Images, Description: each Image comprises of following fields
        ///         Field Name: imageCaption , Data Type: string, Description: mandatory parameter
        ///         Field Name: imagePath , Data Type: string, Description: mandatory parameter
        ///         Field Name: promotionId , Data Type: int, Description: mandatory parameter
        ///         Field Name: featuredImage , Data Type: Boolean, Description: mandatory parameter
        ///         Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
        ///         Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: PromotionPackagesXml , Data Type: Collection of Packages, Description: each Packages comprises of following fields
        ///         Field Name: packageId , Data Type: int, Description: mandatory parameter
        ///         Field Name: promotionId , Data Type: int, Description: mandatory parameter
        ///         Field Name: runUntill , Data Type: DateTime, Description: mandatory parameter
        /// </param>
        /// <remarks>Response: On succesfully execution this api return 200 status code</remarks>
        [Route("CreatePromotion")]
        [HttpPost]
        public async Task<HttpResponseMessage> AddNearByMePromotionWithPath([FromBody]LibNeeo.NearByMe.NearByMePromotion promotion)
        {
            try
            {
                LogRequest(promotion);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                bool operationCompleted = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.InsertNearByMePromotion(promotion));
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
        /// this api Create a new Promotion with Path
        /// </summery>
        /// <param name="promotion">
        /// Field Name: promotionId , Data Type: int, Description: mandatory parameter
        /// Field Name: username , Data Type: string, Description: mandatory parameter
        /// Field Name: description , Data Type: string, Description: mandatory parameter
        /// Field Name: status , Data Type: Byte, Description: mandatory parameter
        /// Field Name: runUntill , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: audienceMaxAge , Data Type: Byte, Description: mandatory parameter
        /// Field Name: audienceMinAge , Data Type: Byte, Description: mandatory parameter
        /// Field Name: packageId , Data Type: int, Description: mandatory parameter
        /// Field Name: locations , Data Type: string, Description: mandatory parameter
        /// Field Name: audienceGender , Data Type: Byte, Description: mandatory parameter
        /// Field Name: audienceInterests , Data Type: string, Description: mandatory parameter
        /// Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: ImagesXml , Data Type: Collection of Images, Description: each Image comprises of following fields
        ///         Field Name: imageCaption , Data Type: string, Description: mandatory parameter
        ///         Field Name: imagePath , Data Type: string, Description: mandatory parameter
        ///         Field Name: promotionId , Data Type: int, Description: mandatory parameter
        ///         Field Name: featuredImage , Data Type: Boolean, Description: mandatory parameter
        ///         Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
        ///         Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: PromotionPackagesXml , Data Type: Collection of Packages, Description: each Packages comprises of following fields
        ///         Field Name: packageId , Data Type: int, Description: mandatory parameter
        ///         Field Name: promotionId , Data Type: int, Description: mandatory parameter
        ///         Field Name: runUntill , Data Type: DateTime, Description: mandatory parameter
        /// </param>
        /// <remarks>Response: On succesfully execution this api return 200 status code</remarks>
        [Route("UpdatePromotion")]
        [HttpPut]
        public async Task<HttpResponseMessage> UpdateNearByMePromotionWithPath([FromBody]LibNeeo.NearByMe.NearByMePromotion promotion)
        {
            try
            {

                LogRequest(promotion);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                bool operationCompleted = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.UpsertNearByMePromotion(promotion));
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
        /// this api Update a existing Promotion
        /// </summery>
        /// <param name="promotion">
        /// Field Name: promotionId , Data Type: int, Description: mandatory parameter
        /// Field Name: username , Data Type: string, Description: mandatory parameter
        /// Field Name: description , Data Type: string, Description: mandatory parameter
        /// Field Name: status , Data Type: Byte, Description: mandatory parameter
        /// Field Name: runUntill , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: audienceMaxAge , Data Type: Byte, Description: mandatory parameter
        /// Field Name: audienceMinAge , Data Type: Byte, Description: mandatory parameter
        /// Field Name: packageId , Data Type: int, Description: mandatory parameter
        /// Field Name: locations , Data Type: string, Description: mandatory parameter
        /// Field Name: audienceGender , Data Type: Byte, Description: mandatory parameter
        /// Field Name: audienceInterests , Data Type: string, Description: mandatory parameter
        /// Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: ImagesXml , Data Type: Collection of Images, Description: each Image comprises of following fields
        ///         Field Name: imageId , Data Type: int64, Description: mandatory parameter
        ///         Field Name: imageCaption , Data Type: string, Description: mandatory parameter
        ///         Field Name: imagePath , Data Type: string, Description: mandatory parameter
        ///         Field Name: promotionId , Data Type: int, Description: mandatory parameter
        ///         Field Name: featuredImage , Data Type: Boolean, Description: mandatory parameter
        ///         Field Name: createdDate , Data Type: DateTime, Description: mandatory parameter
        ///         Field Name: updatedDate , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: PromotionPackagesXml , Data Type: Collection of Packages, Description: each Packages comprises of following fields
        ///         Field Name: packageId , Data Type: int, Description: mandatory parameter
        ///         Field Name: promotionId , Data Type: int, Description: mandatory parameter
        ///         Field Name: runUntill , Data Type: DateTime, Description: mandatory parameter
        /// Field Name: FilesViewModel , Data Type: Collection of IFormFile, Description: mandatory parameter
        /// </param>
        /// <remarks>Response: On succesfully execution this api return 200 status code</remarks>
        //[HttpPut]
        //[Route("UpdatePromotion")]
        //public async Task<HttpResponseMessage> UpdateNearByMePromotion([FromBody]LibNeeo.NearByMe.NearByMePromotion promotion)
        //{
        //    try
        //    {

        //        LogRequest(promotion);
        //        if (!ModelState.IsValid)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //        }
        //        string path = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["PromotionImagesPath"]);
        //        bool operationCompleted = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.UpsertNearByMePromotion(promotion,path));
        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    catch (ApplicationException applicationException)
        //    {
        //        return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt16(applicationException.Message), NeeoDictionaries.HttpStatusCodeDescriptionMapper[Convert.ToInt16(applicationException.Message)]);
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().GetType(), exception.Message, exception);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //}

        /// <summery>
        /// this api Update a existingpromotion Status
        /// </summery>
        /// <param name="promotionId">
        /// Field Name: promotionId , Data Type: int, Description: mandatory parameter
        /// </param>
        /// <param name="status">
        /// Field Name: status , Data Type: Byte, Description: mandatory parameter
        /// </param>
        /// <remarks>Response: On succesfully execution this api return 200 status code</remarks>
        [HttpPut]
        [Route("UpdatePromotionStatus")]
        public async Task<HttpResponseMessage> UpdateNearByMePromotionStatus(int promotionId, Byte status)
        {
            try
            {
                LogRequest("promotionId"+ promotionId+ ",status"+ status);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                bool operationCompleted = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.UpsertNearByMePromotionStatus(promotionId, status));
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
        ///  This API Gets  near-by-me-promotion Against Requested  nearByMePromotionsById"
        /// </summary>
        /// <param name="promotionId">
        /// Field Name :promotionId,  DataType: Int , Description:  promotionId is target by the  details of singal user
        /// <remarks>Response: HTTP Status 200 And HttpStatusCode.Ok indicates that the request succeeded</remarks>
        /// <returns> All Details About User  </returns>
        [HttpGet]
        [Route("GetPromotionById")]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetNearByMePromotionById(int promotionId)
        {
            try
            {
                LogRequest("UId: " + promotionId);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                var userPromotions = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.GetNearByMePromotionById(promotionId));

                return Request.CreateResponse(HttpStatusCode.OK, userPromotions);
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
        ///  This API Gets  near-by-me-promotion Against Requested  personalNearByMePromotionsPersonal
        /// </summary>
        /// <param name="username">
        /// Field Name :username,  DataType: Nvarchar , Description:  username is target by the Promotion 
        /// </param>
        ///  <remarks>Response: HTTP Status 200 And HttpStatusCode.Ok indicates that the request succeeded  </remarks>
        [HttpGet]
        [Route("GetPersonalizedPromotionsForUser")]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetPersonalNearByMePromotionByUserName(string username, string advertiser=null)
        {
            try
            {
                LogRequest("username" + username + ",advertiser" + advertiser);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                var userPromotions = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.GetPersonalNearByMePromotionByUserName(username,advertiser));

                return Request.CreateResponse(HttpStatusCode.OK, userPromotions);
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
        ///  This API Gets  near-by-me-promotion Against Requested  AdvertisedNearByMePromotions
        /// </summary>
        /// <param name="username">
        ///  Field Name :username,  DataType: Nvarchar , Description:  username is target by the Promotion 
        /// <remarks>Response: HTTP Status 200 And HttpStatusCode.Ok indicates that the request succeeded</remarks>
        [Route("GetAdvertiserPromotions")]
        [HttpGet]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetAdvertisedNearByMePromotionByUserName(string username)
        {
            try
            {
                LogRequest("UId: " + username);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                var userPromotions = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.GetAdvertisedNearByMePromotionByUserName(username));

                return Request.CreateResponse(HttpStatusCode.OK, userPromotions);
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
        ///  This API Gets  near-by-me-promotion Against Requested  TopNearByMePromotions
        /// </summary>
        /// <param name="username">
        ///   Field Name :username,  DataType: Nvarchar , Description:  username is target by the Promotion 
        /// <remarks>Response: HTTP Status 200 And HttpStatusCode.Ok indicates that the request succeeded</remarks>
        /// <returns> list of Top Country Name With Users Promotions  </returns>
        [HttpGet]
        [Route("GetTopNPersonalizedPromotionsForUser")]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetTopNearByMePromotionByUserName(string username,int top, string advertiser = null)
        {
            try
            {
                LogRequest("UId: " + username+ "top: " + username+ "advertiser: " + advertiser);
                
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                var userPromotions = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.GetTopNearByMePromotionByUserName(username, top, advertiser));

                return Request.CreateResponse(HttpStatusCode.OK, userPromotions);
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
        ///  This API Gets  near-by-me-promotion Against Requested   AllAdvertisedAccounts
        /// </summary>
        /// <remarks>Response: On Succesfull Execution</remarks>
        /// <returns>Array Of Country Objects Is Returned</returns>
        /// 
        [Route("GetAllAdvertisedAccounts")]
        [HttpGet]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetAllAdvertisedAccounts()
        {
            try
            {
                LogRequest("GetAllAdvertisedAccounts");
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                var AllAdvertisedAccounts = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.GetAllAdvertisedAccounts());

                return Request.CreateResponse(HttpStatusCode.OK, AllAdvertisedAccounts);
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
        ///  This API Gets  near-by-me-promotion Against Requested Top 5 AdvertisedAccounts
        /// </summary>
        /// <remarks>Response: On Succesfull Execution</remarks>
        /// <returns>Array Of Advertised Accounts Object Is Returned</returns>
        /// 
        [Route("GetTopFiveAdvertisedAccounts")]
        [HttpGet]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetTopFiveAdvertisedAccounts()
        {
            try
            {
                LogRequest("ProcGetTopFiveAdvertisedAccounts");
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                var AllAdvertisedAccounts = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.ProcGetTopFiveAdvertisedAccounts());

                return Request.CreateResponse(HttpStatusCode.OK, AllAdvertisedAccounts);
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
        ///  This API Gets  near-by-me-promotion Against Requested  GetAdvertisedAccountsByCountry
        /// </summary>
        /// <param name="countryId">
        /// Field Name :countryId,  DataType: Int , Description: Id of the target Country  
        /// </param>
        ///  <remarks>Response: On Succesfull Execution</remarks>
        /// <returns>>Array Of Country Objects Is Returned by  Id Wise</returns>
        [Route("GetAdvertisedAccountsByCountry")]
        [HttpGet]
        [CacheFilter(TimeDuration = 10)]
        [CompressionFilter]
        public async Task<HttpResponseMessage> GetAdvertisedAccountsByCountry(int countryId)
        {
            try
            {
                LogRequest("UId: " + countryId);
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                var AdvertisedAccountsByCountry = await System.Threading.Tasks.Task.Run(() => nearByMePromotionManager.GetAdvertisedAccountsByCountry(countryId));
                return Request.CreateResponse(HttpStatusCode.OK, AdvertisedAccountsByCountry);
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
