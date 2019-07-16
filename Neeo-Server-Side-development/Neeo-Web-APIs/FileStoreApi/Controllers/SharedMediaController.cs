using System.Web.SessionState;
using Common;
using Common.Controllers;
using Common.Extension;
using FileStoreApi.Models;
using LibNeeo;
using LibNeeo.IO;
using LibNeeo.MediaSharing;
using LibNeeo.Url;
using Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using File = LibNeeo.IO.File;

namespace FileStoreApi.Controllers
{
    [RoutePrefix("v1/media")]
    public class SharedMediaController : NeeoApiController
    {
        private bool _logRequestResponse =
        Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="uID"></param>
        ///// <param name="data"></param>
        ///// <param name="mimeType"></param>
        ///// <param name="recipientCount"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public string Upload(string uID, string fID, string data, ushort mimeType, ushort recipientCount)
        //{
        //    ulong temp = 0;
        //    string resultUrl = null;
        //    uID = (uID != null) ? uID.Trim() : uID;
        //    fID = (fID != null) ? fID.Trim() : fID;
        //    fID = (fID != null) ? fID.Trim() : fID;

        //    #region log user request and response

        //    /***********************************************
        //     To log user request
        //     ***********************************************/
        //    if (_logRequestResponse)
        //    {
        //        LogManager.CurrentInstance.InfoLogger.LogInfo(
        //            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
        //            "Request ===> senderID : " + uID + ", mimeType : " + mimeType.ToString() + ", fID : " + fID + ", recipientCount : " + recipientCount.ToString());
        //    }

        //    #endregion

        //    //      #region Verify User
        //    //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
        //    //string keyFromClient = request.Headers["key"];
        //    //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
        //    //{
        //    //    #endregion

        //    if (NeeoUtility.IsNullOrEmpty(uID) && !ulong.TryParse(uID, out temp) && !Enum.IsDefined(typeof(MimeType), mimeType) && (recipientCount == 0 || recipientCount >= 255) &&
        //        NeeoUtility.IsNullOrEmpty(data))
        //    {
        //        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
        //    }
        //    else
        //    {
        //        if (!NeeoUtility.IsNullOrEmpty(fID))
        //        {
        //            Guid resultingGuid;
        //            if (!Guid.TryParse(fID, out resultingGuid))
        //            {
        //                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
        //            "fID : " + fID + " is not parseable.");
        //                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
        //                return resultUrl;
        //            }
        //            fID = resultingGuid.ToString("N").ToLower();
        //        }

        //        NeeoUser sender = new NeeoUser(uID);
        //        try
        //        {
        //            LibNeeo.IO.File file = new LibNeeo.IO.File() { Info = new NeeoFileInfo() { Creator = uID, MediaType = MediaType.Image, MimeType = (MimeType)mimeType, Name = fID }, Data = data };
        //            if (SharedMedia.Save(sender, file, FileCategory.Shared, recipientCount))
        //            {
        //                resultUrl = file.Info.Url;
        //            }
        //            #region log user request and response

        //            /***********************************************
        //            To log user response
        //            ***********************************************/
        //            if (_logRequestResponse)
        //            {
        //                LogManager.CurrentInstance.InfoLogger.LogInfo(
        //                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
        //                    "Response ===> " + resultUrl);
        //            }

        //            #endregion
        //        }
        //        catch (ApplicationException appExp)
        //        {
        //            NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
        //        }
        //        catch (Exception exp)
        //        {
        //            LogManager.CurrentInstance.ErrorLogger.LogError(
        //                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
        //            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
        //        }
        //    }
        //    return resultUrl;
        //    //}
        //    //else
        //    //{
        //    //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);
        //    //    return resultUrl;
        //    //}
        //}

