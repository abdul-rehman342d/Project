using Common;
using LibNeeo.IO;
using LibNeeo.Url;
using Newtonsoft.Json;

namespace LibNeeo.NearByMe
{
    public class NearByUser
    {
        public string UId { get; set; }
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool ShowInfo { get; set; }
        public bool ShowProfileImage { get; set; }
        public bool IsPrivateAccount { get; set; }
        public FriendRequestStatus Status { get; set; }
        public string AvatarUrl {
            get
            {
                NeeoUser user = new NeeoUser(UId);
                NeeoFileInfo filePath = null;
                ulong avatarTimeStamp;
                AvatarState avatarState = user.GetAvatarState(0, out avatarTimeStamp, out filePath);

                if (avatarState == AvatarState.NotExist)
                {
                    return NeeoUrlBuilder.BuildAvatarUrl(UId, 0, 0);
                }

                return NeeoUrlBuilder.BuildAvatarUrl(UId,0, 0);
            }
        }
        [JsonIgnore]
        public bool IsCurrentLocation { get; set; }
    }
}
