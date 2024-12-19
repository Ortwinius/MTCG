using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Utilities.CustomExceptions
{
    public class PackageConflictException : Exception
    {
        public PackageConflictException()
            : base("One or multiple cards already exist.") { }
    }
}
