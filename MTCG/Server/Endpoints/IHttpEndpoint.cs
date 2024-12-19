using MTCG.Models.ResponseObject;

namespace MTCG.Server.Endpoints
{
    public interface IHttpEndpoint
    {
        public ResponseObject? HandleRequest(string method, string path, Dictionary<string,string> headers, string body);
    }
}