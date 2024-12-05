﻿using MTCG.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories
{
    // TODO : replace with DB later
    public class UserRepository
    {
        private readonly List<User> _users = new List<User>();

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public User GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        // TODO: update user info (Username, description?)
        // Http : PUT /users/{username}
        // 200, 401 (unauthorized), 404 (not found)
        public void UpdateUser(User user)
        {
            var existingUser = GetUserByUsername(user.Username);
            if (existingUser != null)
            {
                //existingUser.HashedPassword = user.HashedPassword; // BAD!!
                //Console.WriteLine("Info: Password can't be updated yet due to missing database and otherwise potential securityleak");
                //TODO: update Userbio etc.
            }
        }
        // checks if user exists in DB, if so return true
        public bool UserExists(string username)
        {
            return GetUserByUsername(username) != null;
        }

        public void DeleteUser(string username)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                _users.Remove(user);
            }
        }
        
        public bool IsCardInUserStack(User user, Guid cardId)
        {
            return user.Stack!.Any(c => c.Id == cardId);
        }
    }
}
