using Common;
using LibNeeo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using UtilityService.Models;

namespace UtilityService.Controllers
{
    [RoutePrefix("api/v1/user-chat")]
    public class ChatController : ApiController
    {

        /// <summery>
        /// This api Create a Chat Backup of user
        /// </summery>
        /// <param name="chatBackupDTO">
        /// Field Name: sender , Data Type: string, Description: mandatory parameter
        /// Field Name: messagesXml , Data Type: xml, Description: Collection of Messages
        /// </param>
        /// <remarks>Response: On succesfully execution this api return 200 status code</remarks>
        [Route("CreateChatBackup")]
        [HttpPost]

        public async Task<HttpResponseMessage> CreateXMLChatBackup([FromBody] ChatBackupDTO chatBackupDTO)
        {

            NeeoUser neeoUser = new NeeoUser("0");
            try
            {

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                bool operationCompleted = await System.Threading.Tasks.Task.Run(() => neeoUser.CreateXMLChatBackup(chatBackupDTO.sender, chatBackupDTO.messagesXml));
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
        ///  This API Gets  Messages  Against Requested of sender"
        /// </summary>
        /// <param name="sender">
        /// Field Name :sender,  DataType: Int , Description:  sender is target by the  details of singal user
        /// <remarks>Response: HTTP Status 200 And HttpStatusCode.Ok indicates that the request succeeded</remarks>
        /// <returns> All Details About User Messages  </returns>

        [HttpGet]
        [Route("GetChatBackup/{sender}")]
        public async Task<HttpResponseMessage> GetChatBackup([FromUri] string sender)
        {
            try
            {
                NeeoUser neeoUser = new NeeoUser(sender);
                var operationCompleted = await System.Threading.Tasks.Task.Run(() => neeoUser.GetXMLChatBackup(sender));
                return Request.CreateResponse(HttpStatusCode.OK , operationCompleted);
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
