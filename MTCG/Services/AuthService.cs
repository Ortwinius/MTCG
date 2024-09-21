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
        public static string Register(User user, string inputUsername, string inputPassword)
        {
            Console.WriteLine($"Registering... Welcome {user.Username}");
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Hash the password and save it in user
            var hashedPassword = passwordHasher.HashPassword(user, inputPassword);
            user.HashedPassword = hashedPassword;
            user.Username = inputUsername;

            // Generate authToken 
            string authToken = Guid.NewGuid().ToString();

            // Save the user with hashedPassword and token in the database 
            // TODO: SaveUserToDatabase(user);

            return authToken;
        }
        public static void Login(User user, string inputUsername, string inputPassword)
        {
            //if username || password arent correct or username cant be found in database -> error

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

