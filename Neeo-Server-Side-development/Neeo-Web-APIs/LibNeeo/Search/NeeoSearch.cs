using Common;
using Common.Models;
using DAL;
using LibNeeo.NearByMe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LibNeeo.Search
{
    public class NeeoSearch
    {
        public List<SearchedUser> FindUserByName(string userId, string searchText, double latitude, double longitude, bool isCurrentLocation)
        {
            DbManager dbManager = new DbManager();
            var searchResult = new List<SearchedUser>();
            DataTable dtSearchedUser = dbManager.FindUserByName(userId, searchText, latitude, longitude, isCurrentLocation);

            searchResult = dtSearchedUser.AsEnumerable().Select(row =>
                                            new SearchedUser
                                            {
                                                Name = row.Field<string>("name"),
                                                UId = row.Field<string>("uid"),
                                                Latitude = row.Field<double?>("latitude"),
                                                Longitude = row.Field<double?>("longitude"),
                                                IsPrivateAccount = row.Field<bool>("isPrivateAccount"),
                                                Status = (FriendRequestStatus)row.Field<int>("status")
                                            }).ToList();
            return searchResult;
        }
    }
}
