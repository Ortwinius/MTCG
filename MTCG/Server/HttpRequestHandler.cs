using MTCG.Server.Endpoints;
using MTCG.Models.HttpRequest;
using System.Collections.Generic;
using MTCG.Models.ResponseObject;

namespace MTCG.Server
{
    public class HttpRequestHandler
    {
        private Dictionary<string, IHttpEndpoint> _endpoints;

        public HttpRequestHandler()
        {
            // Dictionary of all available endpoints
            _endpoints = new Dictionary<string, IHttpEndpoint>();
        }
        
        public void AddEndpoint(string path, IHttpEndpoint endpoint)
        {
            _endpoints[path] = endpoint;
        }

        public ResponseObject HandleRequest(HttpRequest request)
        {
            // Look for the exact path in the endpoints
            if (_endpoints.ContainsKey(request.Path))
            {
                return _endpoints[request.Path].HandleRequest(request.Method, request.Path, request.Headers, request.Body);
            }

            // If no exact match, return 404
            return new ResponseObject(404, "Not Found");
        }
    }
}
