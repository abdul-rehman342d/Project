using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.Plugin
{
    internal class UserServiceStatus
    {
        public const string Ok = "ok";
        public const string IllegalArgumentException = "IllegalArgumentException";
        public const string UserNotFoundException = "UserNotFoundException";
        public const string UserAlreadyExistsException = "UserAlreadyExistsException";
        public const string RequestNotAuthorised = "RequestNotAuthorised";
        public const string UserServiceDisabled = "UserServiceDisabled";
        public const string SharedGroupException = "SharedGroupException";
    }
}
