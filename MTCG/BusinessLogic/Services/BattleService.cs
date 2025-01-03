using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Utilities;
using MTCG.Utilities.Exceptions.CustomExceptions;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using System.Globalization;
using MTCG.Models.Users.DTOs;
using System.Transactions;
using MTCG.Repositories.Interfaces;
using System.Diagnostics;
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
        private readonly IUserRepository _userRepository;
        private readonly IDeckRepository _deckRepository;

        public BattleService(IDeckRepository deckRepository, IUserRepository userRepository)
        {
            _deckRepository = deckRepository;
            _userRepository = userRepository;
        }
        public static BattleService GetInstance(IDeckRepository deckRepository, IUserRepository userRepository)
        {
            if(_instance == null)
            {
                _instance = new BattleService(deckRepository, userRepository);
            }
            return _instance;
        }
        public static void ResetInstance() => _instance = null;
        /*
        returns BattleLog
        */
        public List<string> TryBattle(User lhs, User rhs)
        {
            var initialDeckLhs = GetDeck(lhs);
            var initialDeckRhs = GetDeck(rhs);

            var currentDeckLhs = new List<ICard>(initialDeckLhs);
            var currentDeckRhs = new List<ICard>(initialDeckRhs);

            List<string> battleLog = InitializeBattleLog(lhs, rhs);

            int roundCount = 0;

            while (roundCount < Constants.MaxBattleRounds && currentDeckLhs.Count > 0 && currentDeckRhs.Count > 0)
            {
                roundCount++;
                ExecuteBattleRound(lhs, rhs, currentDeckLhs, currentDeckRhs, battleLog, roundCount);
            }

            ProcessBattleResults(lhs, rhs, currentDeckLhs, currentDeckRhs, battleLog);
            ApplyCardTransfers(lhs, rhs, initialDeckLhs, initialDeckRhs, currentDeckLhs, currentDeckRhs);
            UpdateUserStats(lhs, rhs, currentDeckLhs.Count > currentDeckRhs.Count);

            return battleLog;
        }
        internal void ExecuteBattleRound(
            User lhs, User rhs,
            List<ICard> deckLhs, List<ICard> deckRhs,
            List<string> battleLog, int roundCount)
        {
            battleLog.Add($"Round {roundCount}:");

            var cardLhs = deckLhs.ElementAt(new Random().Next(deckLhs.Count));
            var cardRhs = deckRhs.ElementAt(new Random().Next(deckRhs.Count));

            // applying mandatory unique feature powerSurge
            int damageLhs = ApplyRandomPowerSurge(cardLhs, battleLog);
            damageLhs = CalculateDamage(cardLhs, cardRhs, battleLog);

            int damageRhs = ApplyRandomPowerSurge(cardRhs, battleLog);
            damageRhs = CalculateDamage(cardRhs, cardLhs, battleLog);

            battleLog.Add($"{lhs.Username}: {cardLhs.Name} [+{damageLhs}]");
            battleLog.Add($"{rhs.Username}: {cardRhs.Name} [+{damageRhs}]");

            if (damageLhs > damageRhs)
            {
                battleLog.Add($"{lhs.Username} wins this round! Card \"{cardRhs.Name}\" is taken.\n");
                deckRhs.Remove(cardRhs);
                deckLhs.Add(cardRhs);
            }
            else if (damageRhs > damageLhs)
            {
                battleLog.Add($"{rhs.Username} wins this round! Card \"{cardLhs.Name}\" is taken.\n");
                deckLhs.Remove(cardLhs);
                deckRhs.Add(cardLhs);
            }
            else
            {
                battleLog.Add("It's a draw! No cards are moved.\n");
            }
        }

        private void ProcessBattleResults(
            User lhs, User rhs,
            List<ICard> deckLhs, List<ICard> deckRhs,
            List<string> battleLog)
        {
            if (deckLhs.Count > 0 && deckRhs.Count == 0)
            {
                battleLog.Add($"\nFinal battle result: {lhs.Username} wins the battle!");
                _deckRepository.ResetDeck(rhs.UserId);
            }
            else if (deckRhs.Count > 0 && deckLhs.Count == 0)
            {
                battleLog.Add($"\nFinal battle result: {rhs.Username} wins the battle!");
                _deckRepository.ResetDeck(lhs.UserId);
            }
            else
            {
                battleLog.Add("Battle result: It's a draw! No Elo changes.");
            }
        }

        private void ApplyCardTransfers(
            User lhs, User rhs,
            List<ICard> initialDeckLhs, List<ICard> initialDeckRhs,
            List<ICard> finalDeckLhs, List<ICard> finalDeckRhs)
        {

            var cardsGainedByLhs = finalDeckLhs.Except(initialDeckLhs).ToList();
            var cardsGainedByRhs = finalDeckRhs.Except(initialDeckRhs).ToList();

            using var transaction = new TransactionScope();
            try
            {
                List<Guid> gainedGuids = new();

                if (cardsGainedByLhs.Any())
                {
                    gainedGuids = cardsGainedByLhs.Select(card => card.Id).ToList();
                    _deckRepository.TransferDeckCardsOwnership(gainedGuids, lhs.UserId);
                }

                if (cardsGainedByRhs.Any())
                {
                    gainedGuids = cardsGainedByRhs.Select(card => card.Id).ToList();
                    _deckRepository.TransferDeckCardsOwnership(gainedGuids, rhs.UserId);
                }

                transaction.Complete();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
                throw;
            }
        }

        internal void UpdateUserStats(
            User lhs, User rhs,
            bool isLhsWinner)
        {
            var statsLhs = _userRepository.GetUserStatsByToken(lhs.AuthToken!);
            var statsRhs = _userRepository.GetUserStatsByToken(rhs.AuthToken!);

            if (isLhsWinner)
            {
                statsLhs!.Wins++;
                statsRhs!.Losses++;
                statsLhs.Elo = CalculateElo(statsLhs.Elo, statsRhs.Elo, true);
                statsRhs.Elo = CalculateElo(statsRhs.Elo, statsLhs.Elo, false);
            }
            else
            {
                statsLhs!.Losses++;
                statsRhs!.Wins++;
                statsLhs.Elo = CalculateElo(statsLhs.Elo, statsRhs.Elo, false);
                statsRhs.Elo = CalculateElo(statsRhs.Elo, statsLhs.Elo, true);
            }

            _userRepository.UpdateUserStats(lhs.Username, statsLhs!);
            _userRepository.UpdateUserStats(rhs.Username, statsRhs!);
        }
        internal int CalculateElo(int eloA, int eloB, bool isAWinner)
        {
            //Console.WriteLine($"[BattleService] Old Elo for {(isAWinner ? "Winner" : "Loser")}: {eloA}");
            double expectedA = 1 / (1 + Math.Pow(10, (eloB - eloA) / 400.0));
            int k = 32;
            int newElo = eloA + (int)(k * (isAWinner ? 1 - expectedA : 0 - expectedA));
            if (newElo <= 0) newElo = 1; // Elo should never be negative

            //Console.WriteLine($"[BattleService] New Elo for {(isAWinner ? "Winner" : "Loser")}: {newElo}");
            return newElo;
        }
        internal int CalculateDamage(ICard attacker, ICard defender, List<string> battleLog)
        {
            int damage = attacker.Damage;

            if (attacker is MonsterCard monsterA && defender is MonsterCard monsterB)
            {
                if (monsterA.MonType == MonsterType.Goblin && monsterB.MonType == MonsterType.Dragon)
                {
                    battleLog.Add($"{monsterA.Name} (Goblin) is too afraid to attack {monsterB.Name} (Dragon). Damage is 0.");
                    return 0;
                }

                if (monsterA.MonType == MonsterType.Ork && monsterB.MonType == MonsterType.Wizzard)
                {
                    battleLog.Add($"{monsterB.Name} (Wizzard) controls {monsterA.Name} (Ork). Damage is 0.");
                    return 0;
                }

                if (monsterA.MonType == MonsterType.Dragon && monsterB.MonType == MonsterType.FireElf)
                {
                    battleLog.Add($"{monsterB.Name} (FireElf) evades {monsterA.Name} (Dragon)'s attack. Damage is 0.");
                    return 0;
                }

                return damage;
            }

            if (attacker is SpellCard spellA && defender is MonsterCard monsterOpponent)
            {
                if (spellA.ElemType == ElementType.Water && monsterOpponent.MonType == MonsterType.Knight)
                {
                    battleLog.Add($"{spellA.Name} (WaterSpell) drowns {monsterOpponent.Name} (Knight). Instant defeat.");
                    return int.MaxValue;
                }
                if (monsterOpponent.MonType == MonsterType.Kraken)
                {
                    battleLog.Add($"{monsterOpponent.Name} (Kraken) is immune to spells. Damage is 0.");
                    return 0;
                }
            }

            if (attacker is SpellCard || defender is SpellCard)
            {
                damage = ApplyEffectiveness(attacker, defender, battleLog);
            }

            return damage;
        }

        internal int ApplyEffectiveness(ICard attacker, ICard defender, List<string> battleLog)
        {
            int damage = attacker.Damage;

            if (attacker.ElemType == ElementType.Water && defender.ElemType == ElementType.Fire)
            {
                damage *= 2;
                battleLog.Add($"{attacker.Name} (Water) is super effective against {defender.Name} (Fire). Damage is doubled to {damage}.");
            }
            else if (attacker.ElemType == ElementType.Fire && defender.ElemType == ElementType.Water)
            {
                damage /= 2;
                battleLog.Add($"{attacker.Name} (Fire) is not effective against {defender.Name} (Water). Damage is halved to {damage}.");
            }
            else if (attacker.ElemType == ElementType.Fire && defender.ElemType == ElementType.Normal)
            {
                damage *= 2;
                battleLog.Add($"{attacker.Name} (Fire) is super effective against {defender.Name} (Normal). Damage is doubled to {damage}.");
            }
            else if (attacker.ElemType == ElementType.Normal && defender.ElemType == ElementType.Fire)
            {
                damage /= 2;
                battleLog.Add($"{attacker.Name} (Normal) is not effective against {defender.Name} (Fire). Damage is halved to {damage}.");
            }
            else if (attacker.ElemType == ElementType.Normal && defender.ElemType == ElementType.Water)
            {
                damage *= 2;
                battleLog.Add($"{attacker.Name} (Normal) is super effective against {defender.Name} (Water). Damage is doubled to {damage}.");
            }
            else if (attacker.ElemType == ElementType.Water && defender.ElemType == ElementType.Normal)
            {
                damage /= 2;
                battleLog.Add($"{attacker.Name} (Water) is not effective against {defender.Name} (Normal). Damage is halved to {damage}.");
            }

            return damage;
        }

        internal int ApplyRandomPowerSurge(ICard card, List<string> battleLog)
        {
            Random random = new Random();
            int damage = card.Damage;

            // 10% chance for a random power surge
            if (random.Next(1, 101) <= 10)
            {
                damage = (int)(damage * 1.5); // increases damage by 50%
                battleLog.Add($"Behold! {card.Name} experiences a POWER SURGE! Damage increases to {damage}.");
            }

            return damage;
        }
        private List<ICard> GetDeck(User user)
        {
            var deck = _deckRepository.GetDeckOfUser(user.UserId);
            if (deck == null || deck.Count == 0)
            {
                throw new DeckIsNullException();
            }
            return deck;
        }
        private List<string> InitializeBattleLog(User lhs, User rhs)
        {
            return new List<string>
            {
                "======",
                $"{lhs.Username} VS {rhs.Username}",
                "======",
                $"Starting battle between {lhs.Username} and {rhs.Username}"
            };
        }

    }
}
