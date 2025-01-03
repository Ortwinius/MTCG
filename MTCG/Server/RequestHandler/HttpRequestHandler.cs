using MTCG.Server.Endpoints;
using MTCG.Models.HttpRequest;
using System.Collections.Generic;
using MTCG.Models.ResponseObject;
using System.Text.RegularExpressions;
using MTCG.Utilities;

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
                string pattern = Helpers.ConvertToRegexPattern(path);
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
                return _staticEndpoints[request.Path].HandleRequest(request.Method!, request.Path, request.Body, request.Headers);
            }

            foreach (var (pattern, endpoint) in _dynamicEndpoints)
            {
                if (Regex.IsMatch(request.Path, pattern))
                {
                    var routeParams = Helpers.ExtractRouteParameters(request.Path, pattern);
                    return endpoint.HandleRequest(request.Method!, request.Path, request.Body, request.Headers, routeParams);
                }
            }

            Console.WriteLine($"[Server] Error: Endpoint {request.Path} not found");
            return new ResponseObject(404, "Endpoint not Found");
        }

    }
}
