using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLogic.Services;
using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Models.Users;
using MTCG.Repositories.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MTCGTests.BattleTests
{
    [TestFixture]
    public class DamageCalculationTest
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
        [TestCase(ElementType.Water, ElementType.Fire, 50, 100)]
        [TestCase(ElementType.Fire, ElementType.Normal, 50, 100)]
        [TestCase(ElementType.Normal, ElementType.Water, 50, 100)]
        public void ApplyEffectiveness_ShouldCalculateCorrectDamage(ElementType attackerType, ElementType defenderType, int baseDamage, int expectedDamage)
        {
            
            var attacker = Substitute.For<ICard>();
            attacker.Damage.Returns(baseDamage);
            attacker.ElemType.Returns(attackerType);
            attacker.Name.Returns("AttackerCard");

            var defender = Substitute.For<ICard>();
            defender.ElemType.Returns(defenderType);
            defender.Name.Returns("DefenderCard");

            
            var actualDamage = _battleService.ApplyEffectiveness(attacker, defender, _battleLog);

            
            Assert.AreEqual(expectedDamage, actualDamage);
        }

        [Test]
        public void CalculateDamage_MonsterVsMonster_ShouldReturnBaseDamage()
        {
            var goblin = new MonsterCard { Id = Guid.NewGuid(), Name = "Goblin", Damage = 50, ElemType = ElementType.Normal, MonType = MonsterType.Goblin };
            var ork = new MonsterCard { Id = Guid.NewGuid(), Name = "Ork", Damage = 60, ElemType = ElementType.Normal, MonType = MonsterType.Ork };

            var damage = _battleService.CalculateDamage(goblin, ork, _battleLog);

            Assert.AreEqual(50, damage);
        }
        [Test]
        public void CalculateDamage_OrkVsWizzard_ShouldReturnZeroDamage()
        {
            var ork = new MonsterCard { Id = Guid.NewGuid(), Name = "Ork", Damage = 60, ElemType = ElementType.Normal, MonType = MonsterType.Ork };
            var wizzard = new MonsterCard { Id = Guid.NewGuid(), Name = "Wizzard", Damage = 70, ElemType = ElementType.Normal, MonType = MonsterType.Wizzard };

            var damage = _battleService.CalculateDamage(ork, wizzard, _battleLog);

            Assert.AreEqual(0, damage);
        }
        [Test]
        public void CalculateDamage_GoblinVsDragon_ShouldReturnZeroDamage()
        {
            var card1 = new MonsterCard { Id = Guid.NewGuid(), Name = "Goblin", Damage = 10, ElemType = ElementType.Normal, MonType = MonsterType.Goblin };
            var card2 = new MonsterCard { Id = Guid.NewGuid(), Name = "Dragon", Damage = 50, ElemType = ElementType.Fire, MonType = MonsterType.Dragon };

            var damage = _battleService.CalculateDamage(card1, card2, _battleLog);

            Assert.AreEqual(0, damage);
        }
        [Test]
        public void CalculateDamage_WaterSpellVsFireMonster_ShouldDoubleDamage()
        {
            var waterSpell = new SpellCard { Id = Guid.NewGuid(), Name = "Water Spell", Damage = 40, ElemType = ElementType.Water };
            var fireMonster = new MonsterCard { Id = Guid.NewGuid(), Name = "Fire Monster", Damage = 50, ElemType = ElementType.Fire, MonType = MonsterType.Dragon };

            var damage = _battleService.CalculateDamage(waterSpell, fireMonster, _battleLog);

            Assert.AreEqual(80, damage);
        }
        [Test]
        public void CalculateDamage_SpellVsKraken_ShouldReturnZeroDamage()
        {
            var kraken = new MonsterCard { Id = Guid.NewGuid(), Name = "Kraken", Damage = 70, ElemType = ElementType.Water, MonType = MonsterType.Kraken };
            var fireSpell = new SpellCard { Id = Guid.NewGuid(), Name = "Fire Spell", Damage = 40, ElemType = ElementType.Fire };

            var damage = _battleService.CalculateDamage(fireSpell, kraken, _battleLog);

            Assert.AreEqual(0, damage);
        }
        [Test]
        public void CalculateDamage_WaterSpellVsKnight_ShouldReturnInstantKill()
        {
            var waterSpell = new SpellCard { Id = Guid.NewGuid(), Name = "Water Spell", Damage = 40, ElemType = ElementType.Water };
            var knight = new MonsterCard { Id = Guid.NewGuid(), Name = "Knight", Damage = 50, ElemType = ElementType.Normal, MonType = MonsterType.Knight };

            var damage = _battleService.CalculateDamage(waterSpell, knight, _battleLog);

            Assert.AreEqual(int.MaxValue, damage);
        }

        [Test]
        public void CalculateDamage_DragonVsFireElf_ShouldReturnZeroDamage()
        {
            var fireElf = new MonsterCard { Id = Guid.NewGuid(), Name = "Fire Elf", Damage = 30, ElemType = ElementType.Fire, MonType = MonsterType.FireElf };
            var dragon = new MonsterCard { Id = Guid.NewGuid(), Name = "Dragon", Damage = 70, ElemType = ElementType.Fire, MonType = MonsterType.Dragon };

            var damage = _battleService.CalculateDamage(dragon, fireElf, _battleLog);

            Assert.AreEqual(0, damage);
        }
    }
}
