using MTCG.BusinessLogic.Services;
using MTCG.Models.Card;
using MTCG.Models.ResponseObject;
using MTCG.Utilities.CardJsonConverter;
using MTCG.Utilities.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTCG.Server.Endpoints
{
    public class CardsEndpoint : IHttpEndpoint
    {
        private readonly CardService _cardService;
        private readonly AuthService _authService;
        public CardsEndpoint(CardService cardService, AuthService authService)
        {
            _cardService = cardService;
            _authService = authService;
        }

        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string body)
        {
            switch (method)
            {
                case "GET":
                    return GetUserCards(body, headers);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }
        // Gets all cards of a user 
        public ResponseObject GetUserCards(string body, Dictionary<string, string> headers)
        {
            try
            {
                if (!_authService.IsAuthenticated(headers["Authorization"]))
                {
                    throw new UnauthorizedException();
                }
                var userCards = _cardService.GetUserCards(body);

                var jsonUserCards = JsonSerializer.Serialize(userCards, new JsonSerializerOptions
                {
                    Converters = { new CardJsonConverter() },
                });

                return new ResponseObject(200, jsonUserCards);
            }
            catch(UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
            catch(UserStackIsEmptyException)
            {
                return new ResponseObject(204, "The request was fine, but the user doesn't have any cards.");
            }
        }
    }
}

