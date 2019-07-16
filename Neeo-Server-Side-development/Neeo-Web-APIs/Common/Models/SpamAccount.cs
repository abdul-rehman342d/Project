using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Common;


namespace Common.Models
{
    [DataContract]
    public class SpamAccount
    {
        private string _userID;
        private string _appID;
        private string _spamUserID;
        private string _reason;

        /// <summary>
        /// Specifies user id
        /// </summary>
        [DataMember(IsRequired = true, Name = "uid")]
        public string UserID
        {
            get
            {
                if (_userID != null)
                {
                    return _userID.Trim();
                }
                return null;
            }

            set
            {
                _userID = value;
            }
        }

        /// <summary>
        /// Specifies application id
        /// </summary>
        [DataMember(IsRequired = true, Name = "app-id")]
        public string AppID
        {
            get
            {
                if (_appID != null)
                {
                    return _appID.Trim();
                }
                return null;
            }

            set
            {
                _appID = value;
            }
        }

        /// <summary>
        /// Specifies user id which is to be marked as spam
        /// </summary>
        [DataMember(IsRequired = true, Name = "spam-uid")]
        public string SpamUserID
        {
            get
            {
                if (_spamUserID != null)
                {
                    return _spamUserID.Trim();
                }
                return null;
            }

            set
            {
                _spamUserID = value;
            }
        }

        /// <summary>
        /// Specifies spam reason
        /// </summary>
        [DataMember(Name = "reason")]
        public string Reason
        {
            get
            {
                if (_reason != null)
                {
                    return _reason.Trim();
                }
                return "";
            }

            set
            {
                _reason = value;
            }
        }

        public bool Valid()
        {
            if (NeeoUtility.IsNullOrEmpty(UserID) || NeeoUtility.IsNullOrEmpty(AppID) || NeeoUtility.IsNullOrEmpty(SpamUserID) || UserID == SpamUserID)
            {
                return false;
            }

            return true;
        }
    }
}