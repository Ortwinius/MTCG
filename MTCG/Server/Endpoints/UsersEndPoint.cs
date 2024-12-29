using Microsoft.Extensions.DependencyInjection;
using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
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
                case "GET":
                    if (path.StartsWith("/users/"))
                    {
                        var match = Regex.Match(path, @"^/users/(?<username>[^/]+)$");
                        if (match.Success)
                        {
                            var username = match.Groups["username"].Value;
                            return GetUserData(username, headers);
                        }
                    }
                    break;
                case "PUT":
                    if (path.StartsWith("/users/"))
                    {
                        var match = Regex.Match(path, @"^/users/(?<username>[^/]+)$");
                        if (match.Success)
                        {
                            var username = match.Groups["username"].Value;
                            //return UpdateUserData(username, body, headers);
                            throw new NotImplementedException();
                        }
                    }
                    break;
            }

            return new ResponseObject(405, "Method not allowed.");
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
        /*
         * Returns Name, Bio and image of user
        TODO : throw user not found exception in getUserByValidToken ? or somewhere else 
        */
        private ResponseObject GetUserData(string username, Dictionary<string,string> headers)
        {   
            try
            {
                var token = _authService.GetAuthToken(headers);

                if(!_authService.IsAuthenticated(token))
                {
                    throw new UnauthorizedException();
                }

                var user = _authService.GetUserByValidToken(token);

                var jsonUser = JsonSerializer.Serialize(user, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return new ResponseObject(200, jsonUser);

            }
            catch(UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
            catch(UserNotFoundException)
            {
                return new ResponseObject(404, "User not found.");
            }
        }
        private ResponseObject UpdateUserData()
        {
            throw new NotImplementedException();
        }
    }
}
