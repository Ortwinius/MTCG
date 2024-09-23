using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Singleton Service for Deck-related CRUD logic*/
namespace MTCG.Services
{
    public class DeckService
    {
        private static DeckService _instance;
        private readonly CardService _cardService; // instance of cardService is used for basic card operations

        private DeckService(CardService cardService)
        {
            _cardService = cardService;
        }
        public static DeckService GetInstance(CardService cardService)
        {
            if (cardService != null)
            {
                _instance = new DeckService(cardService);
            }
            return _instance;
        }

        public void ClearDeck(User user)
        {        
            // TODO: fix access 
            user.Stack.Clear();
        }
        // Configure deck via four provided cards (array of strings)
        // Failed request doesnt change previously defined stack
        // uuid1, uuid2, uuid3, uuid4
        public bool ConfigureDeck(User user, string[] cardIds)
        {
            if (!user.validateAction()) return false; // 401 Unauthorized

            if (user.Stack.Count < 1)
            {
                return false;
            }
            if (cardIds.Length != 4) // 400 Bad Request
            {
                Console.WriteLine("Error: Can't add card to deck because the deck is already full (4/4). Consider replacing another card");
                user.ShowDeck();
                return false;
            }

            // if min 1 card is not in UserStack -> 403 Forbidden
            // Prüfe, ob alle Karten in dem User-Stack sind
            foreach (var id in cardIds)
            {
                if (_cardService.IsCardInUserStack(user, Guid.Parse(id)) != true)
                {
                    Console.WriteLine($"Error: Card with ID {id} is not in the user's stack.");
                    return false; 
                }
            }

            // 200 "OK" -> set new deck 
            ClearDeck(user);

            foreach (var id in cardIds)
            {
                var card = _cardService.GetCardById(Guid.Parse(id));
                _cardService.AddCardToStack(user, card);
            }

            return true;
        }

    }
}
