/*
Monster Trading Card Game - Main file
*/
using System;
using MTCG.Card.Monster;
using MTCG.Card.Spell;
using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Services;

namespace MTCG
{
    class MTCGApp
    {
        static List<ICard> InitiateRandomCards()
        {
            List<ICard> cardPool = new List<ICard>();

            // Creating various Monster and Spell cards
            cardPool.Add(new MonsterCard("FireElve", ElementType.Fire, 20));
            cardPool.Add(new MonsterCard("Kraken", ElementType.Water, 25));
            cardPool.Add(new SpellCard("Waterwave", ElementType.Water, 15));
            cardPool.Add(new MonsterCard("Gornak the Goblin", ElementType.Normal, 18));
            cardPool.Add(new MonsterCard("Thrag the Dragon", ElementType.Fire, 50));
            cardPool.Add(new MonsterCard("Morgath the Wizzard", ElementType.Fire, 40));
            cardPool.Add(new MonsterCard("Grulok the Orc", ElementType.Normal, 35));
            cardPool.Add(new MonsterCard("Sir Gallant the Knight", ElementType.Normal, 45));
            cardPool.Add(new MonsterCard("Ragnar the Kraken", ElementType.Water, 50));
            cardPool.Add(new MonsterCard("Elvina the FireElve", ElementType.Fire, 30));
            cardPool.Add(new SpellCard("Flame Burst", ElementType.Fire, 30));
            cardPool.Add(new SpellCard("Tsunami Wave", ElementType.Water, 35));
            cardPool.Add(new SpellCard("Rock Slam", ElementType.Normal, 25));
            cardPool.Add(new SpellCard("Inferno Blast", ElementType.Fire, 40));
            cardPool.Add(new SpellCard("Aqua Strike", ElementType.Water, 28));
            cardPool.Add(new SpellCard("Stone Crusher", ElementType.Normal, 20));

            return cardPool;  // Return the pool of cards
        }
        static void Main(string[] args)
        {
            // Initiating a pool of (currently hardcoded) cards (later from the database)
            List<ICard> cardPool = InitiateRandomCards();

            // Users for testing - later users won't be constructed by that 
            User userA = new User();
            User userB = new User();

            // POST Register Http Request with username and password
            AuthService.Register(userA, "Ortwinius", "safepassword123");
            AuthService.Register(userB, "Lyria", "anotherpassword456");

            // POST Login Http Request with username and password
            AuthService.Login(userA, "Ortwinius", "safepassword123");
            //AuthService.Login(userB, "Lyria", "anotherpassword456");
            // Debug info
            Console.WriteLine($"Hashed password of {userA.Username}: {userA.HashedPassword}");
            Console.WriteLine($"Hashed password of {userB.Username}: {userB.HashedPassword}");
            Console.WriteLine($"AuthToken of {userA.Username}: {userA.AuthToken}");
            Console.WriteLine($"AuthToken of {userB.Username}: {userB.AuthToken}");

            // Assigning random cards from the card pool to each user
            Random rand = new Random();
            int cardCount = 5; // Number of random cards to give each user

            // Giving cards to userA
            for (int i = 0; i < cardCount; i++)
            {
                ICard randomCard = cardPool[rand.Next(cardPool.Count)];
                userA.AddCardToStack(randomCard); // boolean
            }

            // Giving cards to userB
            for (int i = 0; i < cardCount; i++)
            {
                ICard randomCard = cardPool[rand.Next(cardPool.Count)];
                userB.AddCardToStack(randomCard); // boolean
            }

            // Show stack info for both users
            userA.ShowStack();

            //Testing for removing card from userA
            //userA.RemoveCardFromStack(cardGuid xxx);

            userB.ShowStack();

            // Logging out userA
            AuthService.Logout(userA);
            AuthService.Logout(userB);
        }
    
    }
}
