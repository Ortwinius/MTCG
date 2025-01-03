using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Utilities.Exceptions.CustomExceptions
{
    public class NotAdminException : Exception
    {
        public NotAdminException() : base("The provided user is not admin.") { }
    }
}
