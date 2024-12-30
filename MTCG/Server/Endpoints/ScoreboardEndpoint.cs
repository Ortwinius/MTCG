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
    public class ScoreboardEndpoint : IHttpEndpoint
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        public ScoreboardEndpoint(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }
        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string body)
        {
            switch (method)
            {
                case "GET":
                    return GetScoreboard(headers);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }
        private ResponseObject GetScoreboard(Dictionary<string, string> headers)
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
            catch (Exception e)
            {
                return new ResponseObject(400, Helpers.CreateStandardJsonResponse(e.Message));
            }
        }
    }

}
