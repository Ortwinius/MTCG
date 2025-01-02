using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Repositories.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MTCGTests
{
    public class PackageRepositoryTests
    {
        private IPackageRepository _mockPackageRepository;

        [SetUp]
        public void Setup()
        {
            _mockPackageRepository = Substitute.For<IPackageRepository>();
        }

        [Test]
        public void AddPackage_ShouldAddCardsToPackage()
        {
            // Arrange: Create a sample package with 5 cards
            var cards = new List<ICard>
            {
                new MonsterCard { Id = Guid.NewGuid(), Name = "Dragon", Damage = 50, ElemType = ElementType.Fire, MonType = MonsterType.Dragon },
                new SpellCard { Id = Guid.NewGuid(), Name = "FireSpell", Damage = 30, ElemType = ElementType.Fire, SpellType = SpellType.FireSpell },
                new MonsterCard { Id = Guid.NewGuid(), Name = "Knight", Damage = 40, ElemType = ElementType.Normal, MonType = MonsterType.Knight },
                new SpellCard { Id = Guid.NewGuid(), Name = "WaterSpell", Damage = 20, ElemType = ElementType.Water, SpellType = SpellType.WaterSpell },
                new MonsterCard { Id = Guid.NewGuid(), Name = "Goblin", Damage = 10, ElemType = ElementType.Normal, MonType = MonsterType.Goblin }
            };

            // Act: Call AddPackage on the mock repository
            _mockPackageRepository.AddPackage(cards);

            // Assert: Verify the AddPackage method was called once with the provided cards
            _mockPackageRepository.Received(1).AddPackage(cards);
        }
    }
}
