using MTCG.Server.Endpoints;
using MTCG.Models.HttpRequest;
using System.Collections.Generic;
using MTCG.Models.ResponseObject;
using System.Text.RegularExpressions;

namespace MTCG.Server.RequestHandler
{
    public class HttpRequestHandler
    {
        private Dictionary<string, IHttpEndpoint> _staticEndpoints;
        private List<(string pattern, IHttpEndpoint endpoint)> _dynamicEndpoints;

        public HttpRequestHandler()
        {
            _staticEndpoints = new Dictionary<string, IHttpEndpoint>();
            _dynamicEndpoints = new List<(string, IHttpEndpoint)>();
        }

        /*
         * doesnt only add static endpoints - dynamic strings like users/{username} are verified via regex and parsed
        */
        public void AddEndpoint(string path, IHttpEndpoint endpoint)
        {
            if (path.Contains("{"))
            {
                string pattern = "^" + path
                    .Replace("{", "(?<")  // Begin a named group
                    .Replace("}", ">[^/]+)") + "$"; // End the named group and specify allowed characters

                _dynamicEndpoints.Add((pattern, endpoint));
            }
            else
            {
                _staticEndpoints[path] = endpoint;
            }
        }


        /*
        Differentiating between static endpoints and dynamic ones like users/{username}
        */
        public ResponseObject? HandleRequest(HttpRequest request)
        {
            if (request.Path == null)
            {
                Console.WriteLine("[Server] Error: Request path is null");
                return new ResponseObject(400, "Bad Request");
            }

            if (_staticEndpoints.ContainsKey(request.Path))
            {
                return _staticEndpoints[request.Path].HandleRequest(request.Method!, request.Path, request.Headers, request.Body);
            }

            foreach (var (pattern, endpoint) in _dynamicEndpoints)
            {
                var match = Regex.Match(request.Path, pattern);
                if (match.Success)
                {
                    var routeParams = new Dictionary<string, string>();
                    foreach (var groupName in match.Groups.Keys)
                    {
                        if (groupName != "0" && match.Groups[groupName].Success)
                        {
                            routeParams[groupName] = match.Groups[groupName].Value;
                        }
                    }

                    return endpoint.HandleRequest(request.Method!, request.Path, request.Headers, request.Body);
                }
            }

            Console.WriteLine($"[Server] Error: Endpoint {request.Path} not found");
            return new ResponseObject(404, "Endpoint not Found");
        }
    }
}
