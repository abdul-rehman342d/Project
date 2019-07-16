using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DAL;
using LibNeeo.IO;
using LibNeeo.Plugin;
using File = LibNeeo.IO.File;

namespace LibNeeo.MUC
{
    /// <summary>
    /// 
    /// </summary>
    public class NeeoGroup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static void DeleteGroupIcon(string userID, string groupID)
        {
            FileManager.Delete(groupID, FileCategory.Group);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static File GetGroupIcon(string groupID)
        {
            return FileManager.GetFile(groupID, FileCategory.Group, MediaType.Image);
            //string filePath = Path.Combine(ConfigurationManager.AppSettings[NeeoConstants.GroupImageRootPath], NeeoUtility.GetHierarchicalPath(groupID, NeeoConstants.HierarchyLevelLimit));
            //var file = File.GetFile(groupID, filePath, MediaType.Image);
            //return file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="senderID"></param>
        public static List<NeeoUser> GetGroupParticipants(int groupID, string senderID, int messageType)
        {
            DbManager dbManager = new DbManager();
            DataTable dtGroupParticipantData = new DataTable();
            dtGroupParticipantData = dbManager.GetGroupParticipantsData(groupID,
                ConfigurationManager.AppSettings[NeeoConstants.Domain], senderID, messageType);

            List<NeeoUser> lstParticipants = new List<NeeoUser>();
            if (dtGroupParticipantData.Rows.Count > 0)
            {
                lstParticipants = dtGroupParticipantData.AsEnumerable()
                    .Select(
                        row =>
                            new NeeoUser(row.Field<string>(0))
                            {
                                DeviceToken = row.Field<string>(1),
                                OfflineMsgCount = row.Field<int>(2),
                                ImTone = (IMTone)row.Field<Byte>(3),
                                DevicePlatform = (DevicePlatform)row.Field<Byte>(4)
                            }).ToList();
                GetParticipantsPresence(lstParticipants);
            }
            return lstParticipants;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="senderID"></param>
        /// <returns></returns>
        public static List<NeeoUser> GetGroupParticipants(string roomName, string senderID)
        {
            DbManager dbManager = new DbManager();
            DataTable dtGroupParticipantData = new DataTable();
            dtGroupParticipantData = dbManager.GetGroupParticipantsData(roomName,
                ConfigurationManager.AppSettings[NeeoConstants.Domain], senderID);

            List<NeeoUser> lstParticipants = new List<NeeoUser>();
            if (dtGroupParticipantData.Rows.Count > 0)
            {
                lstParticipants = dtGroupParticipantData.AsEnumerable()
                    .Select(
                        row =>
                            new NeeoUser(row.Field<string>(0))
                            {
                                DeviceToken = row.Field<string>(1),
                                OfflineMsgCount = row.Field<int>(2),
                                ImTone = (IMTone)row.Field<Byte>(3),
                                DevicePlatform = (DevicePlatform)row.Field<Byte>(4)
                            }).ToList();
                GetParticipantsPresence(lstParticipants);
            }
            return lstParticipants;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstParticipant"></param>
        public static void GetParticipantsPresence(List<NeeoUser> lstParticipant)
        {
            Parallel.ForEach(lstParticipant, item =>
            {
                item.GetPresence();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static List<GroupInfo> GetUserGroupsDetails(NeeoUser user)
        {
            string[] delimeter = { "," };
            DbManager dbManager = new DbManager();
            DataTable dtUserGroupDetails = dbManager.GetUserGroupsDetails(user.UserJid);
            List<GroupInfo> groupList = new List<GroupInfo>();
            if (dtUserGroupDetails.Rows.Count > 0)
            {
                groupList =
                    dtUserGroupDetails.AsEnumerable()
                    .Select(dr => new GroupInfo()
                    {
                        Id = dr.Field<string>("name"),
                        Subject = dr.Field<string>("subject"),
                        Owner = dr.Field<string>("admin"),
                        Creator = dr.Field<string>("creatorId"),
                        Participants = dr.Field<string>("participants").EndsWith(delimeter[0]) ? dr.Field<string>("participants").Substring(0, dr.Field<string>("participants").Length - 1).Split(delimeter, StringSplitOptions.None) : null,
                        CreationDate = dr.Field<string>("creationDate")
                    })
                        .ToList();
            }
            return groupList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static bool GroupIconExists(string groupID)
        {
            return FileManager.IsExist(groupID, FileCategory.Group);
            //string filePath = Path.Combine(ConfigurationManager.AppSettings[NeeoConstants.GroupImageRootPath], NeeoUtility.GetHierarchicalPath(groupID, NeeoConstants.HierarchyLevelLimit));
            //return File.Exists(Path.Combine(filePath, NeeoUtility.AddFileExtension(groupID, MediaType.Image)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool SaveGroupIcon(string userID, string groupID, File file)
        {
            file.Info.Name = groupID;
            return FileManager.Save(file, FileCategory.Group);
        }
    }
}
