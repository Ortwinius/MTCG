using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MTCG.BusinessLogic.Manager;

namespace MTCG.Controllers
{
    public class ServerController
    {
        private GameManager _gameManager;

        public ServerController(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void RegisterRequest(string username, string password)
        {
            _gameManager.RegisterUser(username, password);
        }

        public void ConfigureDeckRequest(string username, string[] cardIds)
        {
            _gameManager.ConfigureUserDeck(username, cardIds);
        }

        public void BattleRequest(string username)
        {
            _gameManager.JoinBattle(username);
        }
        public void Demo()
        {
            Console.WriteLine("Server-Demo^use port http://localhost:8000/");
            // Start TCP Server and interpret textual data as HTTP
            var server = new TcpListener(IPAddress.Any, 8000);
            server.Start();

            // Request loop
            while (true)
            {

                // Wait for incoming connection
                var client = server.AcceptTcpClient();
                using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                using var reader = new StreamReader(client.GetStream());

                // 2. HTTP-Request - 1st line
                // GET /users HTTP/1.1
                string? line;
                line = reader.ReadLine();
                Console.WriteLine(line);
                // split line via spaces and save string arraY in httpParts
                var httpParts = line.Split(' ');
                var method = httpParts[0];
                var path = httpParts[1];
                var version = httpParts[2];
                Console.WriteLine($"Method: {method}, Path: {path}, Version: {version}");

                // 3. HTTP Request - Headers
                //e.g.: "User Agent: curl/7.68.0"
                // "Accept: */*"
                int content_length = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    if (line.Length == 0)
                    {
                        break; // empty line indicates the end of HTTP-headers
                    }

                    // Parse the header
                    var headerParts = line.Split(":");
                    var headerName = headerParts[0];
                    var headerValue = headerParts[1].Trim();
                    Console.WriteLine($"Header: {headerName}={headerValue}");
                    if (headerName == "Content-Length")
                    {
                        content_length = int.Parse(headerValue);
                    }
                }

                //Http Response
                // Add chars to requestBody in binary style
                StringBuilder requestBody = new StringBuilder();
                if (content_length > 0)
                {
                    char[] chars = new char[1024];
                    int bytesReadTotal = 0;

                    while (bytesReadTotal < content_length)
                    {
                        var bytesRead = reader.Read(chars, 0, chars.Length);
                        bytesReadTotal += bytesRead;
                        if (bytesReadTotal == 0)
                        {
                            break;
                        }
                        // append char buffer to requestBody
                        requestBody.Append(chars, 0, bytesReadTotal);
                    }
                }
                Console.WriteLine($"Body: {requestBody.ToString()}");
                writer.WriteLine("Hello World");
            }
        }   

    }
}
