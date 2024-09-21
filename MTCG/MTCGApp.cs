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
            MonsterCard monster = new MonsterCard("Terminator", Card.ElementType.Fire, 20);
            SpellCard waterWave = new SpellCard("Waterwave", Card.ElementType.Water, 15);

            List<ICard> exPackage = new List<ICard>();
            exPackage.Add(monster);
            exPackage.Add(waterWave);

            User userA = new User();
            AuthService.Register(userA, "Ortwinius", "safepassword123");
            Console.WriteLine($"Hashed password of {userA.Username}: {userA.HashedPassword}");
            foreach(var card in exPackage) 
            {
                userA.AddCardToStack(card);
            }
            userA.PrintStackInfo();
            AuthService.Logout(userA);
        }
    }
}
