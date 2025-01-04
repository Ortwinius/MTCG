using System;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MTCG.Models.Users;
using MTCG.Models.Users.DTOs;
using MTCG.Repositories;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.Exceptions.CustomExceptions;

/*
Singleton Service for User-related authentication logic 
*/
namespace MTCG.BusinessLogic.Services
{
    public class AuthService
    {
        private static AuthService? _instance;

        private readonly IUserRepository _userRepository; 
        private readonly PasswordHasher<User> _passwordHasher;

        // Dependency Injection über Konstruktor
        private AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
        }
        public static AuthService GetInstance(IUserRepository userRepository)
        {
            if(_instance == null)
            {
                _instance = new AuthService(userRepository);
            }
            return _instance;
        }
        public static void ResetInstance() => _instance = null;

        #region Register

        // Register Http "POST /users"
        public void Register(string inputUsername, string inputPassword)
        {
            // 409: check if user already exists
            if (_userRepository.UserExists(inputUsername))
            {
                throw new UserAlreadyExistsException();
            }

            var user = new User(inputUsername, inputPassword);
            string HashedPassword = _passwordHasher.HashPassword(user, inputPassword);
            user.Password = HashedPassword;
            string authToken = Guid.NewGuid().ToString();
            user.AuthToken = authToken;

            // 201: succesfully created -> save user in database
            _userRepository.AddUser(user);
        }
        #endregion

        #region Login

        // Login Http "POST /sessions"
        public void Login(string inputUsername, string inputPassword, out string authToken)
        {
            var user = _userRepository.GetUserByUsername(inputUsername);
            if (user == null)
                throw new UserNotFoundException("Invalid username or user does not exist.");

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, inputPassword);
            if (verificationResult != PasswordVerificationResult.Success)
                throw new UnauthorizedException("Invalid password.");

            authToken = $"{user.Username}-mtcgToken";
            user.AuthToken = authToken;

            _userRepository.UpdateUser(user);
        }

        #endregion

        #region SessionValidation
        public string GetAuthToken(Dictionary<string, string> headers)
        {
            if (!headers.TryGetValue("Authorization", out var authToken) || string.IsNullOrWhiteSpace(authToken))
            {
                throw new UnauthorizedException(); 
            }

            return authToken;
        }
        // validate each action by checking if user is logged in and authToken is valid
        // Optionally: also checks if path matches authtoken eg users/{username} = {username}-mtcgToken but only if there is a path
        // Optionally: also checks if user is admin and validates token
        public void EnsureAuthenticated(string authToken, string? username = null, bool allowAdmin = false)
        {
            // check if hes admin
            if (allowAdmin && IsAdmin(authToken))
            {
                return; 
            }

            // check if path username matches authtoken by comparring pulled users authtoken with inserted authtoken
            if (username != null)
            {
                var user = _userRepository.GetUserByUsername(username);

                if (user == null)
                {
                    throw new UserNotFoundException();
                }

                if (user.AuthToken != authToken)
                {
                    throw new UnauthorizedException("Authentication token does not match.");
                }
                return;
            }

            // validation without specfic path username
            if (!_userRepository.ValidateToken(authToken))
            {
                throw new UnauthorizedException("Authentication token is invalid.");
            }
        }
        public bool IsAdmin(string authToken)
        {
            var user = _userRepository.GetUserByToken(authToken);
            return user != null && user.Username == "admin"; 
        }
        #endregion
    }
}
