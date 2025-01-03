using MTCG.BusinessLogic.Services;
using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Models.Users;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.Exceptions.CustomExceptions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MTCGTests.PackageTests
{
    [TestFixture]
    public class PackageTest
    {
        private PackageService _packageService;
        private IPackageRepository _packageRepository;
        private IUserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            PackageService.ResetInstance(); 
            _packageRepository = Substitute.For<IPackageRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _packageService = PackageService.GetInstance(_packageRepository, _userRepository);
        }

        [Test]
        public void AddPackage_ValidPackage_ShouldAddSuccessfully()
        {
            
            var cards = new List<ICard>
            {
                Substitute.For<ICard>(), Substitute.For<ICard>(),
                Substitute.For<ICard>(), Substitute.For<ICard>(),
                Substitute.For<ICard>()
            };

            _packageRepository.AddPackage(cards).Returns(true);

            Assert.DoesNotThrow(() => _packageService.AddPackage(cards));
            _packageRepository.Received(1).AddPackage(cards);
        }

        [Test]
        public void AddPackage_InvalidPackage_ShouldThrowInvalidPackageException()
        {
            
            var cards = new List<ICard> { Substitute.For<ICard>() }; // Less than 5 cards

            Assert.Throws<InvalidPackageException>(() => _packageService.AddPackage(cards));
        }

        [Test]
        public void AcquirePackage_UserHasEnoughCoins_ShouldAcquireSuccessfully()
        {
            
            var user = new User { UserId = 1, Username = "TestUser", Coins = 10 };
            var cards = new List<ICard>
            {
                Substitute.For<ICard>(), Substitute.For<ICard>(),
                Substitute.For<ICard>(), Substitute.For<ICard>(),
                Substitute.For<ICard>()
            };

            _packageRepository.AcquirePackage(user.UserId).Returns(cards);

            
            var acquiredCards = _packageService.AcquirePackage(user);

            
            Assert.AreEqual(cards, acquiredCards);
            Assert.AreEqual(5, user.Coins); // Coins reduced by 5
            _userRepository.Received(1).UpdateUser(user);
        }

        [Test]
        public void AcquirePackage_UserHasNotEnoughCoins_ShouldThrowNotEnoughCoinsException()
        {
            
            var user = new User { UserId = 1, Username = "TestUser", Coins = 2 };

            Assert.Throws<NotEnoughCoinsException>(() => _packageService.AcquirePackage(user));
            _packageRepository.DidNotReceive().AcquirePackage(Arg.Any<int>());
        }
    }
}
