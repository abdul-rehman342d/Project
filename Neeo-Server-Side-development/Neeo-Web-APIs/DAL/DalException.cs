using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DalException : Exception
    {
        public DalException()
            : base()
        { }

        public DalException(string message)
            : base(message)
        { }

        public DalException(string message, Exception innerException)
            : base(message, innerException)
        { }

    }
}
