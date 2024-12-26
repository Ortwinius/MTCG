using MTCG.Models.Card;
using MTCG.Models.Package;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Utilities.CustomExceptions;
using System;
using System.Collections.Generic;

namespace MTCG.BusinessLogic.Services
{
    public class PackageService
    {
        private static PackageService? _instance;
        private readonly PackageRepository _packageRepository;
        private readonly UserRepository _userRepository;

        private PackageService(PackageRepository packageRepository, UserRepository userRepository)
        {
            _packageRepository = packageRepository;
            _userRepository = userRepository;
        }

        public static PackageService GetInstance(PackageRepository packageRepository, UserRepository userRepository)
        {
            if (_instance == null)
            {
                _instance = new PackageService(packageRepository, userRepository);
            }
            return _instance;
        }

        public void AddPackage(List<ICard> cards)
        {
            if(!_packageRepository.AddPackage(cards))
            {
                throw new PackageConflictException();
            }
        }

        public List<ICard>? AcquirePackage(User user)
        {
            Console.WriteLine("[PackageService] Checking balance of user");
            if (user!.Coins < 5)
            {
                throw new NotEnoughCoinsException();
            }
            Console.WriteLine("[PackageService] Trying to acquire cards");
            var cards = _packageRepository.AcquirePackage(user.Username);

            if (cards == null)
            {
                throw new NoPackageAvailableException();
            }

            Console.WriteLine("[PackageService] Acquiring package successful - updating user balance = -5");
            // Only subtract coins if there is a package available
            user.Coins -= 5;
            _userRepository.UpdateUser(user);

            return cards;
        }
    }
}
