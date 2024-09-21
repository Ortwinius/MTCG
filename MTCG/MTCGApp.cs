/*
Monster Trading Card Game - Main file
*/
using System;
using MTCG.Card;
using MTCG.Card.Monster;
using MTCG.Card.Spell;
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
/*            foreach (var card in exPackage) 
            {
                Console.WriteLine($"{card.Name} of type {card.Type}: {card.Damage} Damage");
            }*/


            User userA = new User("Ortwinius", "safepassword123");
            foreach(var card in exPackage) 
            {
                userA.AddCardToStack(card);
            }
            userA.PrintStackInfo();

        }
    }
}
