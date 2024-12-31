using System.Collections.Concurrent;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
using MTCG.BusinessLogic.Services;
using MTCG.Utilities.CustomExceptions;

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

        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string body)
        {
            switch(method)
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
                // Authenticate user
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                Console.WriteLine($"[Lobby] {user!.Username} entered the lobby.");

                // Add user to the lobby and check for an opponent
                Lobby.Enqueue(user);


                if (Lobby.Count >= 1 && Lobby.TryDequeue(out var opponent))
                {
                    if (opponent.Username != user.Username)
                    {
                        Console.WriteLine($"[BattlesEndpoint] ");
                        _battleService.StartBattle(user, opponent);
                    }
                }
                

                return new ResponseObject(202, "Waiting for an opponent...");
            }
            catch(DeckIsNullException)
            {
                return new ResponseObject(400, "Battle cannot start without configured deck.");
            }
            catch (UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
                return new ResponseObject(500, "Internal server error.");
            }
        }
    }
}
