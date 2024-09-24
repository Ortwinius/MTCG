using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Singleton Service for Card-related CRUD logic*/
namespace MTCG.Services
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
        public ICard GetCardById(Guid id)
        {
            return _cardRepository.GetCardById(id);
        }
        public ICard GetRandomCard()
        { 
            return _cardRepository.GetRandomCard();
        }
        public Guid GetRandomCardId()
        {
            return GetRandomCard().Id;
        }

        public bool AddCardToStack(User user, ICard card)
        {
            if (!user.validateAction()) return false;

            if (card == null)
            {
                /*throw new ArgumentNullException(nameof(card), "Card to be added cannot be null");*/
                return false;
            }
            // TODO : fix access -> POST Request
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

            // TODO: fix access -> POST Request
            user.Stack.Remove(cardToRemove);
            return true;

        }
        // TODO: put in UserService
        public bool IsCardInUserStack(User user, Guid cardId)
        {
            return _userRepository.IsCardInUserStack(user, cardId);
        }

        // for debugging
        public ICard GetRandomCardOfUser(User user)
        {
            return _cardRepository.GetRandomCardOfUser(user);
        }
    }
}
