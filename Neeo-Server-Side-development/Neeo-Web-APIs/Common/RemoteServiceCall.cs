using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public class RemoteServiceCall
    {
        public Object restApiCall(string baseUrl,string postUrl, RType requestType,object values)
        {
              if (requestType.Equals(RType.POST))
            {
                var client = new RestClient(baseUrl);

                var request = new RestRequest(postUrl, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Content-Type", "application/json; charset=utf-8");
                request.AddJsonBody(values);
                IRestResponse response = client.Execute(request);
                var content = JsonConvert.DeserializeObject<object>(response.Content);
                return content;
            }

            else
            {
                return null;
            }

            
        }
    }
}
