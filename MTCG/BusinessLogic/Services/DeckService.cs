using MTCG.Models.Card.Monster;
using MTCG.Models.Card;
using MTCG.Models.Users;
using System.Security.Cryptography;
using MTCG.Utilities.CustomExceptions;
using MTCG.Repositories;
using MTCG.Repositories.Interfaces;

namespace MTCG.BusinessLogic.Services
{
    public class DeckService
    {
        private static DeckService? _instance;
        private readonly IDeckRepository _deckRepository;

        private DeckService(IDeckRepository deckRepository) 
        { 
            _deckRepository = deckRepository;
        }

        public static DeckService GetInstance(IDeckRepository deckRepository)
        {
            if (_instance == null)
            {
                _instance = new DeckService(deckRepository);
            }
            return _instance;
        }
        public static void ResetInstance() => _instance = null;
        public List<ICard>? GetDeckOfUser(int userId)
        {
            var cards = _deckRepository.GetDeckOfUser(userId);

            if (cards == null)
            {
                throw new DeckIsNullException();
            }

            return cards;
        }

        public void ConfigureUserDeck(int userId, List<ICard>? userCards, List<Guid>? cardIdsToAdd)
        {
            if (cardIdsToAdd == null || cardIdsToAdd.Count != 4)
            {
                throw new InvalidDeckSizeException();
            }

            if(cardIdsToAdd.Distinct().Count() != cardIdsToAdd.Count)
            {
                throw new InvalidDeckSizeException();
            }

            // checks if the user owns the cards
            var cardIdsUserOwns = userCards?.Select(card => card.Id).ToList() ?? new List<Guid>();
            var invalidCardIds = cardIdsToAdd.Except(cardIdsUserOwns).ToList();

            if (invalidCardIds.Count > 0)
            {
                throw new InvalidDeckSizeException();
            }

            _deckRepository.ConfigureDeck(userId, cardIdsToAdd);
        }
    }
}
