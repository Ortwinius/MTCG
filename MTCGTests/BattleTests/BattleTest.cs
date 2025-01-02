using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLogic.Services;
using MTCG.Models.Card;
using MTCG.Models.Users;
using NSubstitute;
using NUnit.Framework;

namespace MTCGTests.BattleTests
{
    public class BattleTest
    {
        private BattleService? _battleService;
        
        [SetUp]
        public void Setup()
        {
            _battleService = BattleService.GetInstance();
        }
    }
}
