using MTCG.BusinessLogic.Services;
using MTCG.Models.Card;
using MTCG.Models.ResponseObject;
using MTCG.Utilities.CardJsonConverter;
using MTCG.Utilities.Exceptions.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTCG.Server.Endpoints
{
    public class DeckEndpoint : IHttpEndpoint
    {
        private readonly DeckService _deckService;
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly CardService _cardService;
        public DeckEndpoint(DeckService deckService, AuthService authService, UserService userService, CardService cardService) 
        {
            _deckService = deckService;
            _authService = authService;
            _cardService = cardService;
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
                case "GET":
                    return GetUserDeck(path, headers);
                case "PUT":
                    return ConfigureUserDeck(body!, headers);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }
        private ResponseObject GetUserDeck(string path, Dictionary<string,string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);
                var deckCards = _deckService.GetDeckOfUser(user!.UserId);

                // check if format should be plain
                if(path.Contains("?format=plain"))
                {
                    var plainDeck = deckCards!;
                    var plainDeckString = string.Join("\n", plainDeck.Select(c => $" - \"{c.Name}\" => Damage: {c.Damage}, Element: {c.ElemType}"));
                    return new ResponseObject(200, plainDeckString);
                }
                else
                {
                    var jsonDeck = JsonSerializer.Serialize(deckCards, new JsonSerializerOptions
                    {
                        Converters = { new CardJsonConverter() },
                        WriteIndented = true
                    });

                    return new ResponseObject(200, jsonDeck);
                }
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }
        private ResponseObject ConfigureUserDeck(string body, Dictionary<string,string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                var cardIdsToAdd = JsonSerializer.Deserialize<List<Guid>>(body);

                var userCards = _cardService.GetUserCards(user!.UserId);

                _deckService.ConfigureUserDeck(user!.UserId, userCards, cardIdsToAdd);

                return new ResponseObject(200, "Deck successfully configured.");
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }
    }
}
