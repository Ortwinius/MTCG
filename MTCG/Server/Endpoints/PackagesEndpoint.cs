﻿using MTCG.Models.Card;
using MTCG.Models.ResponseObject;
using MTCG.BusinessLogic.Services;
using System.Text.Json;
using MTCG.Models.Package;
using MTCG.Utilities.CardJsonConverter;
using MTCG.Utilities.CustomExceptions;

namespace MTCG.Server.Endpoints
{
    public class PackagesEndpoint : IHttpEndpoint
    {
        private readonly PackageService _packageService;
        private readonly AuthService _authService;

        public PackagesEndpoint(PackageService packageService, AuthService authService)
        {
            _packageService = packageService;
            _authService = authService;
        }

        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string body)
        {
            switch (method)
            {
                case "POST" when path == "/packages":
                    return AddPackage(body, headers);
                case "POST" when path == "/transactions/packages":
                    return BuyPackage(headers);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }

        /*
        Gets first package which doesnt belong to a user 
        200: success
        401: unauthorized
        404: no package available
        403: Not enough money
        */
        private ResponseObject BuyPackage(Dictionary<string,string> headers)
        {
            try
            {
                if (!_authService.IsAuthenticated(headers["Authorization"]))
                {
                    throw new UnauthorizedException();
                }
                var user = _authService.GetUserByAuthtoken(headers["Authorization"]);

                var cards = _packageService.AcquirePackage(user!);

                // return cards by serializing them in json format
                return new ResponseObject(200, "User " + user!.Username + "acquired the package successfully");
            }
            catch(UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
            catch (NotEnoughCoinsException)
            {
                return new ResponseObject(403, "Not enough coins");
            }
            catch(NoPackageAvailableException)
            {
                return new ResponseObject(404, "No package available");
            }
        }

        private ResponseObject AddPackage(string body, Dictionary<string, string> headers)
        {
            try
            {
                Console.WriteLine("Authorization header: " + headers["Authorization"]);

                // Authentication and admin validation
                var token = headers["Authorization"];

                if (!_authService.IsAuthenticated(token))
                    throw new UnauthorizedException();

                if (!_authService.IsAdmin(token))
                    throw new NotAdminException();

                // Call business logic in PackageService
                var cards = JsonSerializer.Deserialize<List<ICard>>(body, new JsonSerializerOptions
                {
                    Converters = { new CardJsonConverter() }
                });

                if (cards == null || cards.Count != 5)
                    throw new InvalidPackageException("A package must contain exactly 5 cards.");
                
                // if everything is fine add package
                _packageService.AddPackage(cards);

                return new ResponseObject(201, "Package successfully created.");
            }
            catch (UnauthorizedAccessException)
            {
                return new ResponseObject(401, "Unauthorized.");
            }
            catch (NotAdminException)
            {
                return new ResponseObject(403, "Forbidden: Admin rights required.");
            }
            catch (InvalidPackageException ex)
            {
                return new ResponseObject(400, ex.Message);
            }
            catch (JsonException)
            {
                return new ResponseObject(400, "Invalid JSON format.");
            }
            catch (PackageConflictException)
            {
                return new ResponseObject(409, "One or more cards are already in a package.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new ResponseObject(500, "Internal server error.");
            }
        }

    }
}
