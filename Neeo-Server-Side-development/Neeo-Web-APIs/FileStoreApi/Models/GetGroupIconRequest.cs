using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FileStoreApi.Models
{
    public class GetGroupIconRequest : Common.Entities.BaseRequest
    {
        [Required]
        public string gID { get; set; }
    }
}