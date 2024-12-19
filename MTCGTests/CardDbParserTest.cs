//using MTCG.Models.Card.Monster;
//using MTCG.Models.Card.Spell;
//using MTCG.Models.Card;
//using MTCG.Utilities;
//using NSubstitute;
//using System.Data;

//[TestFixture]
//public class CardDbParserTest
//{
//    [Test]
//    public void ParseCardsFromReader_ShouldReturnCorrectCards()
//    {
//        // Arrange
//        var mockReader = Substitute.For<IDataReader>();

//        // Simuliere Reihenfolge der Reads
//        mockReader.Read().Returns(
//            true,  // First card
//            true,  // Second card
//            false  // End of data
//        );

//        // Simuliere Daten für die erste Karte (MonsterCard)
//        mockReader.GetGuid(0).Returns(Guid.Parse("845f0dc7-37d0-426e-994e-43fc3ac83c08"));
//        mockReader.GetString(1).Returns("Goblin");
//        mockReader.GetString(2).Returns("MonsterCard");
//        mockReader.GetString(3).Returns("Normal");
//        mockReader.GetInt32(4).Returns(10);

//        // Simuliere Daten für die zweite Karte (SpellCard)
//        mockReader.GetGuid(0).Returns(Guid.Parse("e85e3976-7c86-4d06-9a80-641c2019a79f"));
//        mockReader.GetString(1).Returns("FireSpell");
//        mockReader.GetString(2).Returns("SpellCard");
//        mockReader.GetString(3).Returns("Fire");
//        mockReader.GetInt32(4).Returns(30);

//        // Act
//        var cards = Helpers.ParseCardsFromReader(mockReader);

//        // Assert
//        Assert.AreEqual(2, cards.Count);

//        // Überprüfe MonsterCard
//        var monsterCard = cards[0] as MonsterCard;
//        Assert.IsNotNull(monsterCard);
//        Assert.AreEqual(Guid.Parse("845f0dc7-37d0-426e-994e-43fc3ac83c08"), monsterCard.Id);
//        Assert.AreEqual(MonsterType.Goblin, monsterCard.MonType);
//        Assert.AreEqual(ElementType.Normal, monsterCard.ElemType);
//        Assert.AreEqual(10, monsterCard.Damage);

//        // Überprüfe SpellCard
//        var spellCard = cards[1] as SpellCard;
//        Assert.IsNotNull(spellCard);
//        Assert.AreEqual(Guid.Parse("e85e3976-7c86-4d06-9a80-641c2019a79f"), spellCard.Id);
//        Assert.AreEqual(SpellType.FireSpell, spellCard.SpellType);
//        Assert.AreEqual(ElementType.Fire, spellCard.ElemType);
//        Assert.AreEqual(30, spellCard.Damage);
//    }
//}
