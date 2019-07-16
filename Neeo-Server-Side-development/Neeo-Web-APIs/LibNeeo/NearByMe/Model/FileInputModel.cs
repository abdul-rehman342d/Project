using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.NearByMe.Model
{
    public class FileInputModel
    {
        public List<IFormFile> FileToUpload { get; set; }
    }
}
