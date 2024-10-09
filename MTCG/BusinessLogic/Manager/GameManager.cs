using MTCG.BusinessLogic.Services;
using MTCG.Models.Users;
using MTCG.Models.Card;
using static MTCG.Utilities.Constants;

namespace MTCG.BusinessLogic.Manager
{
    public class GameManager
    {
        private readonly AuthService _authService;
        private readonly CardService _cardService;
        private readonly DeckService _deckService;
        private readonly BattleService _battleService;
        private readonly StackService _stackService;

        // Constructor with DI (Dependency Injection)
        public GameManager(AuthService authService, CardService cardService, DeckService deckService, BattleService battleService, StackService stackService)
        {
            _authService = authService;
            _cardService = cardService;
            _deckService = deckService;
            _battleService = battleService;
            _stackService = stackService;
        }

        public void RegisterUser(string username, string password)
        {
            string token = _authService.Register(username, password);
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"User {username} registered successfully.");
            }
        }

        public void LoginUser(string username, string password)
        {
            string token = _authService.Login(username, password);
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"User {username} logged in successfully.");
            }
        }
        public void LogoutUser(string username)
        {
            _authService.Logout(username);
        }

        public void ConfigureUserDeck(string username, string[] cardIds)
        {
            User user = _authService.GetUserByUsername(username);

            List<ICard> cards = _cardService.ConvertCardIdsToCards(cardIds);

            _deckService.ConfigureDeck(user, cards);
        }

        // add playerA to lobby and look for a playerB to intialize it
        public void JoinBattle(string username)
        {
            User playerA = _authService.GetUserByUsername(username);

            _battleService.AddPlayerToLobby(playerA);

            _battleService.TryBattle(playerA);
        }

        public void ShowUserStack(string username)
        {
            User user = _authService.GetUserByUsername(username);
            _stackService.ShowStack(user);
        }
        public void ShowUserDeck(string username)
        {
            User user = _authService.GetUserByUsername(username);
            _deckService.ShowDeck(user);
        }

        public string[] Debug_CreateDummyDeck(string username)
        {
            //mockup for cardIds, simply random cards from stack
            string[] cardIds = new string[4];
            User user = _authService.GetUserByUsername(username);

            for (int i = 0; i < 4; i++)
            {
                ICard card = _cardService.GetRandomCardOfUser(user);
                cardIds[i] = Convert.ToString(card.Id);
            }

            return cardIds;
        }
        public void Debug_CreateDummyStack(string username)
        {
            User user = _authService.GetUserByUsername(username);
            for (int i = 0; i < 20; i++)
            {
                var randomCard = _cardService.GetRandomCard();
                _stackService.AddCardToStack(user, randomCard);
            }
        }
    }

}