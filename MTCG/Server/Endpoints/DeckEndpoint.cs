using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Utilities.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server.Endpoints
{
    public class DeckEndpoint : IHttpEndpoint
    {
        private readonly DeckService _deckService;
        private readonly AuthService _authService;
        public DeckEndpoint(DeckService deckService, AuthService authService) 
        {
            _deckService = deckService;
            _authService = authService;
        }
        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string body)
        {
            switch (method)
            {
                case "GET":
                    return GetUserDeck(headers);
                case "PUT":
                    return UpdateUserDeck(body, headers);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }
        private ResponseObject GetUserDeck(Dictionary<string,string> headers)
        {
            try
            {
                var user = _authService.GetUserByValidToken(headers["Authorization"]);

                var deck = _deckService.GetDeckOfUser(user!.Username);
            }
            catch (UnauthorizedException)
            {
                return new ResponseObject(401, "Unauthorized");
            }
            catch(DeckIsNullException)
            {
                return new ResponseObject(204, "Request was fine, but the deck doesnt have any cards.");
            }
            return null;
        }
        private ResponseObject UpdateUserDeck(string body, Dictionary<string,string> headers)
        {
            return null;
        }
    }
}
