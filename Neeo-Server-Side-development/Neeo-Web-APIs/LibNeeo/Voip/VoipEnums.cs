using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.Voip
{
    /// <summary>
    /// Specifies whether user push enabled or not.
    /// </summary>
    public enum PushEnabled : short
    {
        NotSpecified = -1,
        False = 0,
        True = 1
    }

    /// <summary>
    /// Specifies whether user is acitve or in-active.
    /// </summary>
    public enum UserStatus : short
    {
        NotSpecified = -1,
        Inactive = 0,
        Active = 1
    }

    /// <summary>
    /// Specifies the request mode for voip registration api.
    /// </summary>
    public enum RequestMode : short
    {
        Add = 1,
        Get = 2,
        Set = 3
    }
}
