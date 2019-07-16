using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common;
using Common.Entities;

namespace FileStoreApi.Models
{
    public class UploadFileRequest : BaseRequest
    {
        public string FId { get; set; }
        [Required]
        public string Data { get; set; } 
        [Required]
        public MimeType MimeType { get; set; }
        public ushort RecipientCount { get; set; }
    }
}