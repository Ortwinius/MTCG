using System;
using Microsoft.AspNetCore.Identity;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Utilities.CustomExceptions;

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
        // also checks if path matches authtoken eg users/{username} = {username}-mtcgToken but only if there is a path
        public bool IsAuthenticated(string authToken, string? pathUsername = null)
        {
            if(pathUsername != null)
            {
                return _userRepository.GetUserByUsername(pathUsername)?.AuthToken == authToken;
            }
            return _userRepository.ValidateToken(authToken); 
        }

        // bad - what if admin has other username?
        public bool IsAdmin(string authToken)
        {
            return authToken == "admin-mtcgToken";
        }

        #region Register

        // Register Http "POST /users"
        public bool Register(string inputUsername, string inputPassword)
        {
            Console.WriteLine("[AuthService] Trying to execute register logic");
            Console.WriteLine("[AuthService] InputUsername: " + inputUsername);
            Console.WriteLine("[AuthService] InputPassword: " + inputPassword);
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


        // Get user by username
        public User? GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }
        public User? GetUserByValidToken(string authtoken)
        {
            var user = _userRepository.GetUserByValidToken(authtoken);
            if(user == null)
            {
                throw new UnauthorizedException();
            }
            return user;
        }
        public string GetValidAuthToken(Dictionary<string, string> headers)
        {
            if (!headers.TryGetValue("Authorization", out var authToken) || string.IsNullOrWhiteSpace(authToken))
            {
                throw new UnauthorizedException(); 
            }

            return authToken;
        }
    }
}
