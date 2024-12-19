using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server.Endpoints
{
    public class CardsEndpoint : IHttpEndpoint
    {
        private readonly CardService _cardService;
        public CardsEndpoint(CardService cardService)
        {
            _cardService = cardService;
        }

        public ResponseObject HandleRequest(string method, string path, Dictionary<string, string> headers, string body)
        {
            switch (method)
            {
                case "GET":
                    return GetUserCards(body);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }
        // Gets all cards of a user 
        public ResponseObject GetUserCards(string body)
        {
            return new ResponseObject(200, "User cards shown.");
        }
    }
}

