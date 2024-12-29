using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Utilities.CustomExceptions;
using System;
using System.Text.Json;

namespace MTCG.Server.Endpoints
{
    // Endpoint for user login, authservice required
    public class SessionsEndpoint : IHttpEndpoint
    {
        private readonly AuthService _authService;

        public SessionsEndpoint(AuthService authService)
        {
            _authService = authService;
        }

        // This method now returns a ResponseObject
        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string body)
        {
            switch (method)
            {
                case "POST":
                    return LoginUser(body);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }

        private ResponseObject LoginUser(string body)
        {
            try
            {
                User? user = JsonSerializer.Deserialize<User>(body);

                if (user == null)
                {
                    return new ResponseObject(400, "Invalid JSON structure");
                }

                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                {
                    return new ResponseObject(400, "Username and password must not be empty");
                }

                string authToken = "";
                _authService.Login(user.Username, user.Password, out authToken);
                return new ResponseObject(200, $"Login successful. AuthToken: {authToken}");
            }
            catch (UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                Console.WriteLine($"JSON string: {body}");
                return new ResponseObject(400, "Invalid JSON provided");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new ResponseObject(500, "Internal server error");
            }
        }
    }
}
