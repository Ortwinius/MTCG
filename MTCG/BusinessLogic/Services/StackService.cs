using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Users;
using MTCG.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // constructor
        private StackService(UserRepository userRepository)
        {
            _userRepository = userRepository 
                ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public void ShowStack(User user) // Http "GET /cards"
        {
            if (user == null || user.Stack == null)
            {
                Console.WriteLine("User could not be found.");
                return;
            }

            Console.WriteLine($"\nStack of User: {user.Username}:");

            if (user.Stack.Count < 1)
            {
                Console.WriteLine("[Empty]");
                return;
            }

            int i = 1;
            foreach (var card in user.Stack.Values)
            {
                string cardType = card is MonsterCard ? "Monster" : "Spell";
                Console.WriteLine($"{i}. -> {cardType}: \"{card.Name}\" ({card.ElemType}) {card.Damage} Damage");
                i++;
            }
        }

        public bool AddCardToStack(User user, string cardKey, ICard card)
        {
            if (user == null || user.Stack == null || card == null)
            {
                Console.WriteLine("Error: User, stack, or card is null.");
                return false;
            }

            // TODO: fix access -> POST Request
            if (!user.Stack.ContainsKey(cardKey))
            {
                user.Stack.Add(cardKey, card);
                return true;
            }

            Console.WriteLine($"Card with key {cardKey} already exists in the stack.");
            return false;
        }

        public bool RemoveCardFromStack(User user, string cardKey)
        {
            if (user?.Stack == null)
            {
                return false;
            }

            if (!user.Stack.Remove(cardKey))
            {
                Console.WriteLine($"Error: Card with key {cardKey} not found.");
                return false;
            }

            return true;
        }

        // TODO: put in UserService
        public bool IsCardInUserStack(User user, Guid cardId)
        {
            if (user == null || user.Stack == null)
            {
                Console.WriteLine("Error: User or stack is null.");
                return false;
            }

            return _userRepository.IsCardInUserStack(user, cardId);
        }
    }
}
