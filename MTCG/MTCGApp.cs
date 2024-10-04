using MTCG.Controllers;
using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Services;

namespace MTCG
{
    class MTCGApp
    {
        static void Main(string[] args)
        {
            ServerController serverController = new ServerController();
            serverController.Demo();

            CardRepository cardRepos = new CardRepository();
            UserRepository userRepos = new UserRepository();


            var authS = AuthService.GetInstance(userRepos);
            var cardS = CardService.GetInstance(cardRepos, userRepos); // TODO: Combine UserService as DeckService+StackService
            var deckS = DeckService.GetInstance(cardS);
            var battleS = BattleService.GetInstance(userRepos);
            try
            {
                // Registering
                authS.Register("Ortwinius", "safepassword123");
                authS.Register("Lyria", "anothersafepassword456");

                // Login validation
                authS.Login("Ortwinius", "safepassword123");
                authS.Login("Lyria", "anothersafepassword456");

                User ortwinius = userRepos.GetUserByUsername("Ortwinius");
                User lyria = userRepos.GetUserByUsername("Lyria");

                // Add 20 random cards to both users
                int cardCount = 20;
                for (int i = 0; i < cardCount; i++)
                {
                    cardS.AddCardToStack(ortwinius, cardS.GetRandomCard());
                    cardS.AddCardToStack(lyria, cardS.GetRandomCard());
                }

                string[] deckCardIds = new string[4];

                for(int i = 0; i < 4; i++)
                {
                    ICard card = cardS.GetRandomCardOfUser(ortwinius);
                    deckCardIds[i] = Convert.ToString(card.Id);
                }
                deckS.ConfigureDeck(ortwinius, deckCardIds);

                for (int i = 0; i < 4; i++)
                {
                    ICard card = cardS.GetRandomCardOfUser(lyria);
                    deckCardIds[i] = Convert.ToString(card.Id);
                }
                deckS.ConfigureDeck(lyria, deckCardIds);

                ortwinius.ShowStack();
                ortwinius.ShowDeck();
                lyria.ShowStack();
                lyria.ShowDeck();

                //battleS.AddPlayerToLobby("Ortwinius");
                
                authS.Logout("Ortwinius");                
                authS.Logout("Lyria");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

    }

}