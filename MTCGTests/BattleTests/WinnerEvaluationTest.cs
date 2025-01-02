using MTCG.BusinessLogic.Services;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card;
using MTCG.Models.Users;
using MTCG.Repositories.Interfaces;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCGTests.BattleTests
{
    [TestFixture]
    public class WinnerEvaluationTest
    {
        private BattleService _battleService;
        private List<string> _battleLog;
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IDeckRepository _deckRepository = Substitute.For<IDeckRepository>();
        [SetUp]
        public void Setup()
        {
            _battleService = BattleService.GetInstance(_deckRepository, _userRepository);
            _battleLog = new();
        }
        [Test]
        public void ExecuteBattleRound_ShouldCorrectlyProcessWinner()
        {
            // Arrange
            var lhs = new User { Username = "PlayerA" };
            var rhs = new User { Username = "PlayerB" };

            var deckLhs = new List<ICard> { new MonsterCard { Name = "CardA", Damage = 50 } };
            var deckRhs = new List<ICard> { new MonsterCard { Name = "CardB", Damage = 30 } };
            var battleLog = new List<string>();

            // Act
            _battleService.ExecuteBattleRound(lhs, rhs, deckLhs, deckRhs, battleLog, 1);

            // Assert
            Assert.AreEqual(2, deckLhs.Count); // CardB should be added to PlayerA's deck
            Assert.AreEqual(0, deckRhs.Count); // PlayerB's deck should be empty
        }

    }
}
