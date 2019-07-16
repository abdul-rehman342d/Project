using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Common;
using LibNeeo.Voip;

namespace MCRService
{
   
    [ServiceContract]
    public interface IMcrService
    {

        [OperationContract]
        [WebInvoke(UriTemplate = "/mcrCount", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetMcrCount(string userID);

        [OperationContract]
        [WebInvoke(UriTemplate = "/mcrDetails", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        McrData GetMcrDetails(string userID,bool flush);
    }


   
}
