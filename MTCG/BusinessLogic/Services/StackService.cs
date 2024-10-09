using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLogic.Services
{
    // Service for stack related logic 
    public class StackService
    {
        private static StackService _instance;
        private readonly UserRepository _userRepository;

        // singleton instance
        public static StackService GetInstance(UserRepository userRepository)
        {
            if (_instance == null)
            {
                _instance = new StackService(userRepository);
            }
            return _instance;
        }
        //constructor
        private StackService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void ShowStack(User user) //Htpp "GET /cards"
        {
            if (user == null)
            {
                Console.WriteLine($"User {user.Username} not found.");
                return;
            }

            Console.WriteLine($"\nStack of User: {user.Username}:");

            if (user.Stack.Count < 1)
            {
                Console.WriteLine("[Empty]");
                return;
            }
            int i = 1;
            foreach (var card in user.Stack)
            {
                string cardType = card is MonsterCard ? "Monster" : "Spell";

                Console.WriteLine($"{i}. -> {cardType}: \"{card.Name}\" ({card.ElemType}) {card.Damage} Damage");
                i++;
            }
        }

        public bool AddCardToStack(User user, ICard card)
        {
            //if (!user.validateAction()) return false;

            if (card == null)
            {
                /*throw new ArgumentNullException(nameof(card), "Card to be added cannot be null");*/
                return false;
            }
            // TODO : fix access -> POST Request
            user.Stack.Add(card);
            return true;
        }
        public bool RemoveCardFromStack(User user, Guid cardId)
        {
            //if (!user.validateAction()) return false;
            // search for card in Stack
            var cardToRemove = user.Stack.Find(card => card.Id == cardId);

            if (user.Stack.Count <= 0 || cardToRemove == null)
            {
                return false;
            }

            // TODO: fix access -> POST Request
            user.Stack.Remove(cardToRemove);
            return true;

        }
        // TODO: put in UserService
        public bool IsCardInUserStack(User user, Guid cardId)
        {
            return _userRepository.IsCardInUserStack(user, cardId);
        }
    }
}
