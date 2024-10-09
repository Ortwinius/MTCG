using MTCG.Controllers;
using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.BusinessLogic.Services;
using MTCG.BusinessLogic.Manager;

namespace MTCG
{
    class MTCGApp
    {
        static void Main(string[] args)
        {
            var userRepos = new UserRepository();
            var cardRepos = new CardRepository();
            var authService = AuthService.GetInstance(userRepos);
            var cardService = CardService.GetInstance(cardRepos, userRepos);
            var deckService = DeckService.GetInstance();
            var battleService = BattleService.GetInstance(deckService);
            var stackService = StackService.GetInstance(userRepos);

            var gameManager = new GameManager(authService, cardService, deckService, battleService, stackService);
            var serverController = new ServerController(gameManager);

            //serverController.Demo();

            try
            {
                gameManager.RegisterUser("Ortwinius", "safepassword123");
                gameManager.RegisterUser("Lyria", "anothersafepassword456");

                gameManager.ShowUserStack("Ortwinius");

                gameManager.LoginUser("Ortwinius", "safepassword123");
                gameManager.LoginUser("Lyria", "anothersafepassword456");

                // TODO: Add cards to stack first, then fix that cards need to be from stack
                gameManager.Debug_CreateDummyStack("Ortwinius");
                gameManager.Debug_CreateDummyStack("Lyria");

                string[] deckOrtwin = gameManager.Debug_CreateDummyDeck("Ortwinius");
                gameManager.ConfigureUserDeck("Ortwinius", deckOrtwin);

                string[] deckLyria = gameManager.Debug_CreateDummyDeck("Lyria");
                gameManager.ConfigureUserDeck("Lyria", deckLyria);

                gameManager.ShowUserStack("Ortwinius");
                gameManager.ShowUserStack("Lyria");

                gameManager.ShowUserDeck("Ortwinius");

                gameManager.JoinBattle("Ortwinius");
                gameManager.JoinBattle("Lyria");

                gameManager.LogoutUser("Ortwinius");                
                gameManager.LogoutUser("Lyria");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

    }

}