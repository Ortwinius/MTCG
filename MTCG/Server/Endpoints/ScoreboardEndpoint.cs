using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Utilities;
using MTCG.Utilities.CustomExceptions;
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
                _authService.EnsureAuthenticated(token);
                var scoreboard = _userService.GetAllUserStats(token);
                // JSON serialize
                var jsonScoreboard = JsonSerializer.Serialize(scoreboard, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return new ResponseObject(200, jsonScoreboard);
            }
            catch (UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
        }
    }

}
