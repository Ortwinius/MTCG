using MTCG.Card;
using MTCG.Services;
using System;
using System.Net.WebSockets;

namespace MTCG.Users
{
    public class User
    {

        #region Variables

        private int _coins; // default 
        private int _elo = 100; // default 
        private const int _maxDeckSize = 4; // default
        private string _authToken; //default => indicating no authToken
        private List<ICard> _stack;
        private List<ICard> _deck;

        // TODO : change username & hashedPassword & IsLoggedIn
        // setter to private <-> in conflict with authservice!
        public string Username { get; set; }

        public string HashedPassword { get; set; }
        public bool IsLoggedIn { get; set; }
        public int Coins { get; private set; } = 20;
        public List<ICard> Stack { get; }
        public List<ICard> Deck { get; }
        // base constructor
        public User() 
        {
            Username = "";
            HashedPassword = "";
            _authToken = "";
            Stack = new List<ICard>();
            Deck = new List<ICard>();
            IsLoggedIn = false;
        }
        // TODO : change so Password doesnt save actual password but instead the hash
        public User(string username, string notHashedPassword)
        {
            Username = username;
            HashedPassword = notHashedPassword; // !!!
            Stack = new List<ICard>();
            Deck = new List<ICard>();
            IsLoggedIn = false;
        }
        #endregion

        #region Stack
        public void AddCardToStack(ICard card)
        {
            if(card == null)
            {
                throw new ArgumentNullException(nameof(card), "Card to be added cannot be null");
            }
            Stack.Add(card);
        }
        public void RemoveCardFromStack(ICard card)
        {
            if (Stack.Count <= 0)
            {
                throw new InvalidOperationException("User stack is empty");
            }
            if (card == null)
            {
                throw new ArgumentNullException("Card to be removed was null");
            }
            if (!Stack.Contains(card))
            {
                throw new InvalidOperationException("Can't be deleted because user does not possess the card to be removed");
            }

            Stack.Remove(card);
        }

        #endregion
        public void AddCardToDeck(ICard card)
        {
            if(card == null)
            {
                throw new ArgumentNullException(nameof(card), "Card to be added cannot be null");
            }
            Stack.Add(card);
        }

        #region Info
        public void PrintStackInfo()
        {
            Console.WriteLine($"Stack of User: {Username}:\n");
            int i = 1;
            foreach(var card in Stack)
            {
                Console.WriteLine($"{i}. -> \"{card.Name}\" ({card.Type}) {card.Damage} Damage");
                i++;
            }
        }
        #endregion
    }
}
