using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services.SessionValidation
{
    public interface ILogin
    {
        public string Username { get; }
        public string Password { get; }
        public bool IsLoggedIn { get; }
        public void Login(string username, string password);
        public void Logout();
    }
}
