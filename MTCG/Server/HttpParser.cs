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
        // Parse Request Line (e.g., "POST /users HTTP/1.1")
        public HttpRequest Parse(StreamReader reader)
        {
            var request = new HttpRequest();
            string? line = reader.ReadLine();

            if (line == null)
            {
                return request;
            }

            var requestLineParts = line.Split(' ');
            request.Method = requestLineParts[0];
            request.Path = requestLineParts[1];
            request.Version = requestLineParts[2];

            // Parse Headers
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length == 0) break; // End of headers

                var headerParts = line.Split(':',2);
                if(headerParts.Length == 2)
                {
                    var key = headerParts[0].Trim();
                    var value = headerParts[1].Trim();
                    request.Headers[key] = value;
                }
            }
            request.Headers = FormatHeaders(request.Headers);

            // Parse Body (if content length is present)
            if (request.Headers.ContainsKey("Content-Length"))
            {
                int contentLength = int.Parse(request.Headers["Content-Length"]);
                char[] buffer = new char[contentLength];
                int readLength = reader.Read(buffer, 0, contentLength);
                request.Body = new string(buffer, 0, readLength); 
            }

            return request;
        }
        // used to trim bearer from authentication token
        private Dictionary<string, string> FormatHeaders(Dictionary<string, string> headers)
        {
            var formattedHeaders = new Dictionary<string, string>();

            foreach (var header in headers)
            {
                var key = header.Key.Trim();
                var value = header.Value.Trim();

                // Normalize the Authorization header
                if (key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) && value.StartsWith("Bearer "))
                {
                    value = value.Substring("Bearer ".Length).Trim(); // Remove "Bearer " prefix
                }

                formattedHeaders[key] = value;
            }

            return formattedHeaders;
        }

    }
}
