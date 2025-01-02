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
        private static readonly object LobbyLock = new object();
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

                lock (LobbyLock)
                {
                    Lobby.Enqueue(user);

                    while (true)
                    {
                        // Check if there are at least 2 users in the lobby
                        if (Lobby.Count > 1)
                        {
                            // Try to dequeue two users for a battle
                            if (Lobby.TryDequeue(out var opponent) && opponent != user)
                            {
                                Console.WriteLine($"[Lobby] Match found: {user.Username} vs {opponent.Username}");
                                var battleLog = _battleService.StartBattle(user, opponent);

                                // Serialize the battle log into JSON response
                                var jsonLog = string.Join("\n", battleLog);
                                return new ResponseObject(200, $"Battle Request successful. Log: \n{jsonLog}");
                            }
                        }

                        // If not enough users, wait
                        Console.WriteLine($"[Lobby] {user.Username} is waiting for an opponent...");
                        Monitor.Wait(LobbyLock);
                    }

                }
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
