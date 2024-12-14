using System.Text.Json;
using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using NSubstitute;
using NUnit.Framework;
using MTCG.Utilities.CardJsonConverter;

namespace MTCGTests
{

    [TestFixture]
    public class CardJsonConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public CardJsonConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new CardJsonConverter() },
                PropertyNameCaseInsensitive = true
            };
        }

        [Test]
        public void Deserialize_MonsterCard_Success()
        {
            // Arrange
            string json = @"
            {
                ""id"": ""845f0dc7-37d0-426e-994e-43fc3ac83c08"",
                ""name"": ""Goblin"",
                ""type"": ""MonsterCard"",
                ""element"": ""Normal"",
                ""damage"": 10,
                ""monType"": ""Goblin""
            }";

            // Act
            var card = JsonSerializer.Deserialize<ICard>(json, _options);

            // Assert
            Assert.IsNotNull(card);
            Assert.IsInstanceOf<MonsterCard>(card);
            var monsterCard = (MonsterCard)card;
            Assert.AreEqual("Goblin", monsterCard.Name);
            Assert.AreEqual(ElementType.Normal, monsterCard.ElemType);
            Assert.AreEqual(MonsterType.Goblin, monsterCard.MonType);
        }

        [Test]
        public void Deserialize_SpellCard_Success()
        {
            // Arrange
            string json = @"
            {
                ""id"": ""e85e3976-7c86-4d06-9a80-641c2019a79f"",
                ""name"": ""FireSpell"",
                ""type"": ""SpellCard"",
                ""element"": ""Fire"",
                ""damage"": 30,
                ""spellType"": ""FireSpell""
            }";

            // Act
            var card = JsonSerializer.Deserialize<ICard>(json, _options);

            // Assert
            Assert.IsNotNull(card);
            Assert.IsInstanceOf<SpellCard>(card);
            var spellCard = (SpellCard)card;
            Assert.AreEqual("FireSpell", spellCard.Name);
            Assert.AreEqual(ElementType.Fire, spellCard.ElemType);
            Assert.AreEqual(SpellType.FireSpell, spellCard.SpellType);
        }
    }
}
