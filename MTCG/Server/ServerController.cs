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
using System.Diagnostics;
using MTCG.Server.DI;
using MTCG.Server.Endpoints.Initializer;
using Npgsql.Internal;

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

            EndpointInitializer.InitializeEndpoints(_serviceProvider, _requestHandler);
        }

        /*
        Multithreaded listening to incoming client requests.
        For every request a thread is created (and joined after running)
        */
        public void ListenAsync()
        {
            Console.WriteLine($"[Server] Server listening on http://localhost:{_port}/");
            var server = new TcpListener(IPAddress.Any, _port);
            server.Start();

            while (true)
            {
                var client = server.AcceptTcpClient();
                Task.Run(() => HandleClient(client));
            }
        }

        /*
        Client requests are handled in parallel by using Tasks, see HandleRequestAsync
        */
        private void HandleClient(TcpClient client)
        {
            var timer = Stopwatch.StartNew();
            Console.WriteLine("[Server] Accepted client, executing request");

            using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            using var reader = new StreamReader(client.GetStream());

            try
            {
                var request = _httpParser.Parse(reader);

                var response = _requestHandler.HandleRequest(request);

                _responseHandler.SendResponse(writer, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Error while trying to handle client requests: {ex.Message}");
                _responseHandler.SendResponse(writer, new ResponseObject(500, $"Internal server error: {ex.Message}"));
            }
            finally
            {
                timer.Stop();
                Console.WriteLine($"[Timer] Total time to handle client request: {timer.ElapsedMilliseconds} ms");
            }
        }
    }
}
