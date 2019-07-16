using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LibNeeo.Voip
{
    public class BlockUser
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("callee")]
        public string Callee { get; set; }

        [JsonProperty("caller")]
        public List<string> Caller { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }
}
