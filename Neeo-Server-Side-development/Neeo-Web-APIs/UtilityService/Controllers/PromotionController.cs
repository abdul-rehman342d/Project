using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using LibNeeo;
using Logger;
using Newtonsoft.Json;
using UtilityService.Models;

namespace UtilityService.Controllers
{
    public class PromotionController : ApiController
    {
        private bool _logRequestResponse =
        Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);

        [HttpPost]
        public HttpResponseMessage Invitation([FromBody] NeeoInvitation invitation)
        {
            // invitation.UID = (invitation.UID != null) ? invitation.UID.Trim() : invitation.UID;
            // invitation.UName = (invitation.UName != null) ? invitation.UName.Trim() : invitation.UName;
            //  invitation.Contacts = (invitation.Contacts != null) ? invitation.Contacts.Trim() : invitation.Contacts;
            //  invitation.Lang = (invitation.Lang != null) ? invitation.Lang.Trim() : invitation.Lang;

            string[] repsonse = { "OK" };

            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> " + JsonConvert.SerializeObject(invitation));
            }

            #endregion

            #region user authentication

            IEnumerable<string> headerValues;
            //string keyFromClient = "";
            //if (Request.Headers.TryGetValues("key", out headerValues))
            //{
            //    keyFromClient = headerValues.First();
            //}

            //if (!NeeoUtility.AuthenticateUserRequest(invitation.Uid, keyFromClient))
            //{
            //    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            //}

            #endregion

                if (ModelState.IsValid)
                {
                    ulong temp = 0;
                    if (ulong.TryParse(invitation.Uid, out temp))
                    {
                        if (NeeoUtility.IsNullOrEmpty(invitation.Lang))
                        {
                            invitation.Lang = "en";
                        }
                        NeeoUser neeoUser = new NeeoUser(invitation.Uid.Trim());
                        try
                        {
                            neeoUser.SendInvitation(invitation.UName, invitation.Contacts, invitation.Lang);
                            return Request.CreateResponse(HttpStatusCode.OK, repsonse);
                        }
                        catch (ApplicationException appEx)
                        {
                            return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt16(appEx.Message), NeeoDictionaries.HttpStatusCodeDescriptionMapper[Convert.ToInt16(appEx.Message)]);
                        }
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, NeeoDictionaries.HttpStatusCodeDescriptionMapper[(int)HttpStatusCode.BadRequest]);
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, NeeoDictionaries.HttpStatusCodeDescriptionMapper[(int)HttpStatusCode.BadRequest]);
                }
           
        }
    }
}
