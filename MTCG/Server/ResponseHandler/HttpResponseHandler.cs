using MTCG.Models.ResponseObject;
using MTCG.Utilities;
using MTCG.Utilities.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server.ResponseHandler
{
    public class HttpResponseHandler
    {
        public void SendResponse(StreamWriter writer, ResponseObject response)
        {
            Console.WriteLine("[Server] Sending HTTP response to client");
            int statusCode = response.StatusCode;
            string responseBody = (response.ResponseBody is string)
                ? response.ResponseBody
                : Helpers.CreateStandardJsonResponse(response.ResponseBody);

            writer.WriteLine($"HTTP/1.1 {statusCode}");
            writer.WriteLine("Content-Type: application/json");
            writer.WriteLine("Content-Length: " + responseBody.Length);
            writer.WriteLine();
            writer.WriteLine(responseBody);

            // Flush to ensure all data is sent to the client
            writer.Flush();
        }
    }
}
