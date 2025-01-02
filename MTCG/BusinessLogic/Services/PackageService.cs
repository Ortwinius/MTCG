using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.CustomExceptions;
using System;
using System.Collections.Generic;

namespace MTCG.BusinessLogic.Services
{
    public class PackageService
    {
        private static PackageService? _instance;
        private readonly IPackageRepository _packageRepository;
        private readonly IUserRepository _userRepository;

        private PackageService(IPackageRepository packageRepository, IUserRepository userRepository)
        {
            _packageRepository = packageRepository;
            _userRepository = userRepository;
        }

        public static PackageService GetInstance(IPackageRepository packageRepository, IUserRepository userRepository)
        {
            if (_instance == null)
            {
                _instance = new PackageService(packageRepository, userRepository);
            }
            return _instance;
        }

        public void AddPackage(List<ICard>? cards)
        {
            if (cards == null || cards.Count != 5)
                throw new InvalidPackageException("A package must contain exactly 5 cards.");

            Console.WriteLine("[DEBUG] Creating package with cards:");
            foreach (var card in cards)
            {
                Console.WriteLine($"[DEBUG] Card ID: {card.Id}, Name: {card.Name}");
            }

            if (!_packageRepository.AddPackage(cards))
            {
                throw new PackageConflictException();
            }
        }

        public List<ICard>? AcquirePackage(User user)
        {
            if (user!.Coins < 5)
            {
                throw new NotEnoughCoinsException();
            }
            var cards = _packageRepository.AcquirePackage(user.UserId);

            if (cards == null)
            {
                throw new NoPackageAvailableException();
            }

            Console.WriteLine($"[DEBUG] User {user.Username} is acquiring a package...");
            foreach (var card in cards)
            {
                Console.WriteLine($"[DEBUG] Acquired Card ID: {card.Id}, Name: {card.Name}");
            }

            user.Coins -= 5;
            _userRepository.UpdateUser(user);

            return cards;
        }
    }
}
