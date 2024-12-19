using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Utilities.CustomExceptions
{
    public class NotEnoughCoinsException : Exception
    {
        public NotEnoughCoinsException()
            : base("The provided user doesnt have enough coins.") { }
    }
}
