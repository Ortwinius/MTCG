using System.Collections.Generic;
using System.Text.Json;
using MTCG.Models.ResponseObject;
using MTCG.Models.Users;
using MTCG.BusinessLogic.Services;
using System.Threading.Tasks;

namespace MTCG.Server.Endpoints
{
    public class BattlesEndpoint : IHttpEndpoint
    {
        private static Queue<(TaskCompletionSource<string> Tcs, User User)> _userQueue = new();
        private static readonly object _lobbyLock = new();
        private static readonly object _writeLock = new(); 
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
                // Authentifiziere den Benutzer
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                Console.WriteLine($"[Lobby] {user!.Username} entered the lobby.");

                // Initialisiere TaskCompletionSource
                var tcs = new TaskCompletionSource<string>();

                lock (_lobbyLock)
                {
                    if (_userQueue.Count > 0)
                    {
                        var (opponentTcs, opponentUser) = _userQueue.Dequeue();

                        Console.WriteLine($"[Lobby] Match found: {user.Username} vs {opponentUser.Username}");

                        // Start the battle
                        var battleLog = _battleService.TryBattle(user, opponentUser);
                        var formattedLog = string.Join("\n", battleLog);

                        // synchronize writing to the TaskCompletionSources
                        Monitor.Enter(_writeLock);
                        try
                        {
                            tcs.SetResult(formattedLog);
                        }
                        finally
                        {
                            Monitor.Exit(_writeLock);
                        }

                        Monitor.Enter(_writeLock);
                        try
                        {
                            opponentTcs.SetResult(formattedLog);
                        }
                        finally
                        {
                            Monitor.Exit(_writeLock);
                        }
                    }
                    else
                    {
                        // If no opponent is available, add user to queue
                        _userQueue.Enqueue((tcs, user));
                        Console.WriteLine($"[Lobby] {user.Username} is waiting for an opponent...");
                    }
                }

                // Timeout-Logik
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));
                var completedTask = Task.WhenAny(tcs.Task, timeoutTask).Result;

                if (completedTask == timeoutTask)
                {
                    lock (_lobbyLock)
                    {
                        // Remove user from queue if still present
                        _userQueue = new Queue<(TaskCompletionSource<string>, User)>(
                            _userQueue.Where(entry => entry.Tcs != tcs));
                    }

                    Console.WriteLine($"[Lobby] Timeout for user {user.Username}");
                    return new ResponseObject(408, "No opponent found for user.");
                }

                return new ResponseObject(200, $"Battle Request successful. Log:\n{tcs.Task.Result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Exception occurred in EnterLobby: {ex.Message}");
                return ExceptionHandler.HandleException(ex);
            }
        }
    }
}
