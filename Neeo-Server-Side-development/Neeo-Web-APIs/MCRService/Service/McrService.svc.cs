using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using Common;
using LibNeeo;
using LibNeeo.Voip;
using Logger;


namespace MCRService
{

    public class McrService : IMcrService
    {
        public string GetMcrCount(string userID)
        {
            userID = (userID != null) ? userID.Trim() : userID;
            //         #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //   if (NeeoUtility.AuthenticateUserRequest( userID, keyFromClient))
            //{
            //#endregion

            ulong temp;
            if (!NeeoUtility.IsNullOrEmpty(userID) && ulong.TryParse(userID, out temp))
            {
                try
                {
                    return NeeoVoipApi.GetMcrCount(userID);
                }
                catch (ApplicationException applicationException)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(applicationException.Message)));
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                }
                return "";
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
                return "";
            }
            //}
            //   else
            //   {
            //       NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);
            //       return "";
            //   }
        }

        public McrData GetMcrDetails(string userID, bool flush)
        {
            userID = (userID != null) ? userID.Trim() : userID;
            // #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //   if (NeeoUtility.AuthenticateUserRequest( userID, keyFromClient))
            //{
            //#endregion
            ulong temp;
            if (!NeeoUtility.IsNullOrEmpty(userID) && ulong.TryParse(userID, out temp))
            {
                try
                {
                    return NeeoUser.GetMcrDetails(userID, flush);
                }
                catch (ApplicationException applicationException)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(applicationException.Message)));
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                }
                return null;
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
                return null;
            }
            //}
            //   else
            //   {
            //       NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);
            //       return null;
            //   }
        }
    }
}
