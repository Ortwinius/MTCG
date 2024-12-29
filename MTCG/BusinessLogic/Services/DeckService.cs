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

        public ICard? DrawCardFromDeck(User user)
        {
            if (user == null || user.Deck == null || user.Deck.Count < 1)
            {
                Console.WriteLine("Error: User or deck is null or empty.");
                return null;
            }

            Random random = new Random();
            var keys = new List<string>(user.Deck.Keys);
            string randomKey = keys[random.Next(keys.Count)];
            ICard card = user.Deck[randomKey];

            user.Deck.Remove(randomKey);
            return card;
        }

        public void RemoveCardFromDeck(User? user, string cardKey)
        {
            if (user.Deck.Remove(cardKey))
            {
                Console.WriteLine($"Card with key {cardKey} successfully removed from {user.Username}'s deck");
            }
            else
            {
                Console.WriteLine("Error: Card {cardKey} could not be found.");
            }
        }

        public void ShowDeck(User user)
        {
            if (user == null || user.Deck == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            Console.WriteLine($"\nDeck of User: {user.Username}:");

            if (user.Deck.Count < 1)
            {
                Console.WriteLine("[Empty]");
                return;
            }

            int i = 1;
            foreach (var cardEntry in user.Deck)
            {
                string cardType = cardEntry.Value is MonsterCard ? "Monster" : "Spell";
                Console.WriteLine($"{i}. Key: {cardEntry.Key} -> {cardType}: \"{cardEntry.Value.Name}\" ({cardEntry.Value.ElemType}) {cardEntry.Value.Damage} Damage");
                i++;
            }
        }
        public void AddCardToDeck(User user, string cardKey, ICard card)
        {
            if (user.Deck.Count >= 4)
            {
                Console.WriteLine($"Cannot add card {card.Name}: Deck already has 4 cards.");
                return;
            }

            if (!user.Deck.ContainsKey(cardKey))
            {
                user.Deck.Add(cardKey, card);
            }
        }

        public bool ValidateDeck(User user)
        {
            return user.Deck != null && user.Deck.Count == 4;
        }

        public void TransferCard(User from, User to, string cardKey)
        {
            if (from.Deck.ContainsKey(cardKey))
            {
                ICard cardToTransfer = from.Deck[cardKey];
                RemoveCardFromDeck(from, cardKey);
                AddCardToDeck(to, cardKey, cardToTransfer);
            }
        }
    }
}
