using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common.Entities;

namespace FileStoreApi.Models
{
    public class FileDescription: BaseRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string MimeType { get; set; }
        [Required]
        public string Extension { get; set; }
        [Required]
        public long Length { get; set; }
    }
}