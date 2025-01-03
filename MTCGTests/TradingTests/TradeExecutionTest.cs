using MTCG.BusinessLogic.Services;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card;
using MTCG.Models.TradingDeal;
using MTCG.Models.Users;
using MTCG.Repositories.Interfaces;
using MTCG.Repositories;
using NSubstitute;
using MTCG.Utilities.Exceptions.CustomExceptions;

namespace MTCGTests.TradingTests
{
    [TestFixture]
    public class TradingServiceTests
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
        public void ValidateOfferedCard_ValidMonsterCard_ShouldReturnTrue()
        {
            
            var trade = new TradingDeal
            {
                Type = "monster",
                MinDamage = 50
            };
            var offeredCardId = Guid.NewGuid();
            var offeredCard = new MonsterCard(offeredCardId, "Dragon", ElementType.Fire, 60, MonsterType.Dragon);

            _cardRepository.GetCardById(offeredCardId).Returns(offeredCard);

            
            var isValid = _tradingService.ValidateOfferedCard(trade, offeredCardId);

            
            Assert.IsTrue(isValid);
        }

        [Test]
        public void ValidateOfferedCard_InvalidType_ShouldReturnFalse()
        {
            var trade = new TradingDeal
            {
                Type = "spell",
                MinDamage = 50
            };
            var offeredCardId = Guid.NewGuid();
            var offeredCard = new MonsterCard(offeredCardId, "Ork", ElementType.Normal, 60, MonsterType.Ork);

            _cardRepository.GetCardById(offeredCardId).Returns(offeredCard);

            var isValid = _tradingService.ValidateOfferedCard(trade, offeredCardId);

            Assert.IsFalse(isValid);
        }

        [Test]
        public void ValidateOfferedCard_InsufficientDamage_ShouldReturnFalse()
        {
            var trade = new TradingDeal
            {
                Type = "monster",
                MinDamage = 100
            };
            var offeredCardId = Guid.NewGuid();
            var offeredCard = new MonsterCard(offeredCardId, "Goblin", ElementType.Normal, 80, MonsterType.Goblin);

            _cardRepository.GetCardById(offeredCardId).Returns(offeredCard);

            var isValid = _tradingService.ValidateOfferedCard(trade, offeredCardId);

            Assert.IsFalse(isValid);
        }

        [Test]
        public void ExecuteTrade_ValidTrade_ShouldCompleteSuccessfully()
        {
            var tradeId = Guid.NewGuid();
            var trade = new TradingDeal
            {
                Id = tradeId,
                CardToTrade = Guid.NewGuid(),
                Type = "monster",
                MinDamage = 50
            };

            var offeredCardId = Guid.NewGuid();
            var user = new User { UserId = 1, Username = "TestUser" };
            var tradeCreatorId = 2;

            var offeredCard = new MonsterCard(offeredCardId, "Dragon", ElementType.Fire, 60, MonsterType.Dragon);
            var tradeCard = new MonsterCard(trade.CardToTrade, "Goblin", ElementType.Normal, 40, MonsterType.Goblin);

            _tradingRepository.GetTradingById(tradeId).Returns(trade);
            _cardRepository.GetUserCards(user.UserId).Returns(new List<ICard> { offeredCard });
            _cardRepository.GetOwnerOfCard(trade.CardToTrade).Returns(tradeCreatorId);
            _cardRepository.GetCardById(offeredCardId).Returns(offeredCard);

            
            _tradingService.ExecuteTrade(tradeId, offeredCardId, user);

            
            _tradingRepository.Received(1).DeleteTradingDeal(tradeId);
            _cardRepository.Received(1).UpdateCardOwnership(trade.CardToTrade, user.UserId);
            _cardRepository.Received(1).UpdateCardOwnership(offeredCardId, tradeCreatorId);
        }

        [Test]
        public void ExecuteTrade_TradeNotFound_ShouldThrowTradeNotFoundException()
        {
            
            var tradeId = Guid.NewGuid();
            var offeredCardId = Guid.NewGuid();
            var user = new User { UserId = 1, Username = "TestUser" };

            _tradingRepository.GetTradingById(tradeId).Returns((TradingDeal)null!);

            Assert.Throws<TradeNotFoundException>(() =>
                _tradingService.ExecuteTrade(tradeId, offeredCardId, user));
        }

        [Test]
        public void ExecuteTrade_TradingWithSelf_ShouldThrowInvalidTradeException()
        {
            
            var tradeId = Guid.NewGuid();
            var trade = new TradingDeal
            {
                Id = tradeId,
                CardToTrade = Guid.NewGuid(),
                Type = "monster",
                MinDamage = 50
            };

            var user = new User { UserId = 1, Username = "TestUser" };
            var offeredCardId = Guid.NewGuid();

            _tradingRepository.GetTradingById(tradeId).Returns(trade);
            _cardRepository.GetUserCards(user.UserId).Returns(new List<ICard>
            {
                new MonsterCard(trade.CardToTrade, "Goblin", ElementType.Normal, 40, MonsterType.Goblin)
            });

            var ex = Assert.Throws<InvalidTradeException>(() =>
                _tradingService.ExecuteTrade(tradeId, offeredCardId, user));
            Assert.AreEqual("You cannot trade with yourself.", ex.Message);
        }
    }
}
