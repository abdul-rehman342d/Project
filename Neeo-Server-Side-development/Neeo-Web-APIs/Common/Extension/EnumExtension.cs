using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extension
{
    /// <summary>
    /// Defines the extension methods for the enum.
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Gets the description defined on the enum values.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>returns the description specified for the enum value.</returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            if (fieldInfo != null)
            {
                var attributes = fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), true);
                if (attributes.Length > 0)
                {
                    return ((DescriptionAttribute) attributes[0]).Description;
                }
            }
            return null;
        }
    }
}