        [HttpPost]
        [Route("uploadNew")]
        public async Task<HttpResponseMessage> Post([FromUri]string type)
        {
            try
            {
                switch (type)
                {
                    case "data":
                        return await UploadDataAsync(this);

                    case "multipart":
                        return await UploadMultipartAsync();

                    case "resumable":
                        return await UploadResumableAsync();

                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (ApplicationException appExp)
            {
                return SetCustomResponseMessage("", (HttpStatusCode)Convert.ToInt32(appExp.Message));
            }
            catch (InvalidDataException invalidDataException)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, invalidDataException.Message, invalidDataException, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (JsonReaderException jsonReaderException)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, jsonReaderException.Message, jsonReaderException);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> UploadDataAsync(ApiController controller)
        {
            if (controller.Request.Content.Headers.ContentType == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (Request.Content.Headers.ContentType.ToString().Split(new char[] { ';' })[0] != "application/json")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var requestBody = await Request.Content.ReadAsStringAsync();
            var fileRequest = JsonConvert.DeserializeObject<UploadFileRequest>(requestBody);
            controller.Validate(fileRequest);

            if (!ModelState.IsValid)
            {
                return controller.Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (!NeeoUtility.IsNullOrEmpty(fileRequest.FId))
            {
                Guid resultingGuid;
                if (!Guid.TryParse(fileRequest.FId, out resultingGuid))
                {
                    LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                "fID : " + fileRequest.FId + " is not parseable.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                fileRequest.FId = resultingGuid.ToString("N").ToLower();
            }

            NeeoUser sender = new NeeoUser(fileRequest.Uid);

            File uploadedFile = FileFactory.Create(fileRequest.MimeType.GetDescription());
            uploadedFile.Info.Name = fileRequest.FId;
            uploadedFile.Info.Creator = fileRequest.Uid;
            uploadedFile.Data = fileRequest.Data;
            //LibNeeo.IO.File uploadingFile = new LibNeeo.IO.File()
            //{
            //    Info = new NeeoFileInfo()
            //    {

            //        MediaType = MediaType.Image,
            //        MimeType = fileRequest.MimeType,

            //        Extension = "jpg"
            //    },
            //    Data = fileRequest.Data
            //};

            SharedMedia.Save(sender, uploadedFile, FileCategory.Shared, 0);

            #region log user request and response

            /***********************************************
                    To log user response
                 ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "Response ===> " + uploadedFile.Info.Url);
            }

            #endregion log user request and response

            return Request.CreateResponse(HttpStatusCode.OK,
                new Dictionary<string, string>() { { "UploadFileResult", uploadedFile.Info.Url } });
        }

        public async Task<HttpResponseMessage> UploadMultipartAsync()
        {
            string fileId = "923116024000";
            //if (!Request.Headers.Contains("uid"))
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}
            //if (Request.Headers.Contains("id"))
            //{
            //    fileId = Request.Headers.GetValues("id").First();
            //}

            string uId = fileId;
            if (Request.Content.IsMimeMultipartContent())
            {
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
                    if (!MimeTypeMapping.ValidateMimeType(contentType))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                    using (var stream = content.ReadAsStreamAsync().Result)
                    {
                        File uploadedFile = FileFactory.Create(contentType);
                        uploadedFile.Info.Name = fileId;
                        //file.Info.Extension = "." + sourceExtension;
                        uploadedFile.Info.Creator = uId;
                        uploadedFile.Info.Length = Convert.ToInt64(content.Headers.ContentLength);
                        uploadedFile.FileStream = new FileDataStream()
                        {
                            Stream = stream
                        };
                        SharedMedia.Save(new NeeoUser(uId), uploadedFile, FileCategory.Shared, 0);
                        var response = new HttpResponseMessage();
                        response.StatusCode = HttpStatusCode.OK;
                        response.Headers.Location = new Uri(uploadedFile.Info.Url);
                        return response;
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        public async Task<HttpResponseMessage> UploadResumableAsync()
        {
            var uploadingFileInfo = new NeeoFileInfo();
            if (!Request.Headers.Contains("uid"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (Request.Headers.Contains("id"))
            {
                uploadingFileInfo.Name = Request.Headers.GetValues("id").First();
            }
            else
            {
                uploadingFileInfo.Name = Guid.NewGuid().ToString("N");
            }
            if (!Request.Headers.Contains("upload-content-type"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (!Request.Headers.Contains("upload-content-length"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (!Request.Headers.Contains("filename"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var contentType = Request.Headers.GetValues("upload-content-type").First();
            if (!MimeTypeMapping.ValidateMimeType(contentType))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var mimeTypeDetail = MimeTypeMapping.GetMimeTypeDetail(contentType);
            uploadingFileInfo.Creator = Request.Headers.GetValues("uid").First();
            uploadingFileInfo.Length = Convert.ToInt64(Request.Headers.GetValues("upload-content-length").First());
            uploadingFileInfo.MimeType = mimeTypeDetail.MimeType;
            uploadingFileInfo.MediaType = mimeTypeDetail.MediaType;
            uploadingFileInfo.Extension = mimeTypeDetail.Extension;

            var session = await UploadSessionManager.CreateSessionAsync(uploadingFileInfo);
            if (session == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("id", uploadingFileInfo.Name);
            response.Headers.Location = new Uri(NeeoUrlBuilder.BuildResumableUploadUrl(session.SessionID));
            return response;
        }

        [HttpPut]
        [Route("upload")]
        public async Task<HttpResponseMessage> Put([FromUri]string type, [FromUri]string sessionID)
        {
            long startingPosition = 0;
            try
            {
                if (type != "resumable" || NeeoUtility.IsNullOrEmpty(sessionID))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                var session = await UploadSessionManager.ValidateSessionAsync(sessionID);
                if (session == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                session.FileInfo.Extension =
                    MimeTypeMapping.GetMimeTypeDetail(session.FileInfo.MimeType.GetDescription()).Extension;
                long? contentLength = Request.Content.Headers.ContentLength;
                string contentType = Request.Content.Headers.ContentType == null
                    ? null
                    : Request.Content.Headers.ContentType.ToString();

                if (Request.Content.Headers.Contains("Content-Range"))
                {
                    var contentRange = Request.Content.Headers.GetValues("Content-Range").First();

                    if (contentRange == "bytes */*")
                    {
                        var file = SharedMedia.GetMedia(session.FileInfo.Name, session.FileInfo.MediaType);
                        if (file != null)
                        {
                            var response = Request.CreateResponse((HttpStatusCode) 308);
                            response.Headers.Add("Range", "bytes=0-" + file.Info.Length);
                            response.ReasonPhrase = "Resumable Incomplete";
                            return response;
                        }
                        else
                        {
                            var response = Request.CreateResponse((HttpStatusCode)308);
                            response.Headers.Add("Range", "bytes=0-0");
                            response.ReasonPhrase = "Resumable Incomplete";
                            return response;
                        }
                    }
                    else
                    {
                        var delimeter = new char[] {'-', '/',' '};
                        var rangeValues = contentRange.Split(delimeter);
                        if (rangeValues.Length != 4)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                        if (Convert.ToInt64(rangeValues[2]) != session.FileInfo.Length)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                        startingPosition = Convert.ToInt64(rangeValues[1]);
                    }
                }

                if (contentLength == null || contentLength > (long) session.FileInfo.Length)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                using (var stream = await Request.Content.ReadAsStreamAsync())
                {
                    var newFile = new File()
                    {
                        Info = session.FileInfo,
                        FileStream = new FileDataStream()
                        {
                            Stream = stream,
                            Postion = startingPosition
                        }
                    };

                    if (SharedMedia.Save(new NeeoUser(session.FileInfo.Creator), newFile, FileCategory.Shared, 0))
                    {
                        // check the length of the file with the database then delete it.
                        await UploadSessionManager.DeleteSessionAsync(sessionID);
                        var response = new HttpResponseMessage(HttpStatusCode.Created);
                        response.Headers.Location = new Uri(newFile.Info.Url);
                        return response;
                    }
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}

/*
 * 
 * else
                {

                    using (var stream = await Request.Content.ReadAsStreamAsync())
                    {
                        File newFile = new File()
                        {
                            Info = session.FileInfo,
                            FileStream = new FileDataStream() { Stream = stream }
                        };
                        try
                        {
                            SharedMedia.Save(new NeeoUser("923458412963"), newFile, FileCategory.Shared, 0, true);
                        }
                        catch (Exception)
                        {
                        }


                    }
                }
//var fileStream = System.IO.File.OpenWrite(Path.Combine(tempDirectoryPath, guid + ".temp"));
                        //try
                        //{
                        //    fileStream.Position = fileStream.Length;
                        //    await stream.CopyToAsync(fileStream);
                        //    fileStream.Close();
                        //    System.IO.File.Copy(@"E:\Neeo Storage\" + guid + ".temp", @"E:\Neeo Storage\" + guid + "." + extension);
                        //    System.IO.File.Delete(@"E:\Neeo Storage\" + guid + ".temp");
                        //}
                        //catch (Exception)
                        //{
                        //}
                        //finally
                        //{
                        //    fileStream.Close();
                        //}
 * 
 * 
 *  //var fileStream = System.IO.File.OpenWrite(Path.Combine(tempDirectoryPath, guid + ".temp"));

                        //await stream.CopyToAsync(fileStream);
                        //fileStream.Close();
                        //System.IO.File.Copy(@"E:\Neeo Storage\" + guid + ".temp", @"E:\Neeo Storage\" + guid + "." + extension);
                        //System.IO.File.Delete(@"E:\Neeo Storage\" + guid + ".temp");
 */