using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common;

namespace FileStoreApp.Models
{
    public class FileRequest
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Signature { get; set; }
        [Required]
        public FileCategory FileCategory { get; set; }
        [Required]
        public MediaType MediaType { get; set; }

        public string Name
        {
            get
            {
                if (FullName != null)
                {
                    return FullName.Split(new char[] { '.' }).First();
                }
                return null;
            }
        }

        public string Extension
        {
            get
            {
                if (FullName != null)
                {
                    return FullName.Split(new char[] { '.' }, 2).Last();
                }
                return null;
            }
        }
    }
}