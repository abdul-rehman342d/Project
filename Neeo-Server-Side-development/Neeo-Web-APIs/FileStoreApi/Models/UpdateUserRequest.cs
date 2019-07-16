using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FileStoreApi.Models
{
    public class UpdateUserRequest :Common.Entities.BaseRequest
    {
        [Required]
        public string name{get;set;}
        
        [Required]
        public string fileData { get; set; }
    }
}