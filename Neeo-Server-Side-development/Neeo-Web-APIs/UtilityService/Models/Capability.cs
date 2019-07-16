using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using Newtonsoft.Json;

namespace UtilityService.Models
{
    public class Capability
    {
        [JsonPropertyAttribute("ash")]
        public ushort AudioShare { get; set; }

        [JsonPropertyAttribute("vsh")]
        public ushort VideoShare { get; set; }

        [JsonPropertyAttribute("psh")]
        public ushort PhotoShare { get; set; }

        [JsonPropertyAttribute("vac")]
        public ushort AudioCall { get; set; }

        [JsonPropertyAttribute("vvc")]
        public ushort VideoCall { get; set; }

        [JsonPropertyAttribute("gch")]
        public ushort GroupChat { get; set; }

        [JsonPropertyAttribute("tp")]
        public ushort TypingPresence { get; set; }

        [JsonIgnore]
        private bool SerializeAudioShare { get; set; }

        [JsonIgnore]
        private bool SerializeVideoShare { get; set; }

        [JsonIgnore]
        private bool SerializePhotoShare { get; set; }

        [JsonIgnore]
        private bool SerializeAudioCall { get; set; }

        [JsonIgnore]
        private bool SerializeVideoCall { get; set; }

        [JsonIgnore]
        private bool SerializeGroupChat { get; set; }

        [JsonIgnore]
        private bool SerializeTypingPresence { get; set; }

        public Capability()
        {
            
        }

        public Capability(Capability capability)
        {
            AudioShare = capability.AudioShare;
            VideoShare = capability.VideoShare;
            PhotoShare = capability.PhotoShare;
            AudioCall = capability.AudioCall;
            VideoCall = capability.VideoCall;
            GroupChat = capability.GroupChat;
            TypingPresence = capability.TypingPresence;
        }

        public bool ShouldSerializeAudioShare()
        {
            return SerializeAudioShare;
        }

        public bool ShouldSerializeVideoShare()
        {
            return SerializeVideoShare;
        }

        public bool ShouldSerializePhotoShare()
        {
            return SerializePhotoShare;
        }

        public bool ShouldSerializeAudioCall()
        {
            return SerializeAudioCall;
        }

        public bool ShouldSerializeVideoCall()
        {
            return SerializeVideoCall;
        }

        public bool ShouldSerializeGroupChat()
        {
            return SerializeGroupChat;
        }

        public bool ShouldSerializeTypingPresence()
        {
            return SerializeTypingPresence;
        }

        public void SetRequestedCapabilities(ushort reqCapabiliities)
        {
            if ((AppCapabilities) reqCapabiliities == AppCapabilities.All)
            {
                this.SerializeAudioShare = true;
                this.SerializeVideoShare = true;
                this.SerializePhotoShare = true;
                this.SerializeAudioCall = true;
                this.SerializeVideoCall = true;
                this.SerializeGroupChat = true;
                this.SerializeTypingPresence = true;
            }
            else
            {
                var enumValues = (AppCapabilities[]) Enum.GetValues(typeof (AppCapabilities));
                for (int index = 0; index < enumValues.Length - 1; index ++)
                {
                    AppCapabilities appCapabilities =
                        (AppCapabilities) (reqCapabiliities & Convert.ToInt16(enumValues[index].ToString("D")));

                    switch (appCapabilities)
                    {
                        case AppCapabilities.Vac:
                            this.SerializeAudioCall = true;
                            break;
                        case AppCapabilities.Vvc:
                            this.SerializeVideoCall = true;
                            break;
                        case AppCapabilities.Ash:
                            this.SerializeAudioShare = true;
                            break;
                        case AppCapabilities.Vsh:
                            this.SerializeVideoShare = true;
                            break;
                        case AppCapabilities.Psh:
                            this.SerializePhotoShare = true;
                            break;
                        case AppCapabilities.Gch:
                            this.SerializeGroupChat = true;
                            break;
                        case AppCapabilities.Tp:
                            this.SerializeTypingPresence = true;
                            break;

                    }
                }
            }
        }
    }
}