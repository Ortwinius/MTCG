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
    public class UsersEndPoint : IHttpEndpoint
    {
        private readonly AuthService _authService;
        private readonly UserRepository _userRepository;
        public UsersEndPoint(AuthService authService, UserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        public ResponseObject HandleRequest(string method, string path, string body)
        {
            switch(method)
            {
                case "POST":
                    return RegisterUser(body);
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
    }
}
