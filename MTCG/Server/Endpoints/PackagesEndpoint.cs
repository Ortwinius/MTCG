using MTCG.Models.Card;
using MTCG.Models.ResponseObject;
using MTCG.BusinessLogic.Services;
using System.Text.Json;
using MTCG.Utilities.CardJsonConverter;
using MTCG.Utilities.Exceptions.CustomExceptions;

namespace MTCG.Server.Endpoints
{
    public class PackagesEndpoint : IHttpEndpoint
    {
        private readonly PackageService _packageService;
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public PackagesEndpoint(PackageService packageService, AuthService authService, UserService userService)
        {
            _packageService = packageService;
            _authService = authService;
            _userService = userService;
        }

        public ResponseObject HandleRequest(
            string method,
            string path,
            string? body,
            Dictionary<string, string> headers,
            Dictionary<string, string>? routeParams = null)
        {
            switch (method)
            {
                case "POST" when path == "/packages":
                    return AddPackage(body!, headers);
                case "POST" when path == "/transactions/packages":
                    return BuyPackage(headers);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }

        private ResponseObject BuyPackage(Dictionary<string,string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                Console.WriteLine("[PackagesEndpoint] Authenticated user tries to buy a package -> [PackageService]");

                var cards = _packageService.AcquirePackage(user!);

                var jsonCards = JsonSerializer.Serialize(cards, new JsonSerializerOptions
                {
                    Converters = { new CardJsonConverter() },
                    WriteIndented = true
                });

                return new ResponseObject(200, jsonCards);
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }

        private ResponseObject AddPackage(string body, Dictionary<string, string> headers)
        {
            try
            {
                Console.WriteLine("Authorization header: " + headers["Authorization"]);

                // Authentication and admin validation
                var token = _authService.GetAuthToken(headers);
                
                _authService.EnsureAuthenticated(token, allowAdmin: true);

                // Call business logic in PackageService
                var cards = JsonSerializer.Deserialize<List<ICard>>(body, new JsonSerializerOptions
                {
                    Converters = { new CardJsonConverter() }
                });
                
                // if everything is fine add package
                _packageService.AddPackage(cards);

                return new ResponseObject(201, "Package successfully created.");
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }

    }
}
