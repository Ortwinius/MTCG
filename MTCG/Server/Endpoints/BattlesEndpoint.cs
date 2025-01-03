using System.Collections.Concurrent;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
using MTCG.BusinessLogic.Services;
using System.Diagnostics;
using MTCG.Utilities.Exceptions.CustomExceptions;

namespace MTCG.Server.Endpoints
{
    public class BattlesEndpoint : IHttpEndpoint
    {
        private static readonly ConcurrentQueue<User> Lobby = new();
        private readonly BattleService _battleService;
        private readonly AuthService _authService;
        private readonly UserService _userService;
        public BattlesEndpoint(BattleService battleService, AuthService authService, UserService userService)
        {
            _battleService = battleService;
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
                case "POST":
                    return EnterLobby(headers);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }

        private ResponseObject EnterLobby(Dictionary<string, string> headers)
        {
            try
            {

                // authenticate user
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                Console.WriteLine($"[Lobby] {user!.Username} entered the lobby.");

                User? opponent = null;

                // check if there is an opponent in the lobby
                if (Lobby.TryDequeue(out opponent) && opponent != user)
                {
                    Console.WriteLine($"[Lobby] Match found: {user.Username} vs {opponent.Username}");

                    var battleLog = _battleService.TryBattle(user, opponent);

                    var formattedBattleLog = string.Join("\n", battleLog);
                    return new ResponseObject(200, $"Battle Request successful. Log: \n{formattedBattleLog}");
                }
                else
                {
                    // no opponent found, add user to lobby
                    Lobby.Enqueue(user);
                    Console.WriteLine($"[Lobby] {user.Username} is waiting for an opponent...");
                    return new ResponseObject(202, "Waiting for an opponent...");
                }
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }
    }
}
