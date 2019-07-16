using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Neeo.Nexmo
{
    class NexmoRequest
    {
        [JsonPropertyAttribute("api_key")]
        public string ApiKey { get; set; }

        [JsonPropertyAttribute("api_secret")]
        public string ApiSecret { get; set; }

        [JsonPropertyAttribute("from")]
        public string From { get; set; }

        [JsonPropertyAttribute("to")]
        public string To { get; set; }

        [JsonPropertyAttribute("text")]
        public string Text { get; set; }

        [JsonPropertyAttribute("type")]
        public string Type { get; set; }

        [JsonIgnore]
        private bool SerializeType { get; set; }

        public bool ShouldSerializeType()
        {
            return SerializeType;
        }

        internal void WithUnicode(bool isUnicode)
        {
            SerializeType = isUnicode;
        }
    }
}
