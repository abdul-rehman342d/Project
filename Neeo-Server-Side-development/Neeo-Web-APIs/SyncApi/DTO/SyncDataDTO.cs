using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.Expressions;
using Common.Entities;
using Common.Models;
using Newtonsoft.Json;

namespace SyncApi.DTO
{
    public class SyncDataDTO : BaseRequest
    {
        [Required]
        public List<ContactDTO> Contacts { get; set; }
        public bool Filtered { get; set; }

        public SyncData MapModel()
        {
            return new SyncData()
            {
                ContactsList = Contacts.Select(x => new Contact() { PhoneNumber = x.PhoneNumber }).ToList(),
                Filtered = Filtered
            };
        }
    }
}