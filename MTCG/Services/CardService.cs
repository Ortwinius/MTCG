using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    
    /* Singleton Service for Card-related logic */
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

        public bool ConfigureDeck(User user, string[] cardIds)
        {
            // Deck configuration logic
            return true;
        }

        public bool AddCardToUser(User user, ICard card)
        {
            return user.AddCardToStack(card);
        }

    }
}
