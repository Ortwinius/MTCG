using MTCG.Card;
using MTCG.Services;
using System;
using System.Net.WebSockets;

namespace MTCG.Users
{
    public class User
    {

        #region Setup

        private int _coins; // default 
        private int _elo = 100; // default 
        private const int _maxDeckSize = 4; // default
        private List<ICard> _stack;
        private List<ICard> _deck;

        // TODO : change username & hashedPassword & AuthToken & IsLoggedIn
        // setter to private <-> in conflict with authservice!
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string AuthToken { get; set; }
        public bool IsLoggedIn { get; set; }
        public int Coins { get; private set; } = 20;
        public List<ICard> Stack { get; }
        public List<ICard> Deck { get; }
        // base constructor
        public User() 
        {
            Username = "";
            HashedPassword = "";
            AuthToken = "";
            Stack = new List<ICard>();
            Deck = new List<ICard>();
            IsLoggedIn = false;
        }
        // TODO : make logic applicable that AuthService can be performed on this constructor?
        public User(string username, string hashedPassword)
        {
            Username = username;
            HashedPassword = hashedPassword;
            Stack = new List<ICard>();
            Deck = new List<ICard>();
            IsLoggedIn = false;
        }
        #endregion

        // validate each action by checking if user is logged in and authToken is valid
        public bool validateAction()
        {
            if (!IsLoggedIn || string.IsNullOrEmpty(AuthToken))
            {
                Console.WriteLine("You cannot perform this action due to missing permission. Are you logged in?");
                return false;
            }
            return true;
        }
        #region Stack
        public void AddCardToStack(ICard card)
        {
            if (!validateAction()) return;

            if (card == null)
            {
                throw new ArgumentNullException(nameof(card), "Card to be added cannot be null");
            }
            Stack.Add(card);
        }
        public void RemoveCardFromStack(ICard card)
        {
            if (!validateAction()) return;

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
            if (!validateAction()) return;

            if(card == null)
            {
                throw new ArgumentNullException(nameof(card), "Card to be added cannot be null");
            }
            if(!Stack.Contains(card))
            {
                throw new InvalidOperationException("Can't be added to deck because user doesn't possess this card in his stack");
            }
            if(Deck.Count >= 4)
            {
                Console.WriteLine("Can't add card to deck because the deck is already full (4/4). Consider replacing another card");
                ShowDeckInfo();
            }
            Deck.Add(card);
        }

        #region Info
        public void ShowStackInfo()
        {
            if (!validateAction()) return;

            Console.WriteLine($"Stack of User: {Username}:\n");

            if (Stack.Count < 1)
            {
                Console.WriteLine("[Empty]");
                return;
            }
            int i = 1;
            foreach(var card in Stack)
            {
                Console.WriteLine($"{i}. -> \"{card.Name}\" ({card.Type}) {card.Damage} Damage");
                i++;
            }
        }        
        public void ShowDeckInfo()
        {
            if (!validateAction()) return;

            Console.WriteLine($"Battledeck of User: {Username}:\n");

            if (Deck.Count < 1)
            {
                Console.WriteLine("[Empty]");
                return;
            }
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
