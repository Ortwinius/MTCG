using MTCG.BusinessLogic.Services;
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
    public class EloCalculationTest
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
        public void CalculateElo_ShouldUpdateCorrectlyForWinnerAndLoser()
        {
            
            int winnerElo = 1200;
            int loserElo = 1000;

            
            int newWinnerElo = _battleService.CalculateElo(winnerElo, loserElo, true);
            int newLoserElo = _battleService.CalculateElo(loserElo, winnerElo, false);

            
            Assert.That(newWinnerElo, Is.GreaterThan(winnerElo));
            Assert.That(newLoserElo, Is.LessThan(loserElo));
        }
    }
}
