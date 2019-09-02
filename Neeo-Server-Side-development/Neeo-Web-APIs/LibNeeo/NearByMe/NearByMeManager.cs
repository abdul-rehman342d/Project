using Common;
using DAL;
using LibNeeo.NearByMe.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNeeo.IO;

namespace LibNeeo.NearByMe
{
    public class NearByMeManager
    {
        private DbManager _dbManager = new DbManager();
        private static NearByMeManager _instance = new NearByMeManager();

        public static NearByMeManager GetInstance()
        {
            return new NearByMeManager();
        }

        public NearByMeSetting GetNearByMeSetting(string username)
        {
            NearByMeSetting setting = null;
            DataTable dtNearbyMeSetting = _dbManager.GetNearByMeSetting(username);

            setting = (from row in dtNearbyMeSetting.AsEnumerable()
                       select new NearByMeSetting()
                       {
                           Enabled = Convert.ToBoolean(row["enabled"]),
                           UserName = Convert.ToString(row["username"]),
                           NotificationOn = Convert.ToBoolean(row["notificationOn"]),
                           NotificationTone = Convert.ToUInt16(row["notificationTone"]),
                           ShowInfo = Convert.ToBoolean(row["showInfo"]),
                           ShowProfileImage = Convert.ToBoolean(row["showProfileImage"]),
                           IsPrivateAccount = Convert.ToBoolean(row["isPrivateAccount"]),
                       }).SingleOrDefault();

            return setting;
        }

        public List<NearByUser> GetNearByMeUserByLocation(NearByUser user)
        {
            List<NearByUser> nearByUsers = new List<NearByUser>();
            DataTable dtNearbyUser = _dbManager.GetNearByMeUserByLocation(user.UId, user.Latitude.Value, user.Longitude.Value, user.IsCurrentLocation);

            nearByUsers = (from row in dtNearbyUser.AsEnumerable()
                           select new NearByUser()
                           {
                               UId = Convert.ToString(row["username"]),
                               Name = Convert.ToString(row["name"]),
                               Latitude = row.Field<double?>("latitude"),
                               Longitude = row.Field<double?>("longitude"),
                               ShowInfo = Convert.ToBoolean(row["showInfo"]),
                               ShowProfileImage = Convert.ToBoolean(row["showProfileImage"]),
                               IsPrivateAccount = Convert.ToBoolean(row["isPrivateAccount"]),
                               Status = (FriendRequestStatus)Convert.ToInt32(row["status"])
                           }).ToList();

            return nearByUsers;
        }

        public bool UpsertNearByMeSetting(NearByMeSetting setting)
        {
            return _dbManager.UpsertNearByMeSetting(setting.UserName, setting.Enabled, setting.NotificationTone, setting.NotificationOn, setting.ShowInfo, setting.ShowProfileImage, setting.IsPrivateAccount);
        }

        public bool UpsertUserGpsLocation(NearByUser user)
        {
            return _dbManager.UpsertUserGpsLocation(user.UId, user.Latitude.Value, user.Longitude.Value);
        }

        public bool UpsertFriendRequest(FriendRequest friendRequest)
        {
            return _dbManager.UpsertFriendRequest(friendRequest.SenderId, friendRequest.RecipientId, friendRequest.Status);
        }

