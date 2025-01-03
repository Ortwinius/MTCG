using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Utilities.Exceptions.CustomExceptions;
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
        public ResponseObject HandleRequest(
            string method,
            string path,
            string? body,
            Dictionary<string, string> headers,
            Dictionary<string, string>? routeParams = null)
        {
            switch (method)
            {
                case "POST":
                    return LoginUser(body!);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }

        private ResponseObject LoginUser(string body)
        {
            try
            {
                var user = JsonSerializer.Deserialize<User>(body);
                if (user == null)
                    throw new JsonException("Invalid JSON payload.");

                _authService.Login(user.Username, user.Password, out var authToken);

                return new ResponseObject(200, $"Login successful. AuthToken: {authToken}");
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }

    }
}
