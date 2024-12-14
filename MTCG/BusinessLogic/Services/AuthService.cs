using System;
using Microsoft.AspNetCore.Identity;
using MTCG.Models.Users;
using MTCG.Repositories;

/*
Singleton Service for User-related authentication logic 
*/
namespace MTCG.BusinessLogic.Services
{
    public class AuthService
    {
        private static AuthService? _instance;

        private readonly UserRepository _userRepository; 
        private readonly PasswordHasher<User> _passwordHasher;

        // Dependency Injection über Konstruktor
        private AuthService(UserRepository userRepository)
        {
            // may not be null :
            _userRepository = userRepository 
                ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = new PasswordHasher<User>();
        }
        public static AuthService GetInstance(UserRepository userRepository)
        {
            if(_instance == null)
            {
                _instance = new AuthService(userRepository);
            }
            return _instance;
        }

        // validate each action by checking if user is logged in and authToken is valid
        public bool IsAuthenticated(User user)
        {
            if (!user.IsLoggedIn || string.IsNullOrEmpty(user.AuthToken))
            {
                Console.WriteLine("You cannot perform this action due to missing permission. Are you logged in?");
                return false;
            }
            return true;
        }

        #region Register

        // Register Http "POST /users"
        public bool Register(string inputUsername, string inputPassword)
        {
            // 409: check if user already exists
            if (_userRepository.UserExists(inputUsername))
            {
                Console.WriteLine("Error: Username already exists");
                return false;
            }

            string HashedPassword = _passwordHasher.HashPassword(null, inputPassword);
            string authToken = Guid.NewGuid().ToString();
            var user = new User(inputUsername,HashedPassword);
            user.AuthToken = authToken;

            // 201: succesfully created -> save user in database
            _userRepository.AddUser(user);
            Console.WriteLine($"Registration successful");

            return true;
        }
        #endregion

        #region Login

        // Login Http "POST /sessions"
        public bool Login(string inputUsername, string inputPassword, out string? authToken)
        {
            var user = _userRepository.GetUserByUsername(inputUsername);
            authToken = null;

            // user not found:
            if (user == null)
            {
                Console.WriteLine("Invalid username");
                return false;
            }
            // if already logged in
            if (user.AuthToken != null)
            {
                Console.WriteLine($"User {user.Username} is already logged in.");
                return false;
            }
            // verify password
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, inputPassword);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                Console.WriteLine("Invalid password");
                return false;
            }
        
            user.AuthToken = user.Username + "-mtcgToken";
            authToken = user.AuthToken; // for out parameter

            // update user in db with new authToken
            _userRepository.UpdateUser(user);

            Console.WriteLine($"Logging in... Welcome {user.Username}!");

            return true;
        }
        #endregion

        #region Logout

        // Benutzer abmelden
        public bool Logout(string inputUsername)
        {
            var user = _userRepository.GetUserByUsername(inputUsername);

            if (user == null || !user.IsLoggedIn)
            {
                //throw new InvalidOperationException("User not logged in.");
                Console.WriteLine("Error: User not logged in or user does not exist");
                return false;
            }

            user.IsLoggedIn = false;
            _userRepository.UpdateUser(user);
            Console.WriteLine($"\nLogging out {user.Username}...");
            return true;
        }
        #endregion

        // Get user by username
        public User GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }
    }
}
