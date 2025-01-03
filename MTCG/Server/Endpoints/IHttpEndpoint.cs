using MTCG.Models.ResponseObject;

namespace MTCG.Server.Endpoints
{
    public interface IHttpEndpoint
    {
        public ResponseObject? HandleRequest(
            string method, 
            string path,
            string? body,
            Dictionary<string, string> headers, 
            Dictionary<string, string>? routeParams = null
            );
    }
}