/*
Monster Trading Card Game - Main file
*/
using System;
using MTCG.Card;
using MTCG.Card.Monster;
using MTCG.Card.Spell;
using MTCG.Services;
using MTCG.Users;

namespace MTCG
{
    class MTCGApp
    {
        static void Main(string[] args)
        {
            MonsterCard Alchiax = new MonsterCard("FireElve", Card.ElementType.Fire, 20);
            MonsterCard Krakos = new MonsterCard("Kraken", Card.ElementType.Water, 25);
            SpellCard Sech = new SpellCard("Waterwave", Card.ElementType.Water, 15);

            List<ICard> exPackage = new List<ICard>();
            exPackage.Add(Alchiax);
            exPackage.Add(Krakos);
            exPackage.Add(Sech);

            User userA = new User();
            User userB = new User();
            // POST Register Http Request with username and password
            AuthService.Register(userA, "Ortwinius", "safepassword123");
            // POST Login Http Request with username and password
            AuthService.Login(userA, "Ortwinius", "safepassword123");
            // Debug info:
            Console.WriteLine($"Hashed password of {userA.Username}: {userA.HashedPassword}");
            Console.WriteLine($"AuthToken of {userA.Username}: {userA.AuthToken}");
            foreach(var card in exPackage) 
            {
                userA.AddCardToStack(card);
            }
            userA.ShowStackInfo();
            AuthService.Logout(userA);
        }
    }
}
