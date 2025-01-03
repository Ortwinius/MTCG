using System.Collections.Concurrent;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
using MTCG.BusinessLogic.Services;
using MTCG.Utilities.CustomExceptions;
using System.Diagnostics;

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

        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string? body, Dictionary<string, string>? routeParams = null)
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
            Console.WriteLine("[Timer] EnterLobby: Start processing lobby logic");
            var timer = Stopwatch.StartNew();
            try
            {

                // Authenticate user
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                Console.WriteLine($"[Lobby] {user!.Username} entered the lobby.");

                User? opponent = null;

                // Versuche, einen Gegner zu finden
                if (Lobby.TryDequeue(out opponent) && opponent != user)
                {
                    Console.WriteLine($"[Lobby] Match found: {user.Username} vs {opponent.Username}");

                    // Starte den Kampf
                    var battleLog = _battleService.TryBattle(user, opponent);
                    Console.WriteLine($"[Timer] After battle logic: {timer.ElapsedMilliseconds} ms");

                    // Gib den Battle-Log zurück
                    var jsonLog = string.Join("\n", battleLog);
                    Console.WriteLine($"[Timer] Lobby logic complete. Total Time: {timer.ElapsedMilliseconds} ms");
                    return new ResponseObject(200, $"Battle Request successful. Log: \n{jsonLog}");
                }
                else
                {
                    // Kein Gegner gefunden, füge Benutzer zur Queue hinzu
                    Lobby.Enqueue(user);
                    Console.WriteLine($"[Lobby] {user.Username} is waiting for an opponent...");
                    return new ResponseObject(202, "Waiting for an opponent...");
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
