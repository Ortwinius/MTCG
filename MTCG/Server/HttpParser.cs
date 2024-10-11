using MTCG.Models.HttpRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server
{
    public class HttpParser
    {
        public HttpRequest Parse(StreamReader reader)
        {
            var request = new HttpRequest();

            // Parse Request Line (e.g., "POST /users HTTP/1.1")
            string? line = reader.ReadLine();
            var requestLineParts = line.Split(' ');
            request.Method = requestLineParts[0];
            request.Path = requestLineParts[1];
            request.Version = requestLineParts[2];

            // Parse Headers
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length == 0) break; // End of headers
                var headerParts = line.Split(':');
                request.Headers[headerParts[0]] = headerParts[1].Trim();
            }

            // Parse Body (if content length is present)
            if (request.Headers.ContainsKey("Content-Length"))
            {
                int contentLength = int.Parse(request.Headers["Content-Length"]);
                char[] buffer = new char[contentLength];
                reader.Read(buffer, 0, contentLength);
                request.Body = new string(buffer);
            }

            return request;
        }
    }
}
