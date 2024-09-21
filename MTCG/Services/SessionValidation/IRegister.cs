using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services.SessionValidation
{
    public interface IRegister
    {
        void Register();
        public string HashPassword(string password);
    }
}
