using MTCG.Models.ResponseObject;
using MTCG.Utilities;

public class HttpResponseHandler
{
    public void SendResponse(StreamWriter writer, ResponseObject? response)
    {
        if (response == null)
        {
            response = new ResponseObject(500, "Internal server error");
        }


        Console.WriteLine("[Server] Sending HTTP response to client");
        int statusCode = response.StatusCode;

        string responseBody = response.ResponseBody is string
            ? response.ResponseBody
            : Helpers.CreateStandardJsonResponse(response.ResponseBody);

        writer.WriteLine($"HTTP/1.1 {statusCode}");
        writer.WriteLine("Content-Type: application/json");
        writer.WriteLine("Content-Length: " + responseBody.Length);
        writer.WriteLine();
        writer.WriteLine(responseBody);

        writer.Flush();
        
    }
}
