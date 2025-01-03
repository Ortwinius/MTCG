using MTCG.BusinessLogic.Services;
using MTCG.Repositories.Interfaces;
using MTCG.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using MTCG.Models.TradingDeal;
using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Models.Users;
using MTCG.Utilities.Exceptions.CustomExceptions;

namespace MTCGTests.TradingTests
{
    [TestFixture]
    public class TradeCreationTest
    {
        private TradingService _tradingService;
        private ITradingRepository _tradingRepository;
        private IDeckRepository _deckRepository;
        private ICardRepository _cardRepository;
        [SetUp]
        public void Setup()
        {
            TradingService.ResetInstance();
            _tradingRepository = Substitute.For<ITradingRepository>();
            _deckRepository = Substitute.For<IDeckRepository>();
            _cardRepository = Substitute.For<ICardRepository>();
            _tradingService = TradingService.GetInstance(_tradingRepository, _deckRepository, _cardRepository);
        }
        [Test]
        public void CreateTradingDeal_ValidDeal_ShouldCreateSuccessfully()
        {
            // Arrange
            var deal = new TradingDeal
            {
                Id = Guid.NewGuid(),
                CardToTrade = Guid.NewGuid(),
                Type = "monster",
                MinDamage = 10
            };
            var user = new User
            {
                UserId = 1,
                Username = "TestUser",
                Password = "TestPassword",
                Coins = 100
            };
            var userCards = new List<ICard>
            {

                new MonsterCard
                {
                    Id = deal.CardToTrade,
                    Name = "Goblin",
                    Damage = 20,
                    ElemType = ElementType.Normal
                },
                new MonsterCard
                {
                    Id = Guid.NewGuid(),
                    Name = "Dragon",
                    Damage = 50,
                    ElemType = ElementType.Fire
                }
            };
            _tradingRepository.GetAllTradings().Returns(new List<TradingDeal>());
            _cardRepository.GetUserCards(user.UserId).Returns(userCards);
            _deckRepository.GetDeckOfUser(user.UserId).Returns(new List<ICard>());
            // Act & Assert
            Assert.DoesNotThrow(() => _tradingService.CreateTradingDeal(deal, user));
            _tradingRepository.Received(1).CreateTradingDeal(deal);
        }
        [Test]
        public void CreateTradingDeal_UserHasNoCards_ShouldThrowInvalidTradeException()
        {
            // Arrange
            var deal = new TradingDeal
            {
                Id = Guid.NewGuid(),
                CardToTrade = Guid.NewGuid(),
                Type = "monster",
                MinDamage = 10
            };
            var user = new User
            {
                UserId = 1,
                Username = "TestUser",
                Password = "TestPassword",
                Coins = 100
            };

            _tradingRepository.GetAllTradings().Returns(new List<TradingDeal>());
            _cardRepository.GetUserCards(user.UserId).Returns(new List<ICard>()); // No cards
            _deckRepository.GetDeckOfUser(user.UserId).Returns(new List<ICard>());

            // Act & Assert
            var ex = Assert.Throws<InvalidTradeException>(() => _tradingService.CreateTradingDeal(deal, user));
            Assert.AreEqual("User doesnt have any cards to trade.", ex.Message);
            _tradingRepository.DidNotReceive().CreateTradingDeal(Arg.Any<TradingDeal>());
        }

        [Test]
        public void CreateTradingDeal_CardDoesNotBelongToUser_ShouldThrowInvalidTradeException()
        {
            // Arrange
            var deal = new TradingDeal
            {
                Id = Guid.NewGuid(),
                CardToTrade = Guid.NewGuid(),
                Type = "monster",
                MinDamage = 10
            };
            var user = new User
            {
                UserId = 1,
                Username = "TestUser",
                Password = "TestPassword",
                Coins = 100
            };
            var userCards = new List<ICard>
            {
                new MonsterCard
                {
                    Id = Guid.NewGuid(), // Different ID
                    Name = "Goblin",
                    Damage = 20,
                    ElemType = ElementType.Normal
                },
                new MonsterCard
                {
                    Id = Guid.NewGuid(),
                    Name = "Dragon",
                    Damage = 50,
                    ElemType = ElementType.Fire
                }
            };

            _tradingRepository.GetAllTradings().Returns(new List<TradingDeal>());
            _cardRepository.GetUserCards(user.UserId).Returns(userCards);
            _deckRepository.GetDeckOfUser(user.UserId).Returns(new List<ICard>());

            // Act & Assert
            var ex = Assert.Throws<InvalidTradeException>(() => _tradingService.CreateTradingDeal(deal, user));
            Assert.AreEqual("Card does not belong to the user.", ex.Message);
            _tradingRepository.DidNotReceive().CreateTradingDeal(Arg.Any<TradingDeal>());
        }
    }
}
