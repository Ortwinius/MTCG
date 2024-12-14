using MTCG.Models.Card;
using MTCG.Models.Package;
using MTCG.Repositories;
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

        public bool AddPackage(List<ICard> cards)
        {
            try
            {
                _packageRepository.AddPackage(cards);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while adding package: {ex.Message}");
                return false;
            }
        }

        public bool AcquirePackage(string username)
        {
            // Step 1: Check if user exists
            var user = _userRepository.GetUserByUsername(username);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return false;
            }

            // Step 2: Check if the user has enough coins
            if (user.Coins < 5)
            {
                Console.WriteLine("Not enough coins to acquire a package.");
                return false;
            }

            // Step 3: Retrieve the next available package
            //var package = _packageRepository.GetNextAvailablePackage();
            //if (package == null)
            //{
            //    Console.WriteLine("No packages available.");
            //    return false;
            //}

            try
            {
                // Step 4: Deduct 5 coins from the user
                user.Coins -= 5;
                _userRepository.UpdateUser(user);

                // Step 5: Assign the cards in the package to the user
                //_packageRepository.AssignPackageToUser(package, user);

                Console.WriteLine("Package successfully acquired.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while acquiring package: {ex.Message}");
                return false;
            }
        }
    }
}
