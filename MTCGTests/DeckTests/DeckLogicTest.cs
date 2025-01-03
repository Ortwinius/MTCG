using MTCG.BusinessLogic.Services;
using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.Exceptions.CustomExceptions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCGTests.DeckTests
{
    [TestFixture]
    public class DeckLogicTest
    {
        private DeckService _deckService;
        private IDeckRepository _deckRepository;

        [SetUp]
        public void Setup()
        {
            DeckService.ResetInstance();
            _deckRepository = Substitute.For<IDeckRepository>();
            _deckService = DeckService.GetInstance(_deckRepository);
        }

        [Test]
        public void GetDeckOfUser_DeckIsNull_ShouldThrowDeckIsNullException()
        {
            // Arrange
            var userId = 1;
            _deckRepository.GetDeckOfUser(userId).Returns((List<ICard>?)null);

            // Act & Assert
            Assert.Throws<DeckIsNullException>(() => _deckService.GetDeckOfUser(userId));
        }

        [Test]
        public void ConfigureUserDeck_InvalidDeckSize_ShouldThrowInvalidDeckSizeException()
        {
            // Arrange
            var userId = 1;
            var userCards = new List<ICard>
            {
                Substitute.For<ICard>(),
                Substitute.For<ICard>(),
                Substitute.For<ICard>(),
                Substitute.For<ICard>(),
                Substitute.For<ICard>()
            };
            var cardIdsToAdd = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }; // Only 3 cards

            // Act & Assert
            Assert.Throws<InvalidDeckSizeException>(() => _deckService.ConfigureUserDeck(userId, userCards, cardIdsToAdd));
        }

        [Test]
        public void ConfigureUserDeck_DuplicateCardIds_ShouldThrowInvalidDeckSizeException()
        {
            // Arrange
            var userId = 1;
            var userCards = new List<ICard>
            {
                Substitute.For<ICard>(),
                Substitute.For<ICard>(),
                Substitute.For<ICard>(),
                Substitute.For<ICard>()
            };
            userCards[0].Id.Returns(Guid.NewGuid());
            userCards[1].Id.Returns(Guid.NewGuid());
            userCards[2].Id.Returns(Guid.NewGuid());
            userCards[3].Id.Returns(Guid.NewGuid());

            // Duplicate Card IDs in the list
            var duplicateCardId = userCards[0].Id;
            var cardIdsToAdd = new List<Guid> { duplicateCardId, duplicateCardId, Guid.NewGuid(), Guid.NewGuid() };

            // Act & Assert
            Assert.Throws<InvalidDeckSizeException>(() => _deckService.ConfigureUserDeck(userId, userCards, cardIdsToAdd));
        }
        [Test]
        public void ConfigureUserDeck_UserDoesNotOwnAllCards_ShouldThrowInvalidDeckSizeException()
        {
            // Arrange
            var userId = 1;
            var userCards = new List<ICard>
            {
                new MonsterCard { Id = Guid.NewGuid(), Name = "CardA" },
                new MonsterCard { Id = Guid.NewGuid(), Name = "CardB" },
                new MonsterCard { Id = Guid.NewGuid(), Name = "CardC" },
                new MonsterCard { Id = Guid.NewGuid(), Name = "CardD" }
            };

            // cardIdsToAdd includes a card the user doesn't own
            var cardIdsToAdd = new List<Guid>
            {
                userCards[0].Id, // Valid
                userCards[1].Id, // Valid
                Guid.NewGuid(),  // Invalid - not owned by the user
                userCards[2].Id  // Valid
            };

            // Act & Assert
            Assert.Throws<InvalidDeckSizeException>(() => _deckService.ConfigureUserDeck(userId, userCards, cardIdsToAdd));
        }
    }
}
