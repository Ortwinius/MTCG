using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Repositories.Interfaces;
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
        private readonly ICardRepository _cardRepository; 
        private CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        public static CardService GetInstance(ICardRepository cardRepository)
        {
            if (_instance == null)
            {
                _instance = new CardService(cardRepository);
            }
            return _instance;
        }
        public static void ResetInstance() => _instance = null;
        public List<ICard>? GetUserCards(int userId)
        {
            var cards = _cardRepository.GetUserCards(userId);
            if (cards == null)
            {
                throw new UserStackIsEmptyException();
            }

            return cards;
        }

        public ICard? GetCardById(Guid id)
        {
            return _cardRepository.GetCardById(id);
        }
    }
}
