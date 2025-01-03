using Microsoft.Extensions.DependencyInjection;
using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
using MTCG.Models.Users.DTOs;
using MTCG.Repositories;
using MTCG.Utilities;
using MTCG.Utilities.Exceptions.CustomExceptions;
using System.ComponentModel.Design;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MTCG.Server.Endpoints
{
    public class UsersEndpoint : IHttpEndpoint
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        public UsersEndpoint(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        /*
        According to API:
        POST /users
        GET /users/{username}
        */
        public ResponseObject HandleRequest(
            string method, 
            string path, 
            string? body, 
            Dictionary<string, string> headers, 
            Dictionary<string, string>? routeParams = null)
        {
            switch(method)
            {
                case "POST":
                    return RegisterUser(body!);
                case "GET" when routeParams != null && routeParams.ContainsKey("username"):
                    return GetUserData(routeParams["username"], headers);
                case "PUT" when routeParams != null && routeParams.ContainsKey("username"):
                    return UpdateUserData(routeParams["username"], body!, headers);
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
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }
        /*
        Returns Name, Bio and image of user
        */
        private ResponseObject GetUserData(string username, Dictionary<string, string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);

                _authService.EnsureAuthenticated(token, username, allowAdmin: true);

                var userData = _userService.GetUserDataByToken(token);

                var jsonUserData = JsonSerializer.Serialize(userData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return new ResponseObject(200, jsonUserData);
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }

        private ResponseObject UpdateUserData(string username, string body, Dictionary<string,string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);

                _authService.EnsureAuthenticated(token, username, allowAdmin: true);

                var userData = JsonSerializer.Deserialize<UserDataDTO>(body);

                _userService.UpdateUserData(username, userData!);
                return new ResponseObject(200, "User data successfully updated.");
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }
    }
}
