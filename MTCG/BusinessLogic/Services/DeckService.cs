using MTCG.Models.Card.Monster;
using MTCG.Models.Card;
using MTCG.Models.Users;
using System.Security.Cryptography;
using MTCG.Utilities.CustomExceptions;
using MTCG.Repositories;

namespace MTCG.BusinessLogic.Services
{
    public class DeckService
    {
        private static DeckService? _instance;
        private readonly DeckRepository _deckRepository;

        private DeckService(DeckRepository deckRepository) 
        { 
            _deckRepository = deckRepository;
        }

        public static DeckService GetInstance(DeckRepository deckRepository)
        {
            if (_instance == null)
            {
                _instance = new DeckService(deckRepository);
            }
            return _instance;
        }
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

            //Console.WriteLine($"[DEBUG] User Cards: {string.Join(", ", userCards?.Select(c => c.Id) ?? new List<Guid>())}");
            //Console.WriteLine($"[DEBUG] Cards to Add: {string.Join(", ", cardIdsToAdd)}");

            // checks if the user owns the cards
            var cardIdsUserOwns = userCards?.Select(card => card.Id).ToList() ?? new List<Guid>();
            var invalidCardIds = cardIdsToAdd.Except(cardIdsUserOwns).ToList();

            if (invalidCardIds.Any())
            {
                Console.WriteLine($"[DEBUG] Invalid Card IDs: {string.Join(", ", invalidCardIds)}");
                throw new CardNotOwnedByUserException();
            }

            Console.WriteLine("[DeckService] -> Entering deckRepository");
            _deckRepository.ConfigureDeck(userId, cardIdsToAdd);
        }
    }
}
