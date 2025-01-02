using MTCG.Models.ResponseObject;

namespace MTCG.Server.Endpoints.Initializer
{
    public class TradingsEndpoint : IHttpEndpoint
    {
        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string? body)
        {
            throw new NotImplementedException();
        }
    }
}