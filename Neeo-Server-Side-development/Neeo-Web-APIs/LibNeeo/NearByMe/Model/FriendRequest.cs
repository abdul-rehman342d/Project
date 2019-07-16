using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.NearByMe
{
    public class FriendRequest
    {
        public long Id { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public FriendRequestStatus Status { get; set; }
    }
}
