using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Repositories;
using MTCG.Server.Endpoints;
using MTCG.Utilities;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MTCG.Server
{
    public class ServerController
    {
        private HttpRequestHandler _requestHandler;
        private HttpParser _httpParser;
        private static int _port = 10001;

        public ServerController()
        {
            _requestHandler = new HttpRequestHandler();
            _httpParser = new HttpParser();

            // Initialize UserRepository
            var userRepository = new UserRepository();

            // Initialize AuthService singleton with UserRepository
            var authService = AuthService.GetInstance(userRepository);

            // Initialize your endpoints with the necessary services
            var usersEndpoint = new UsersEndpoint(authService, userRepository);
            var sessionsEndpoint = new SessionsEndpoint(authService, userRepository);

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
