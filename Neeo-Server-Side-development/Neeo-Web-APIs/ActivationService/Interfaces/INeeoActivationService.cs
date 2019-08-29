using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ActivationService
{
    /// <summary>
    /// Contains the signatures of  method/resources exposed by the Activation service.
    /// </summary>
    [ServiceContract]
    public interface INeeoActivationService
    {
        #region Activation

        #region Iphone 1.x.x, Android 1.x.x
        /*They will be deprecated in future.*/

        /// <summary>
        /// Sends activation code to the phone number provided in <paramref name="phoneNumber"/>.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="phoneNumber">A phone number on which activation code has to be sent.</param>
        /// <param name="devicePlatform">Platform of the user's device.</param>
        /// <param name="activationCode">A code that has to be sent on the give phone number.</param>
        /// <param name="isResend">A bool value indicating this is a resending code request if true,otherwise false (default value). </param>
        /// <param name="isRegenerated">A bool value indicating this is a regenerated code sending request if true,otherwise false (default value). </param>
        /// <returns>An integer indicating whether the SMS has been sent successfully or not, or user is in blocked state.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/SendActivationCode", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        // [TransactionFlow(TransactionFlowOption.Allowed)]
        int SendActivationCode(string phoneNumber, short devicePlatform, string activationCode, bool isResend, bool isRegenerated);

        /// <summary>
        /// Registers and configures user account.
        /// </summary>
        /// <param name="phoneNumber">A string containing phone number for registering account.</param>
        /// <param name="client">An object containing the client information.</param>
        /// <returns>true if the account is successfully registered on the server; otherwise, false.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/RegisterUser", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        //[TransactionFlow(TransactionFlowOption.Allowed)]
        bool RegisterUser(string phoneNumber, UserAgent client);

        #endregion

        #region Iphone 2.0.0, Android x.x.x

        /// <summary>
        /// Sends activation code to the phone number provided in <paramref name="phoneNumber"/>.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="ph">A phone number on which activation code has to be sent.</param>
        /// <param name="dP">Platform of the user's device.</param>
        /// <param name="actCode">A code that has to be sent on the give phone number.</param>
        /// <param name="isRes">A bool value indicating this is a resending code request if true,otherwise false (default value). </param>
        /// <param name="isReg">A bool value indicating this is a regenerated code sending request if true,otherwise false (default value). </param>
        /// <param name="appKey">A string containing the appKey(For Android).</param>
        /// <returns>An integer indicating whether the SMS has been sent successfully or not, or user is in blocked state.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/SendActCode", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        // [TransactionFlow(TransactionFlowOption.Allowed)]
        int SendActCode(string ph, short dP, string actCode, bool isRes, bool isReg, string appKey, short sType);

        /// <summary>
        /// Sends activation code to the phone number provided in <paramref name="phoneNumber"/>.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="ph">A phone number on which activation code has to be sent.</param>
        /// <param name="dP">Platform of the user's device.</param>
        /// <param name="actCode">A code that has to be sent on the give phone number.</param>
        /// <param name="isRes">A bool value indicating this is a resending code request if true,otherwise false (default value). </param>
        /// <param name="isReg">A bool value indicating this is a regenerated code sending request if true,otherwise false (default value). </param>
        /// <param name="appKey">A string containing the appKey(For Android).</param>
        /// <param name="deviceInfo">A string which contain the calling device info. </param>
        /// <param name="isDebugged">A bool to determine if used for debuging and no need to send sms   .</param>
        /// <returns>An integer indicating whether the SMS has been sent successfully or not, or user is in blocked state.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/SendAppActivationCode", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        // [TransactionFlow(TransactionFlowOption.Allowed)]
        int SendAppActivationCode(string ph, short dP, string actCode, bool isRes, bool isReg, string appKey, short sType, string deviceInfo, bool isDebugged );

        ///// <summary>
        ///// Sends activation code to the phone number provided in <paramref name="phoneNumber"/>.
        ///// </summary>
        ///// <remarks>
        ///// </remarks>
        ///// <param name="ph">A phone number on which activation code has to be sent.</param>
        ///// <param name="dP">Platform of the user's device.</param>
        ///// <param name="actCode">A code that has to be sent on the give phone number.</param>
        ///// <returns>An integer indicating whether the SMS has been sent successfully or not, or user is in blocked state.</returns>
        //[OperationContract]
        //[WebInvoke(UriTemplate = "/SendCodeTest", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        //int SendCodeTest(string ph, short dP, string actCode);

        /// <summary>
        /// Registers and configures user account.
        /// </summary>
        /// <param name="ph">A string containing phone number for registering account.</param>
        /// <param name="client">An object containing the client information.</param>
        /// <returns>true if the account is successfully registered on the server; otherwise, false.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/RegisterAppUser", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        //[TransactionFlow(TransactionFlowOption.Allowed)]
        bool RegisterAppUser(string ph, UserAgent client);


        #endregion

        #endregion

        /// <summary>
        /// Updates user's device token into the database for push notifications.
        /// </summary>
        /// <param name="opID">an short integer specifying the type of the operation to be performed.</param>
        /// <param name="uID">A string containing phone number as user id.</param>
        /// <param name="client">An object containing the client information.</param>
        /// <returns>true if the device token is successfully updated in database; otherwise, false.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/DeviceToken", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool DeviceToken(short opID, string uID, UserAgent client);

        /// <summary>
        /// Checks application operatibility and update the user device information in the database for a register user. 
        /// </summary>
        /// <param name="uID">A string containing phone number as user id.</param>
        /// <param name="client">An object containing the client information.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/CheckAppCompatibility", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string CheckAppCompatibility(string uID, UserAgent client);


        /// <summary>
        /// Gets the country of the requesting user 
        /// </summary>
        /// <returns>A string containing the country code (e.g. PK)</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetCountry", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetCountry();

        /// <summary>
        /// Gets the country of the requesting user 
        /// </summary>
        /// <returns>A string containing the country code (e.g. PK)</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/VerifyUser", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool VerifyUser(string uID, string hash);

        /// <summary>
        /// Insert a log record for user registeration request
        /// </summary>
        /// <param name="username">A string containing the user id.</param>
        /// <param name="latitude">A float contain latitude value.</param>
        /// <param name="longitude">A float contain longitude value.</param>
        /// <returns>Returns true on successful insertion.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/InsertUserRegisterationRequestsLog", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        //[TransactionFlow(TransactionFlowOption.Allowed)]
        bool InsertUserRegisterationRequestsLog(string username, float latitude, float longitude);
    }

}
