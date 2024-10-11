using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MTCG.Utilities.Constants;
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
    // TODO : change constructor to private and add GetInstance
    public class BattleService
    {
        private static BattleService _instance;
        private readonly DeckService _deckService;

        //public const int MaxBattleRounds = 100;
        // player lobby list
        private List<User> playerLobby = new List<User>(); // put this in battle repository?
        public BattleService(DeckService deckService)
        {
            _deckService = deckService;
        }
        public static BattleService GetInstance(DeckService deckService)
        {
            if(_instance == null)
            {
                _instance = new BattleService(deckService);
            }
            return _instance;
        }
        public void TryBattle(User playerA)
        {
            // get second user from lobby

            User playerB = SearchForOpponent(playerA);
            if (playerB == null)
            {
                Console.WriteLine("Error: No opponent found.");
                return;
            }

            RemovePlayerFromLobby(playerA);
            RemovePlayerFromLobby(playerB);

            Console.WriteLine($"Battle between {playerA.Username} and {playerB.Username} started.");

            for (int round = 1; round <= MaxBattleRounds; round++)
            {
                Console.WriteLine($"Starting Round {round}...");

                // Ziehe die Karten aus den Decks beider Spieler
                ICard cardA = _deckService.DrawCardFromDeck(playerA);
                ICard cardB = _deckService.DrawCardFromDeck(playerB);

                if (cardA == null || cardB == null)
                {
                    Console.WriteLine("One of the players has run out of cards.");
                    // player won TODO
                    break;
                }

                // Führe die eigentliche Kampfrunde durch
                ExecuteBattleRound(cardA, cardB, playerA, playerB);
            }
        }
        private void ExecuteBattleRound(ICard cardA, ICard cardB, User playerA, User playerB)
        {
            // retrieve damage from cards
            int damageA = cardA.Damage;
            int damageB = cardB.Damage;

            // spell card vs monster
            // TODO

            // check absolute damage difference (to be changed for check if its monster vs monster)
            if (damageA > damageB)
            {
                Console.WriteLine($"{playerA.Username}'s card {cardA.Name} wins this round.");
                // Karte von playerB zu playerA transferieren
                _deckService.TransferCard(playerB, playerA, cardB);
            }
            else if (damageB > damageA)
            {
                Console.WriteLine($"{playerB.Username}'s card {cardB.Name} wins this round.");
                // Karte von playerA zu playerB transferieren
                _deckService.TransferCard(playerA, playerB, cardA);
            }
            else
            {
                Console.WriteLine("It's a draw. No cards are transferred.");
            }
        }

        public void AddPlayerToLobby(User user)
        {
            // if user is already in lobby, do not add
            if (playerLobby.Contains(user))
            {
                Console.WriteLine("Error: User was already in the lobby");
                return;
            }
            Console.WriteLine($"User {user.Username} added to battle lobby");
            playerLobby.Add(user);
        }
        public bool IsInLobby(User user)
        {
            return playerLobby.Contains(user);
        }
        public void RemovePlayerFromLobby(User user)
        {
            playerLobby.Remove(user);
        }
        // look for other player in lobby
        public User SearchForOpponent(User playerA)
        {
            foreach (var playerB in playerLobby)
            {
                if (playerB != playerA)
                {
                    Console.WriteLine($"Opponent found: {playerB.Username}");
                    return playerB;
                }
            }
            Console.WriteLine("Error: No opponent could be found");
            return null;
        }
    }
}