        public FriendRequestDetails GetFriendRequestDetails(string uId)
        {
            FriendRequestDetails friendRequestDetails = new FriendRequestDetails();
            DataTable dtFriendRequest = _dbManager.GetFriendRequestDetails(uId);

            if (dtFriendRequest.Rows.Count > 0)
            {
                friendRequestDetails.PendingRequests = (from row in dtFriendRequest.AsEnumerable()
                                                        where (FriendRequestStatus)int.Parse(row["status"].ToString()) == FriendRequestStatus.Pending
                                                        select new NearByUser()
                                                        {
                                                            UId = Convert.ToString(row["uId"]),
                                                            Name = Convert.ToString(row["name"]),
                                                            Latitude = row.Field<double?>("latitude"),
                                                            Longitude = row.Field<double?>("latitude"),
                                                            ShowInfo = Convert.ToBoolean(row["showInfo"]),
                                                            ShowProfileImage = Convert.ToBoolean(row["showProfileImage"]),
                                                            IsPrivateAccount = Convert.ToBoolean(row["isPrivateAccount"])
                                                        }).ToList();

                friendRequestDetails.AccpetedRequests = (from row in dtFriendRequest.AsEnumerable()
                                                         where (FriendRequestStatus)int.Parse(row["status"].ToString()) == FriendRequestStatus.Accepted
                                                         select new NearByUser()
                                                         {
                                                             UId = Convert.ToString(row["uId"]),
                                                             Name = Convert.ToString(row["name"]),
                                                             Latitude = row.Field<double?>("latitude"),
                                                             Longitude = row.Field<double?>("latitude"),
                                                             ShowInfo = Convert.ToBoolean(row["showInfo"]),
                                                             ShowProfileImage = Convert.ToBoolean(row["showProfileImage"]),
                                                             IsPrivateAccount = Convert.ToBoolean(row["isPrivateAccount"])
                                                         }).ToList();

                friendRequestDetails.RejectedRequests = (from row in dtFriendRequest.AsEnumerable()
                                                         where (FriendRequestStatus)int.Parse(row["status"].ToString()) == FriendRequestStatus.Rejected
                                                         select new NearByUser()
                                                         {
                                                             UId = Convert.ToString(row["uId"]),
                                                             Name = Convert.ToString(row["name"]),
                                                             Latitude = row.Field<double?>("latitude"),
                                                             Longitude = row.Field<double?>("latitude"),
                                                             ShowInfo = Convert.ToBoolean(row["showInfo"]),
                                                             ShowProfileImage = Convert.ToBoolean(row["showProfileImage"]),
                                                             IsPrivateAccount = Convert.ToBoolean(row["isPrivateAccount"])
                                                         }).ToList();
            }

            return friendRequestDetails;
        }

        public FriendRequest IsFriendRequestExist(string username, string friendUId)
        {
            FriendRequest friendRequest = null;
            DataTable dtFriendRequest = _dbManager.IsFriendRequestExist(username, friendUId);

            friendRequest = (from row in dtFriendRequest.AsEnumerable()
                             select new FriendRequest()
                             {
                                 Id = Convert.ToInt64(row["Id"]),
                                 SenderId = Convert.ToString(row["SenderId"]),
                                 RecipientId = Convert.ToString(row["RecipientId"]),
                                 Status = (FriendRequestStatus)Convert.ToInt32(row["Status"]),
                             }).SingleOrDefault();

            return friendRequest;
        }

        public bool DeleteFriendRequest(FriendRequest friendRequest)
        {
            return _dbManager.DeleteFriendRequest(friendRequest.SenderId, friendRequest.RecipientId);
        }

        //by uzair
        public List<NearByUser> GetNearByUsersByLocation(UserSearch user)
        {
            List<NearByUser> nearByUsers = new List<NearByUser>();
            DataTable dtNearbyUser = _dbManager.GetNearByUsersByLocation(user.Uid, user.Latitude, user.Longitude);

            nearByUsers = (from row in dtNearbyUser.AsEnumerable()
                           select new NearByUser()
                           {
                               UId = Convert.ToString(row["username"]),
                               Name = Convert.ToString(row["name"]),
                               Latitude = row.Field<double?>("latitude"),
                               Longitude = row.Field<double?>("longitude"),
                               ShowInfo = Convert.ToBoolean(row["showInfo"]),
                               ShowProfileImage = (Convert.ToBoolean(row["showProfileImage"]) == false || FileManager.GetFile(Convert.ToString(row["username"]), FileCategory.Profile, MediaType.Image) == null) ? false : true,
                               IsPrivateAccount = Convert.ToBoolean(row["isPrivateAccount"]),
                               Status = (FriendRequestStatus)Convert.ToInt32(row["status"])
                           }).ToList();

            return nearByUsers;
        }
    }
}
