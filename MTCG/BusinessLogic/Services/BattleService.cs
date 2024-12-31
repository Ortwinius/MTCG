using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Utilities;
using MTCG.Utilities.CustomExceptions;
/*
Singleton BattleService 
is a container for handling fights between cards
If userA attacks user B => get one random card of userA's and userB's userDeck
Those two cards fight each other depending on cardType(Monster/Spell), damage & effectivity
calculateHigherDamage() => checks who deals more damage and returns card
He who deals more damage wins and gets the card of the opponent - the opponent loses it
*/
namespace MTCG.BusinessLogic.Services
{
    public class BattleService
    {
        private static BattleService? _instance;
        private readonly DeckRepository _deckRepository;
        private readonly BattleRepository _battleRepository;

        public BattleService(BattleRepository battleRespository, DeckRepository deckRepository)
        {
            _battleRepository = battleRespository;
            _deckRepository = deckRepository;
        }
        public static BattleService GetInstance(BattleRepository battleRespository, DeckRepository deckRepository)
        {
            if(_instance == null)
            {
                _instance = new BattleService(battleRespository, deckRepository);
            }
            return _instance;
        }

        /*
        returns BattleLog
        */
        public string? StartBattle(User lhs, User rhs)
        {
            string? battleLog = "";
            // first get decks of users and throw error if they are not configured
            var deckLhs = _deckRepository.GetDeckOfUser(lhs.UserId);
            var deckRhs = _deckRepository.GetDeckOfUser(rhs.UserId);
            
            if(deckLhs == null || deckRhs == null)
            {
                throw new DeckIsNullException();
            }
            // draw card from deckLhs and from deckRhs
            var cardLhs = deckLhs.ElementAt(new Random().Next(deckLhs.Count));
            var cardRhs = deckRhs.ElementAt(new Random().Next(deckRhs.Count));

            // if one player has run out of cards, the other player wins
            if(CheckWinner(lhs, cardLhs, rhs, cardRhs, ref battleLog))
                return battleLog;

            throw new NotImplementedException();
            //ExecuteBattleRound(cardA, cardB, playerA, playerB);
        }
        private void ExecuteBattleRound(ICard cardA, ICard cardB, User playerA, User playerB)
        {
            //// retrieve damage from cards
            //int damageA = cardA.Damage;
            //int damageB = cardB.Damage;

            //// spell card vs monster
            //// TODO

            //// check absolute damage difference (to be changed for check if its monster vs monster)
            //if (damageA > damageB)
            //{
            //    Console.WriteLine($"{playerA.Username}'s card {cardA.Name} wins this round.");
            //    // Karte von playerB zu playerA transferieren
            //    _deckService.TransferCard(playerB, playerA, cardB.Id.ToString());
            //}
            //else if (damageB > damageA)
            //{
            //    Console.WriteLine($"{playerB.Username}'s card {cardB.Name} wins this round.");
            //    // Karte von playerA zu playerB transferieren
            //    _deckService.TransferCard(playerA, playerB, cardA.Id.ToString());
            //}
            //else
            //{
            //    Console.WriteLine("It's a draw. No cards are transferred.");
            //}
        }
        bool CheckWinner(User lhs, ICard cardLhs, User rhs, ICard cardRhs, ref string? battleLog)
        {
            if (cardLhs == null || cardRhs == null)
            {
                if (cardLhs == null)
                {
                    battleLog += $"\nOh no! User {lhs.Username} has run out of cards.\nThe winner is: {rhs.Username}";
                }
                else
                {
                    battleLog += $"\nOh no! User {rhs.Username} has run out of cards.\nThe winner is: {lhs.Username}";
                }
                return true;
            }
            return false;
        }
    }
}
