using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Singleton Service for Card-related CRUD logic*/
namespace MTCG.BusinessLogic.Services
{
    public class CardService
    {
        private static CardService _instance;
        private readonly CardRepository _cardRepository; // readonly to ensure Service always uses the same repos
        private readonly UserRepository _userRepository;
        private CardService(CardRepository cardRepository, UserRepository userRepository)
        {
            _cardRepository = cardRepository;
            _userRepository = userRepository;
        }
        public static CardService GetInstance(CardRepository cardRepository, UserRepository userRepository)
        {
            if (_instance == null)
            {
                _instance = new CardService(cardRepository, userRepository);
            }
            return _instance;
        }

        public List<ICard> ConvertCardIdsToCards(string[] cardIds)
        {
            var cards = new List<ICard>();
            foreach (var id in cardIds)
            {
                var card = GetCardById(Guid.Parse(id)); // Hol die Karte mit der ID
                if (card != null)
                {
                    cards.Add(card);
                }
                else
                {
                    Console.WriteLine($"Warning: Card with ID {id} was not found.");
                }
            }
            return cards;
        }

        public ICard? GetCardById(Guid id)
        {
            return _cardRepository.GetCardById(id);
        }
        public ICard? GetRandomCard()
        { 
            return _cardRepository.GetRandomCard();
        }
        public Guid? GetRandomCardId()
        {
            var card = GetRandomCard();
            
            return card != null ? card.Id : null;
        }

        // for debugging
        public ICard? GetRandomCardOfUser(User user)
        {
            //return _cardRepository.GetRandomCardOfUser(user);
            throw new NotImplementedException();
        }
    }
}
