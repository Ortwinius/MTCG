using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTCG.Server.Endpoints
{
    public class StatsEndpoint : IHttpEndpoint
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        public StatsEndpoint(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }
        public ResponseObject HandleRequest(
            string method,
            string path,
            string? body,
            Dictionary<string, string> headers,
            Dictionary<string, string>? routeParams = null)
        {
            switch (method)
            {
                case "GET":
                    return GetUserStats(headers);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }
        private ResponseObject GetUserStats(Dictionary<string, string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);
                var userStats = _userService.GetUserStatsByToken(token);
                // JSON serialize
                var jsonUserStats = JsonSerializer.Serialize(userStats, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return new ResponseObject(200, jsonUserStats);
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }
    }

}
