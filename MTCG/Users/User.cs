using MTCG.Card;
using MTCG.Services.SessionValidation;
using System;
using System.Net.WebSockets;

namespace MTCG.Users
{
    public class User : ILogin, IRegister
    {

        #region Variables

        private int _coins; // default 
        private int _elo = 100; // default 
        private const int _maxDeckSize = 4; // default
        private string _authToken = ""; //default => indicating no authToken
        private List<ICard> _stack;
        private List<ICard> _deck;


        // TODO : change username and password to required later? => doesnt work though
        public string Username { get; private set; }

        public string Password { get; private set; }
        public bool IsLoggedIn { get; private set; }
        public int Coins { get; private set; } = 20;
        public List<ICard> Stack { get; }
        public List<ICard> Deck { get; }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Stack = new List<ICard>();
            Deck = new List<ICard>();
            IsLoggedIn = false;
        }
        #endregion
        #region ILoginMethods
        public void Login(string username, string password)
        {
            //if username || password arent correct or username cant be found in database -> error
            // TODO

            // else => generate authToken and set logged in to true
            
            Console.WriteLine($"Logging in... Welcome {Username}!");
            _authToken = Guid.NewGuid().ToString(); // generating random token
            Console.WriteLine($"Auth-Token: {_authToken}");
            IsLoggedIn = true;
        }
        public void Logout()
        {
            Console.WriteLine($"Logging out...");
            _authToken = "";
            IsLoggedIn =false;

        }
        #endregion
        #region IRegisterMethods
        public void Register()
        {
            // if data is already stored in database -> error.
            
            // Else: Register user in database
        }
        public string HashPassword(string password)
        {
            // TODO
            return "hashed password";
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
