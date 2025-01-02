using MTCG.Models.Card;

namespace MTCG.Repositories.Interfaces
{
    public interface IDeckRepository
    {
        void ConfigureDeck(int userId, List<Guid> cardIds);
        List<ICard>? GetDeckOfUser(int userId);
        void ResetDeck(int userId);
        void TransferDeckCardsOwnership(List<Guid> cardIds, int userId);
    }
}