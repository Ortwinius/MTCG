using Microsoft.Extensions.DependencyInjection;
using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Utilities;
using System.ComponentModel.Design;
using System.Text.Json;

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
        public ResponseObject HandleRequest(string method, string path, string body)
        {
            switch(method)
            {
                case "POST":
                    return RegisterUser(body);
                case "GET" when path.StartsWith("/users/"):
                    return GetUserData(path);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }
        private ResponseObject RegisterUser(string body)
        {
            User? user = JsonSerializer.Deserialize<User>(body);

            if (user == null)
            {
                return new ResponseObject(400, "Invalid user data.");
            }

            if (_authService.Register(user.Username, user.Password))
            {
                return new ResponseObject(201, "User successfully registered.");
            }
            return new ResponseObject(409, "User already exists.");
        }
        private ResponseObject GetUserData(string path)
        {
            var username = path.Split("/")[2];
            var userData = _authService.GetUserByUsername(username);

            // TODO: convert to string
            return (userData != null)
                ? new ResponseObject(200, "userData: SERIALIZE!")
                : new ResponseObject(404, "User not found.");
        }
    }
}
