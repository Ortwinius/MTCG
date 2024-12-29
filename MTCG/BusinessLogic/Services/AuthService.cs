using System;
using Microsoft.AspNetCore.Identity;
using MTCG.Models.Users;
using MTCG.Models.Users.DTOs;
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

        // bad - what if admin has other username?
        public bool IsAdmin(string authToken)
        {
            return authToken == "admin-mtcgToken";
        }

        #region Register

        // Register Http "POST /users"
        public void Register(string inputUsername, string inputPassword)
        {
            // 409: check if user already exists
            if (_userRepository.UserExists(inputUsername))
            {
                Console.WriteLine("Error: Username already exists");
                throw new UserAlreadyExistsException();
            }

            string HashedPassword = _passwordHasher.HashPassword(null, inputPassword);
            string authToken = Guid.NewGuid().ToString();
            var user = new User(inputUsername,HashedPassword);
            user.AuthToken = authToken;

            // 201: succesfully created -> save user in database
            _userRepository.AddUser(user);
            Console.WriteLine($"Registration successful");
        }
        #endregion

        #region Login

        // Login Http "POST /sessions"
        public void Login(string inputUsername, string inputPassword, out string? authToken)
        {
            var user = _userRepository.GetUserByUsername(inputUsername);
            authToken = null;

            // user not found:
            if (user == null)
            {
                Console.WriteLine("Invalid username");
                throw new UserNotFoundException();
            }
            // verify password
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, inputPassword);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                Console.WriteLine("Invalid password");
                throw new UnauthorizedException();
            }
        
            user.AuthToken = user.Username + "-mtcgToken";
            authToken = user.AuthToken; // for out parameter

            // update user in db with new authToken
            _userRepository.UpdateUser(user);

            Console.WriteLine($"Logging in... Welcome {user.Username}!");
        }
        #endregion


        // Get user by username
        public User? GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }
        public User? GetUserByToken(string authtoken)
        {
            var user = _userRepository.GetUserByToken(authtoken);
            if(user == null)
            {
                throw new UnauthorizedException();
            }
            return user;
        }
        // Get user data by token
        public UserDataDTO? GetUserDataByToken(string authToken)
        {
            var userData = _userRepository.GetUserDataByToken(authToken);
            if (userData == null)
            {
                throw new UnauthorizedException();
            }
            return userData;
        }
        public void UpdateUserData(string username, UserDataDTO userData)
        {
            _userRepository.UpdateUserData(username, userData);
        }
        // get user stats by token

        public string GetAuthToken(Dictionary<string, string> headers)
        {
            if (!headers.TryGetValue("Authorization", out var authToken) || string.IsNullOrWhiteSpace(authToken))
            {
                throw new UnauthorizedException(); 
            }

            return authToken;
        }
        // validate each action by checking if user is logged in and authToken is valid
        // also checks if path matches authtoken eg users/{username} = {username}-mtcgToken but only if there is a path
        public bool IsAuthenticated(string authToken, string? username = null)
        {
            try
            {
                if (username != null)
                {
                    var user = _userRepository.GetUserByUsername(username);

                    if (user == null)
                        throw new UserNotFoundException(); 

                    return user.AuthToken == authToken;
                }

                if (_userRepository.ValidateToken(authToken))
                    return true; 

                throw new UnauthorizedException(); 
            }
            catch (UserNotFoundException)
            {
                throw; 
            }
            catch (Exception)
            {
                throw new UnauthorizedException("Authentication failed."); 
            }
        }

    }
}
