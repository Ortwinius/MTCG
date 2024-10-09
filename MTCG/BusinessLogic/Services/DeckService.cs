using MTCG.Models.Card.Monster;
using MTCG.Models.Card;
using MTCG.Models.Users;

namespace MTCG.BusinessLogic.Services
{
    public class DeckService
    {
        private static DeckService _instance;

        private DeckService() { }

        public static DeckService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DeckService();
            }
            return _instance;
        }

        public void ShowDeck(User user)
        {
            if (user == null)
            {
                Console.WriteLine($"User {user.Username} not found.");
                return;
            }

            Console.WriteLine($"\nDeck of User: {user.Username}:");

            if (user.Deck.Count < 1)
            {
                Console.WriteLine("[Empty]");
                return;
            }
            int i = 1;
            foreach (var card in user.Deck)
            {
                string cardType = card is MonsterCard ? "Monster" : "Spell";
                Console.WriteLine($"{i}. -> {cardType}: \"{card.Name}\" ({card.ElemType}) {card.Damage} Damage");
                i++;
            }
        }

        public bool ConfigureDeck(User? user, List<ICard> cards)
        { 
            if(user == null)
            {
                Console.WriteLine($"Failed to configure deck for {user.Username} : user was null");
            }
            user.Deck.Clear();
            user.Deck.AddRange(cards);

            if(!ValidateDeck(user))
            {
                Console.WriteLine($"Deck for {user.Username} configured successfully.");
            }
            else
            {
                
            }
            return true;
        }
        // draw random card from deck
        public ICard DrawCardFromDeck(User user)
        {
            if (user.Deck.Count < 1) return null;

            Random random = new Random();
            int index = random.Next(0, user.Deck.Count);
            ICard card = user.Deck[index];
            user.Deck.RemoveAt(index);

            return card;
        }

        // remove card from deck if it is already in the deck
        public void RemoveCardFromDeck(User user, ICard card)
        {
            if (user.Deck.Contains(card))
            {
                user.Deck.Remove(card);
            }
        }
        // add card to deck
        public void AddCardToDeck(User user, ICard card)
        {
            user.Deck.Add(card);
        }
        public bool ValidateDeck(User user)
        {
            return user.Deck.Count == 4;
        }

        public void TransferCard(User from, User to, ICard toTransfer)
        {
            RemoveCardFromDeck(from, toTransfer);
            AddCardToDeck(to, toTransfer);
        }
    }
}
