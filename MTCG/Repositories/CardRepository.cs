﻿using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models.Users;

namespace MTCG.Repositories
{
    public class CardRepository
    {
        private readonly Dictionary<Guid, ICard> _cards = new();
        public CardRepository()
        {
            // Beispielhafte Karten
            //_cards.Add(new MonsterCard("FireElve", ElementType.Fire, 20));
            //_cards.Add(new MonsterCard("Kraken", ElementType.Water, 25));
            //_cards.Add(new SpellCard("Waterwave", ElementType.Water, 15));
            //_cards.Add(new MonsterCard("Gornak the Goblin", ElementType.Normal, 18));
            //_cards.Add(new MonsterCard("Thrag the Dragon", ElementType.Fire, 50));
            //_cards.Add(new MonsterCard("Morgath the Wizzard", ElementType.Fire, 40));
            //_cards.Add(new MonsterCard("Grulok the Orc", ElementType.Normal, 35));
            //_cards.Add(new MonsterCard("Sir Gallant the Knight", ElementType.Normal, 45));
            //_cards.Add(new MonsterCard("Ragnar the Kraken", ElementType.Water, 50));
            //_cards.Add(new MonsterCard("Elvina the FireElve", ElementType.Fire, 30));
            //_cards.Add(new SpellCard("Flame Burst", ElementType.Fire, 30));
            //_cards.Add(new SpellCard("Tsunami Wave", ElementType.Water, 35));
            //_cards.Add(new SpellCard("Rock Slam", ElementType.Normal, 25));
            //_cards.Add(new SpellCard("Inferno Blast", ElementType.Fire, 40));
            //_cards.Add(new SpellCard("Aqua Strike", ElementType.Water, 28));
            //_cards.Add(new SpellCard("Stone Crusher", ElementType.Normal, 20));
            //_cards.Add(new MonsterCard(MonsterType.Ork, ElementType.Normal, 35));
            //_cards.Add(new MonsterCard(MonsterType.WaterElf, ElementType.Water, 40));
            //_cards.Add(new MonsterCard(MonsterType.Dragon, ElementType.Fire, 60));
            //_cards.Add(new SpellCard(SpellType.WaterSpell, ElementType.Water, 15));
            _cards.Add(Guid.NewGuid(), new MonsterCard(MonsterType.Ork, ElementType.Fire, 48));
            _cards.Add(Guid.NewGuid(), new MonsterCard(MonsterType.WaterElf, ElementType.Water, 40));
            _cards.Add(Guid.NewGuid(), new MonsterCard(MonsterType.Dragon, ElementType.Fire, 70));

        }
        public List<ICard> GetAllCards() => _cards.Values.ToList();

        public ICard GetCardById(Guid id) => _cards[id];

        // TODO only possible if you are admin!!
        public void AddCard(ICard card)
        {
            _cards.Add(card.Id, card);
        }
        public ICard? GetRandomCard()
        {
            if (_cards.Count == 0)
            {
                Console.WriteLine("Card repository is empty");
                return null;
            }

            Random rand = new Random();
            int index = rand.Next(_cards.Count);
            return _cards.Values.ElementAt(index);
        }
        // for debugging
        public ICard GetRandomCardOfUser(User user)
        {
            List<ICard> userCards = user.Stack!.Values.ToList();
            Random rand = new Random();
            int index = rand.Next(userCards.Count);
            return userCards[index];
        }
    }
}
