using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    
    /* Singleton Service for Card-related CRUD logic*/
    public class CardService
    {
        private static CardService _instance;
        private readonly CardRepository _cardRepository;
        private CardService(CardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        public static CardService GetInstance(CardRepository cardRepository)
        {
            if (_instance == null)
            {
                _instance = new CardService(cardRepository);
            }
            return _instance;
        }
        public ICard GetRandomCard()
        { 
            return _cardRepository.GetRandomCard();
        }

        public bool AddCardToStack(User user, ICard card)
        {
            if (!user.validateAction()) return false;

            if (card == null)
            {
                /*throw new ArgumentNullException(nameof(card), "Card to be added cannot be null");*/
                return false;
            }
            user.Stack.Add(card);
            return true;
        }
        public bool RemoveCardFromStack(User user, Guid cardId)
        {
            if (!user.validateAction()) return false;
            // search for card in Stack
            var cardToRemove = user.Stack.Find(card => card.Id == cardId);

            if (user.Stack.Count <= 0 || cardToRemove == null)
            {
                return false;
            }

            user.Stack.Remove(cardToRemove);
            return true;

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
            // TODO

            // 200 "OK"

            // set new deck 
            // foreach(var id in cardIds)
            // {
            // Deck.Add(GetCardById(cardIds[i]));
            // }

            return true;
        }

    }
}
