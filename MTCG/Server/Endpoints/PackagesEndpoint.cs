using MTCG.Models.Card;
using MTCG.Models.ResponseObject;
using MTCG.BusinessLogic.Services;
using System.Text.Json;
using MTCG.Models.Package;
using MTCG.Utilities.CardJsonConverter;

namespace MTCG.Server.Endpoints
{
    public class PackagesEndpoint : IHttpEndpoint
    {
        private readonly PackageService _packageService;

        public PackagesEndpoint(PackageService packageService)
        {
            _packageService = packageService;
        }

        public ResponseObject HandleRequest(string method, string path, string body)
        {
            switch (method)
            {
                case "POST":
                    return AddPackage(body);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }

        private ResponseObject AddPackage(string body)
        {
            Console.WriteLine("Received body: " + body);
            // Deserialize the request body into a list of cards
            List<ICard>? cards;
            try
            {
                Console.WriteLine("Trying to deserialize body");
                cards = JsonSerializer.Deserialize<List<ICard>>(body, new JsonSerializerOptions
                {
                    Converters = { new CardJsonConverter() }, // Custom converter
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException)
            {
                return new ResponseObject(400, "Invalid JSON format.");
            }

            // Validate the package (exactly 5 cards required)
            if (cards == null || cards.Count != 5)
            {
                return new ResponseObject(400, "A package must contain exactly 5 cards.");
            }

            try
            {
                // Create and add the package
                if (_packageService.AddPackage(cards))
                {
                    return new ResponseObject(201, "Package successfully created.");
                }

                return new ResponseObject(409, "Conflict: One or more cards already exist.");
            }
            catch (Exception ex)
            {
                return new ResponseObject(500, $"An error occurred while adding the package: {ex.Message}");
            }
        }
    }
}
