using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ExpertTexting.Api.com.experttexting.www;

namespace ExpertTexting.Api
{
    public class ExpertTextingClient
    {
        private readonly string _userId;
        private readonly string _password;
        private readonly string _apiKey;

        public ExpertTextingClient(string userId, string password, string apiKey)
        {
            _userId = userId;
            _password = password;
            _apiKey = apiKey;
        }

        public void SendMessage(string recepient, string sender, string message, bool isUnicode = false)
        {
            var exptTextingApi = new ExptTextingAPI();
            if (isUnicode)
            {
                XmlNode response = exptTextingApi.SendMultilingualSMS(_userId, _password, _apiKey, sender, recepient, message);
            }
            else
            {
                XmlNode response = exptTextingApi.SendSMS(_userId, _password, _apiKey, sender, recepient, message);
            }
        }
        
    }
}
