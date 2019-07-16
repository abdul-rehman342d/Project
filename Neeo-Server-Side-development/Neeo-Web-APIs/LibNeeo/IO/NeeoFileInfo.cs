using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Extension;

namespace LibNeeo.IO
{
    /// <summary>
    /// Contains file information.
    /// </summary>
    public class NeeoFileInfo 
    {
        private string _hash;
        private string _fullPath;
        private string _extension;
        /// <summary>
        /// Name of the file without extension.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Name of the file with extension.
        /// </summary>
        public string FullName
        {
            get
            {
                if (Name != null && Extension != null)
                {
                    return Name + Extension;
                }
                return null;
            }
        }
        /// <summary>
        /// Media type of the file
        /// </summary>
        public MediaType MediaType { get; set; }
        /// <summary>
        /// Mime type of the file
        /// </summary>
        public MimeType MimeType { get; set; }

        /// <summary>
        /// Extension of the file
        /// </summary>
        public string Extension
        {
            get
            {
                if (_extension == null)
                {
                    _extension = MimeTypeMapping.GetMimeTypeDetail(MimeType.GetDescription()).Extension;
                }
                return _extension;
            }
            set
            {
                _extension = value;
            }
        }
        /// <summary>
        /// MD5 hash of the file
        /// </summary>
        public string Hash
        {
            get
            {
                return _hash ?? "";
            }
            set
            {
                _hash = value;
            } 
        }
        /// <summary>
        /// Full path of the file containing file name with extension.
        /// </summary>
        public string FullPath
        {
            get
            {
                return _fullPath ?? "";
            }
            set
            {
                _fullPath = value;
            } 
        }
        /// <summary>
        /// Downloading url of the file
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// File creator information of the file.
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// Creation datetime of the file in UTC
        /// </summary>
        public DateTime CreationTimeUtc { get; set; }
        /// <summary>
        /// Length of the file
        /// </summary>
        public long Length { get; set; }
    }
}
