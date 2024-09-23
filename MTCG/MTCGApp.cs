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
            CardRepository cardRepos = new CardRepository();
            UserRepository userRepos = new UserRepository();
            var authS = AuthService.GetInstance(userRepos);
            var cardS = CardService.GetInstance(cardRepos, userRepos); // TODO: own UserService

            try
            {
                // Registrieren und Einloggen von Benutzern
                authS.Register("Ortwinius", "safepassword123");
                authS.Register("Ortwinius", "safepassword123");
                authS.Login("Ortwinius", "safepassword123");

                // Anzahl der Karten, die jedem Benutzer zugewiesen werden sollen
                int cardCount = 20;

                var ortwinius = userRepos.GetUserByUsername("Ortwinius");

                // Zuweisen von Karten an Benutzer A
                for (int i = 0; i < cardCount; i++)
                {
                    cardS.AddCardToStack(ortwinius, cardS.GetRandomCard());
                }

                // Testing: Zeige den Kartenstapel an
                ortwinius.ShowStack();

                // Ausloggen der Benutzer
                authS.Logout("Ortwinius");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

    }

}