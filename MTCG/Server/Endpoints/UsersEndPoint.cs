using Microsoft.Extensions.DependencyInjection;
using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
using MTCG.Models.Users.DTOs;
using MTCG.Repositories;
using MTCG.Utilities;
using MTCG.Utilities.CustomExceptions;
using System.ComponentModel.Design;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MTCG.Server.Endpoints
{
    public class UsersEndpoint : IHttpEndpoint
    {
        private readonly AuthService _authService;
        public UsersEndpoint(AuthService authService)
        {
            _authService = authService;
        }

        /*
        According to API:
        POST /users
        GET /users/{username}
        */
        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string body)
        {
            switch(method)
            {
                case "POST":
                    return RegisterUser(body);
                case "GET" when path.StartsWith("/users/"):                 
                    return GetUserData(Helpers.ExtractUsernameFromPath(path), headers);
                case "PUT" when path.StartsWith("/users/"):
                    return UpdateUserData(Helpers.ExtractUsernameFromPath(path), body, headers);
            }

            return new ResponseObject(405, "Method not allowed.");
        }
        private ResponseObject RegisterUser(string body)
        {
            try
            {
                User? user = JsonSerializer.Deserialize<User>(body);

                if (user == null)
                {
                    return new ResponseObject(400, "Invalid user data.");
                }

                _authService.Register(user.Username, user.Password);

                return new ResponseObject(201, "User successfully registered.");
            }
            catch (UserAlreadyExistsException)
            {
                return new ResponseObject(409, "User with same username already registered.");
            }
        }
        /*
         * Returns Name, Bio and image of user
        TODO : throw user not found exception in getUserByValidToken ? or somewhere else 
        */
        private ResponseObject GetUserData(string username, Dictionary<string, string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);

                _authService.EnsureAuthenticated(token, username, allowAdmin: true);

                var userData = _authService.GetUserDataByToken(token);

                var jsonUserData = JsonSerializer.Serialize(userData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return new ResponseObject(200, jsonUserData);
            }
            catch (UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
            catch (UserNotFoundException)
            {
                return new ResponseObject(404, "User not found.");
            }
        }

        private ResponseObject UpdateUserData(string username, string body, Dictionary<string,string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);

                _authService.EnsureAuthenticated(token, username, allowAdmin: true);

                // parse UserDataDTO from body
                var userData = JsonSerializer.Deserialize<UserDataDTO>(body);
                // print it:
                Console.WriteLine($"[UsersEndpoint] Updating user data: {userData!.Name}, {userData.Bio}, {userData.Image}");

                _authService.UpdateUserData(username, userData!);
                return new ResponseObject(200, "User data successfully updated.");
            }
            catch(UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
            catch (UserNotFoundException)
            {
                return new ResponseObject(404, "User not found.");
            }
        }
    }
}
