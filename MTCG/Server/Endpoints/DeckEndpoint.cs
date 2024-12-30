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
        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string body)
        {
            switch (method)
            {
                case "GET":
                    return GetUserDeck(path, headers);
                case "PUT":
                    return ConfigureUserDeck(body, headers);
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
                    var plainDeck = deckCards!.Select(c => c.Name).ToList();
                    var plainDeckString = string.Join(", ", plainDeck);
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
            catch (UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
            catch(DeckIsNullException)
            {
                return new ResponseObject(204, "The request was fine, but the deck doesnt have any cards.");
            }
        }
        private ResponseObject ConfigureUserDeck(string body, Dictionary<string,string> headers)
        {
            try
            {
                Console.WriteLine("[DeckEndpoint] Authenticating and retrieving user object");
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                Console.WriteLine("[DeckEndpoint] Authentication successful -> Deserializing card ids");
                var cardIdsToAdd = JsonSerializer.Deserialize<List<Guid>>(body);

                Console.WriteLine("[DeckEndpoint] Deserialization successful - entering cardService to get user cards");
                var userCards = _cardService.GetUserCards(user!.UserId);

                Console.WriteLine("[DeckEndpoint] Entering deckService to configure deck -> [DeckService]");
                _deckService.ConfigureUserDeck(user!.UserId, userCards, cardIdsToAdd);

                return new ResponseObject(200, "Deck successfully configured.");
            }
            catch(UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
            catch(InvalidDeckSizeException)
            {
                return new ResponseObject(400, "The provided deck did not include the required amount of 4 unique cards.");
            }
            catch(CardNotOwnedByUserException)
            {
                return new ResponseObject(403, "At least one of the provided cards does not belong to the user or is not available.");
            }
        }
    }
}
