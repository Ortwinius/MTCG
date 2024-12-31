using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using MTCG.Server.Endpoints;
using MTCG.Models.ResponseObject;
using Microsoft.Extensions.DependencyInjection;
using MTCG.Server.Parser;
using MTCG.Utilities;
using MTCG.Server.RequestHandler;
using MTCG.Server.ResponseHandler;

namespace MTCG.Server
{
    public class ServerController
    {
        private readonly IServiceProvider _serviceProvider; // DI-Container
        private readonly HttpRequestHandler _requestHandler;
        private readonly HttpResponseHandler _responseHandler;
        private readonly HttpParser _httpParser;
        private static readonly int _port = Constants.ServerPort;

        public ServerController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _requestHandler = new HttpRequestHandler();
            _httpParser = new HttpParser();
            _responseHandler = new HttpResponseHandler();

            InitializeEndpoints();
        }

        public void Listen()
        {
            Console.WriteLine($"[Server] Server listening on http://localhost:{_port}/");
            var server = new TcpListener(IPAddress.Any, _port);
            server.Start();

            while (true)
            {
                var client = server.AcceptTcpClient();
                HandleClient(client);
            }
        }

        /*
        Client requests are handled in parallel by using Tasks, see HandleRequestAsync
        */
        private void HandleClient(TcpClient client)
        {
            try
            {
                Console.WriteLine("[Server] Accepted client, executing request");
                using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                using var reader = new StreamReader(client.GetStream());

                var request = _httpParser.Parse(reader);
                var response = _requestHandler.HandleRequestAsync(request);

                _responseHandler.SendResponse(writer, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Error while trying to handle client requests: " + ex.Message);
            }
        }

        private void InitializeEndpoints()
        {
            Console.WriteLine("[Server] Initializing endpoint mapping");
            // retrieve registered services from DI container
            var usersEndpoint = _serviceProvider.GetRequiredService<UsersEndpoint>();
            var sessionsEndpoint = _serviceProvider.GetRequiredService<SessionsEndpoint>();
            var packagesEndpoint = _serviceProvider.GetRequiredService<PackagesEndpoint>();
            var cardsEndpoint = _serviceProvider.GetRequiredService<CardsEndpoint>();
            var deckEndpoint = _serviceProvider.GetRequiredService<DeckEndpoint>();
            var statsEndpoint = _serviceProvider.GetRequiredService<StatsEndpoint>();
            var scoreboardEndpoint = _serviceProvider.GetRequiredService<ScoreboardEndpoint>();
            var battlesEndpoint = _serviceProvider.GetRequiredService<BattlesEndpoint>();

            // add endpoints to requestHandler
            _requestHandler.AddEndpoint("/users", usersEndpoint);
            _requestHandler.AddEndpoint("/users/{username}", usersEndpoint);
            _requestHandler.AddEndpoint("/sessions", sessionsEndpoint);
            _requestHandler.AddEndpoint("/packages", packagesEndpoint);
            _requestHandler.AddEndpoint("/cards", cardsEndpoint);
            _requestHandler.AddEndpoint("/transactions/packages", packagesEndpoint);
            _requestHandler.AddEndpoint("/deck", deckEndpoint);
            _requestHandler.AddEndpoint("/deck?format=plain", deckEndpoint);
            _requestHandler.AddEndpoint("/stats", statsEndpoint);
            _requestHandler.AddEndpoint("/scoreboard", scoreboardEndpoint);
            _requestHandler.AddEndpoint("/battles", battlesEndpoint);
        }
    }
}
