using System.Collections.Generic;

namespace LibNeeo.NearByMe
{
    public class FriendRequestDetails
    {
        public List<NearByUser> AccpetedRequests { get; set; } = new List<NearByUser>();
        public List<NearByUser> PendingRequests { get; set; } = new List<NearByUser>();
        public List<NearByUser> RejectedRequests { get; set; } = new List<NearByUser>();
    }
}
