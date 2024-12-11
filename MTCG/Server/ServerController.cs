using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using MTCG.Server.Endpoints;
using MTCG.Models.ResponseObject;
using Microsoft.Extensions.DependencyInjection;
using MTCG.Utilities;

namespace MTCG.Server
{
    public class ServerController
    {
        private readonly IServiceProvider _serviceProvider; // DI-Container
        private readonly HttpRequestHandler _requestHandler;
        private readonly HttpParser _httpParser;
        private static readonly int _port = Constants.ServerPort;

        public ServerController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _requestHandler = new HttpRequestHandler();
            _httpParser = new HttpParser();

            InitializeEndpoints();
        }

        private void InitializeEndpoints()
        {
            // retrieve registered services from DI container
            var usersEndpoint = _serviceProvider.GetRequiredService<UsersEndPoint>();
            var sessionsEndpoint = _serviceProvider.GetRequiredService<SessionsEndpoint>();

            // add endpoints to requestHandler
            _requestHandler.AddEndpoint("/users", usersEndpoint);
            _requestHandler.AddEndpoint("/sessions", sessionsEndpoint);
        }

        public void Listen()
        {
            Console.WriteLine($"Server listening on http://localhost:{_port}/");
            var server = new TcpListener(IPAddress.Any, _port);
            server.Start();

            while (true)
            {
                var client = server.AcceptTcpClient();
                HandleClient(client);
            }
        }

        private void HandleClient(TcpClient client)
        {
            using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            using var reader = new StreamReader(client.GetStream());

            // Parse HTTP Request
            var request = _httpParser.Parse(reader);
            // Handle Request and get response
            var response = _requestHandler.HandleRequest(request);
            // Send Response to client
            SendResponse(writer, response);
        }

        private void SendResponse(StreamWriter writer, ResponseObject response)
        {
            try
            {
                int statusCode = response.StatusCode;
                string responseBody = Helpers.CreateJsonResponse(response.ResponseBody);

                // Write the HTTP status line correctly with status code and description
                writer.WriteLine($"HTTP/1.1 {statusCode}");

                // Write the headers
                writer.WriteLine("Content-Type: application/json");
                writer.WriteLine("Content-Length: " + responseBody.Length); // Length of the body
                writer.WriteLine();  // End headers

                // Write the response body
                writer.WriteLine(responseBody);  // responseBody is the correct JSON body

                // Flush to ensure all data is sent to the client
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error while copying content to a stream: " + ex.Message);
                // Handle or log the error as needed
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket error: " + ex.Message);
                // Handle socket errors if necessary
            }
        }
    }
}
