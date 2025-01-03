using MTCG.Models.ResponseObject;
using MTCG.Utilities;
using MTCG.Utilities.Exceptions.CustomExceptions;
using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server.ResponseHandler
{
    public class HttpResponseHandler
    {
        public void SendResponse(StreamWriter writer, ResponseObject? response)
        {
            // if response is null, return a 500 Internal Server Error
            if (response == null)
            {
                response = new ResponseObject(500, "Internal server error");
            }

            Console.WriteLine("[Server] Sending HTTP response to client");
            int statusCode = response.StatusCode;

            // make sure the body is not JSONified twice
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
