using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Utilities.CustomExceptions;
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
        private static CardService? _instance;
        private readonly CardRepository _cardRepository; 
        //private readonly UserRepository _userRepository;
        private CardService(CardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        public List<ICard>? GetUserCards(int userId)
        {
            var cards = _cardRepository.GetUserCards(userId);
            if (cards == null)
            {
                throw new UserStackIsEmptyException();
            }

            Console.WriteLine($"[DEBUG] Retrieving cards for user ID: {userId}");
            foreach (var card in cards)
            {
                Console.WriteLine($"[DEBUG] User Card ID: {card.Id}, Name: {card.Name}");
            }

            return cards;
        }
        public static CardService GetInstance(CardRepository cardRepository)
        {
            if (_instance == null)
            {
                _instance = new CardService(cardRepository);
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
    }
}
