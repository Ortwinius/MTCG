using MTCG.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MTCG.Services
{
    public static class AuthService
    {
        private static PasswordHasher<User> passwordHasher = new PasswordHasher<User>();

        // Registers the user and generates an authToken if successful
        // TODO: SAVE USER IN DATABASE
        public static void Register(User user, string inputUsername, string inputPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Hash the password and save it in user
            var hashedPassword = passwordHasher.HashPassword(user, inputPassword);

            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new Exception("Password hashing could not be executed");
            }

            user.HashedPassword = hashedPassword;
            user.Username = inputUsername;

            // Generate authToken 
            string authToken = Guid.NewGuid().ToString();

            if (string.IsNullOrEmpty(authToken))
            {
                throw new Exception("AuthToken-generation was not successful");
            }
            user.AuthToken = authToken;
            // Save the user with hashedPassword and token in the database 
            // TODO: SaveUserToDatabase(user);
            Console.WriteLine($"Registration successful");
        }
        public static void Login(User user, string inputUsername, string inputPassword)
        {
            // If username || password arent correct or username cant be found in database -> error

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Verify the password against the stored hash
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, inputPassword);

            if (verificationResult != PasswordVerificationResult.Success || inputUsername != user.Username)
            {
                throw new UnauthorizedAccessException("Invalid login credentials.");
            }

            // Set user as logged in 
            user.IsLoggedIn = true;

            Console.WriteLine($"Logging in... Welcome {user.Username}!");
        }

        public static void Logout(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Console.WriteLine($"Logging out {user.Username}...");
            user.IsLoggedIn = false;
        }
    }
}

