using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common;
using DAL;
using LibNeeo;
using Logger;
using Newtonsoft.Json;
using UtilityService.Models;

namespace UtilityService.Controllers
{
    public class CapabilityController : ApiController
    {
        private bool _logRequestResponse =
        Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);

        [HttpPost]
        public HttpResponseMessage GetCapability([FromBody] CapabilityRequest capabilityRequest)
        {
            //capabilityRequest.Uid = (capabilityRequest.Uid != null) ? capabilityRequest.Uid.Trim() : capabilityRequest.Uid;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> " + JsonConvert.SerializeObject(capabilityRequest));
            }

            #endregion

            //#region user authentication 

            //IEnumerable<string> headerValues;
            //string keyFromClient = "";
            //if (Request.Headers.TryGetValues("key", out headerValues))
            //{
            //    keyFromClient = headerValues.First();
            //}

            //if (!NeeoUtility.AuthenticateUserRequest(capabilityRequest.UID, keyFromClient))
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            //}
            if (ModelState.IsValid)
            {
                ulong temp = 0;
                if (ulong.TryParse(capabilityRequest.Uid, out temp) && capabilityRequest.Cap > 0 && capabilityRequest.Cap < 128)
                {
                    NeeoUser neeoUser = new NeeoUser(capabilityRequest.Uid.Trim());
                    try
                    {
                        Dictionary<string, string> contactsAppVersionDictionary = neeoUser.GetContactsAppVersion(capabilityRequest.Contacts);
                        Capability capability = null;
                        Dictionary<string, Capability> contactsAppCapabilities = new Dictionary<string, Capability>();
                        foreach (var item in contactsAppVersionDictionary)
                        {
                            capability = new Capability(VersionCapability.GetVersionCapability(item.Value.ToUpper()));
                            capability.SetRequestedCapabilities(capabilityRequest.Cap);
                            contactsAppCapabilities.Add(item.Key, capability);
                            capability = null;
                        }

                        CapabilityResponse capabilityResponse = new CapabilityResponse() { UID = capabilityRequest.Uid, ContactsAppCapabilities = contactsAppCapabilities };
                        return Request.CreateResponse(HttpStatusCode.OK, capabilityResponse);
                    }
                    catch (ApplicationException appEx)
                    {
                        return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt16(appEx.Message), NeeoDictionaries.HttpStatusCodeDescriptionMapper[Convert.ToInt16(appEx.Message)]);
                    }
                    catch (Exception appEx)
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
