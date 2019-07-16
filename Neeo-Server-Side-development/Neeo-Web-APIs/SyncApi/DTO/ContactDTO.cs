using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common.Entities;
using Newtonsoft.Json;

namespace SyncApi.DTO
{
    public class ContactDTO
    {
        private string _phoneNumber;
        /// <summary>
        /// Specifies the contact phone number.
        /// </summary>
        [Required]
        [RegularExpression("^([0-9]+)(\\s)*$")]
        [JsonProperty("ph")]
        public string PhoneNumber 
        {
            get
            {
                if (!string.IsNullOrEmpty(_phoneNumber))
                {
                    return _phoneNumber.Trim();
                }
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
            }
        }
    }
}