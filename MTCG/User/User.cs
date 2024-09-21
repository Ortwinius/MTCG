using System;

namespace MTCG.User
{
    public class User
    {
        private string _username;
        private string _password;
        private int _coins = 20; // default start value
        private int _elo = 100; // default start value

        public string Username
        {
            get => _username; set => _username = value;
        }
        // to be modified later
        public string Password
        {
            get => _password; set => _password = value;
        }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
