using System;
using Microsoft.AspNetCore.Identity;
using MTCG.Models.Users;
using MTCG.Repositories;

/*
Singleton Service for User-related authentication logic 
*/
namespace MTCG.Services
{
    public class AuthService
    {
        private static AuthService _instance;

        private readonly UserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        // Dependency Injection über Konstruktor
        private AuthService(UserRepository userRepository)
        {
            // may not be null :
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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

        #region Register

        // Register Http "POST /users"
        public string Register(string inputUsername, string inputPassword)
        {
            // 409: check if user already exists
            if (_userRepository.UserExists(inputUsername))
            {
                //throw new Exception("Username already exists.");
                Console.WriteLine("Error: Username already exists");
                return ""; 
            }

            // Object initializer constructor => calls base constructor
            var user = new User
            {
                Username = inputUsername,
                HashedPassword = _passwordHasher.HashPassword(null, inputPassword),  
                AuthToken = Guid.NewGuid().ToString() 
            };

            // 201: succesfully created -> save user in database
            _userRepository.AddUser(user);
            Console.WriteLine($"Registration successful");

            return user.AuthToken;
        }
        #endregion

        #region Login

        // Login Http "POST /sessions"
        // TODO change exceptions to errors?
        public string Login(string inputUsername, string inputPassword)
        {
            var user = _userRepository.GetUserByUsername(inputUsername);

            // user not found:
            if (user == null)
            {
                //throw new UnauthorizedAccessException("Invalid username.");
                Console.WriteLine("Invalid username");
                return "";
            }

            // verify password
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, inputPassword);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                //throw new UnauthorizedAccessException("Invalid password.");
                Console.WriteLine("Invalid password");
                return "";
            }
        
            user.AuthToken = Guid.NewGuid().ToString(); // update token
            _userRepository.UpdateUser(user); // update user in DB
            user.IsLoggedIn = true;
            Console.WriteLine($"Logging in... Welcome {user.Username}!");

            return user.AuthToken; 
        }
        #endregion

        #region Logout

        // Benutzer abmelden
        public void Logout(string inputUsername)
        {
            var user = _userRepository.GetUserByUsername(inputUsername);

            if (user == null || !user.IsLoggedIn)
            {
                //throw new InvalidOperationException("User not logged in.");
                Console.WriteLine("Error: User not logged in");
            }

            user.IsLoggedIn = false;
            _userRepository.UpdateUser(user);
            Console.WriteLine($"\nLogging out {user.Username}...");
        }
        #endregion
    }
}
