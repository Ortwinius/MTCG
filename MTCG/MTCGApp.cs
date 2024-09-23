using MTCG.Models.Card;
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
            var cardS = CardService.GetInstance(cardRepos);

            // Registrieren und Einloggen von Benutzern
            authS.Register("Ortwinius", "safepassword123");
            authS.Register("Lyria", "anotherpassword456");
            authS.Login("Ortwinius", "safepassword123");
            authS.Login("Lyria", "anotherpassword456");

            // Anzahl der Karten, die jedem Benutzer zugewiesen werden sollen
            int cardCount = 5;

            // Zuweisen von Karten an Benutzer A
            for (int i = 0; i < cardCount; i++)
            {
                ICard randomCard = cardRepos.GetRandomCard(); // Verwendung der neuen Methode
                userRepos.GetUserByUsername("Ortwinius").AddCardToStack(randomCard);
            }

            // Zuweisen von Karten an Benutzer B
            for (int i = 0; i < cardCount; i++)
            {
                ICard randomCard = cardRepos.GetRandomCard(); // Verwendung der neuen Methode
                userRepos.GetUserByUsername("Lyria").AddCardToStack(randomCard);
            }

            // Testing: Zeige den Kartenstapel an
            userRepos.GetUserByUsername("Ortwinius").ShowStack();
            userRepos.GetUserByUsername("Lyria").ShowStack();

            // Ausloggen der Benutzer
            authS.Logout("Ortwinius");
            authS.Logout("Lyria");
        }

    }

}