using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Common.Entities
{
    public class BaseRequest
    {
        private string _uid;

        [Required]
        [RegularExpression("^([0-9]+)(\\s)*$")]
        public string Uid
        {
            get
            {
                if (!string.IsNullOrEmpty(_uid))
                {
                    return _uid.Trim();
                }
                return _uid;
            }
            set
            {
                _uid = value;
            }
        }
    }
}