/*
Monster Trading Card Game - Main file
*/
using MTCG.Card;
using MTCG.Card.MonsterCard;
using MTCG.Card.SpellCard;
using System;

namespace MTCG
{
    class MTCGApp
    {
        static void Main(string[] args)
        {
            MonsterCard monster = new MonsterCard("Terminator", Card.ElementType.Fire, 20);
            SpellCard waterWave = new SpellCard("Waterwave", Card.ElementType.Water, 15);

            List<ICard> cards = new List<ICard>();
            cards.Add(monster);
            cards.Add(waterWave);
            foreach (var card in cards) 
            {
                Console.WriteLine($"{card.Name} of type {card.Type}: {card.Damage} Damage");
            }
        }
    }
}
