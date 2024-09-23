using MTCG.Models.Card;
using MTCG.Services;
using System;
using System.Net.WebSockets;

namespace MTCG.Models.Users
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
        public bool AddCardToStack(ICard card)
        {
            if (!validateAction()) return false;

            if (card == null)
            {
                /*throw new ArgumentNullException(nameof(card), "Card to be added cannot be null");*/
                return false;
            }
            Stack.Add(card);
            return true;
        }
        public bool RemoveCardFromStack(Guid cardId)
        {
            if (!validateAction()) return false;
            // search for card in Stack
            var cardToRemove = Stack.Find(card => card.Id == cardId);

            if (Stack.Count <= 0 || cardToRemove == null)
            {
                return false;
            }

            Stack.Remove(cardToRemove);
            return true;

        }

        #endregion
        // Configure deck via four provided cards (array of strings)
        // Failed request doesnt change previously defined stack
        // uuid1, uuid2, uuid3, uuid4
        public bool ConfigureDeck(string[] cardIds)
        {
            if (!validateAction()) return false; // 401 Unauthorized

            if (Stack.Count < 1)
            {
                return false;
            }
            if (cardIds.Length != 4) // 400 Bad Request
            {
                Console.WriteLine("Can't add card to deck because the deck is already full (4/4). Consider replacing another card");
                ShowDeck();
                return false;
            }

            // if min 1 card is not in UserStack -> 403 Forbidden
            // TODO

            // 200 "OK"

            // set new deck 
            // foreach(var id in cardIds)
            // {
            // Deck.Add(GetCardById(cardIds[i]));
            // }

            return true;
        }

        #region Info

        // TODO Get User Info : Name, (opt) description : Http "GET users/{username}
        // 1. 200 OK
        // 2. 401 Unauthorized (only user or admin can do that)
        // 3. 404 Not found

        public void ShowStack() //Htpp "GET /cards"
        {
            if (!validateAction()) return;

            Console.WriteLine($"\nStack of User: {Username}:");

            if (Stack.Count < 1)
            {
                Console.WriteLine("[Empty]");
                return;
            }
            int i = 1;
            foreach (var card in Stack)
            {
                Console.WriteLine($"{i}. -> \"{card.Name}\" ({card.Type}) {card.Damage} Damage");
                i++;
            }
        }
        public void ShowDeck() //Htpp "GET /deck"
        {
            if (!validateAction()) return;

            Console.WriteLine($"\nBattledeck of User: {Username}:");

            if (Deck.Count < 1)
            {
                Console.WriteLine("[Empty]");
                return;
            }
            int i = 1;
            foreach (var card in Stack)
            {
                Console.WriteLine($"{i}. -> \"{card.Name}\" ({card.Type}) {card.Damage} Damage");
                i++;
            }
        }

        public void ShowUserInfo()
        {
            if (!validateAction()) return;

            // TODO
            // Shows Elo, Coins, Wins, Losses

        }

        #endregion
    }
}
