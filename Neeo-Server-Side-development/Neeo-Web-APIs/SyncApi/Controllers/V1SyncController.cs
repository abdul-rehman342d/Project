using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common;
using Common.Controllers;
using Common.Models;
using LibNeeo;
using Logger;
using Newtonsoft.Json;
using SyncApi.DTO;

namespace SyncApi.Controllers
{
    [RoutePrefix("api/sync/v1")]
    public class V1SyncController : NeeoApiController
    {
        [HttpPost]
        [Route("contacts")]
        public HttpResponseMessage SyncContacts([FromBody]SyncDataDTO syncData)
        {
            LogRequest(syncData);
            if (!ModelState.IsValid || syncData == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (syncData.Contacts.Count > 0)
            {
                var user = new NeeoUser(syncData.Uid);
                try
                {
                    var result = user.GetContactsState(syncData.MapModel());
                    if (result.Count == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.NoContent);
                    }
                    if (!syncData.Filtered)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, result.ConvertAll((MapContactStatusToContactStatusDTO)));
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, result.ConvertAll((MapcontactStatusToContactSubscriptionDTO)));
                    }
                }
                catch (ApplicationException appExp)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [Route("avatar/timestamp")]
        public HttpResponseMessage SyncAvatarTimestamp([FromBody]SyncDataDTO syncData)
        {
            LogRequest(syncData);
            if (!ModelState.IsValid || syncData == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (syncData.Contacts.Count > 0)
            {
                var user = new NeeoUser(syncData.Uid);
                try
                {
                    var result = user.GetContactsAvatarTimestamp(syncData.MapModel());
                    return Request.CreateResponse(HttpStatusCode.OK, result.ConvertAll((MapcontactStatusToContactAvatarTimestampDTO)));
                   
                }
                catch (ApplicationException appExp)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        private static ContactStatusDTO MapContactStatusToContactStatusDTO(ContactStatus contactStatus)
        {
            return new ContactStatusDTO()
            {
                PhoneNumber = contactStatus.Contact.PhoneNumber,
                IsUser = contactStatus.IsNeeoUser,
                IsSubscribed = contactStatus.IsAlreadySubscribed
            };
        }

        private static ContactSubscriptionDTO MapcontactStatusToContactSubscriptionDTO(ContactStatus contactStatus)
        {
            return new ContactSubscriptionDTO()
            {
                PhoneNumber = contactStatus.Contact.PhoneNumber,
                AvatarTimestamp = contactStatus.AvatarTimestamp,
                IsSubscribed = contactStatus.IsAlreadySubscribed
            };
        }

        private static ContactAvatarTimestampDTO MapcontactStatusToContactAvatarTimestampDTO(ContactStatus contactStatus)
        {
            return new ContactAvatarTimestampDTO()
            {
                PhoneNumber = contactStatus.Contact.PhoneNumber,
                AvatarTimestamp = contactStatus.AvatarTimestamp,
            };
        }

    }
}
