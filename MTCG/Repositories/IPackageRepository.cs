using MTCG.Models.Card;

namespace MTCG.Repositories
{
    public interface IPackageRepository
    {
        List<ICard>? AcquirePackage(string username);
        bool AddPackage(List<ICard> cards);
        bool CheckAllCardsUnique(List<ICard> cards);
        void UpdatePackageOwnership(List<Guid> cardIds, string username);
        void UpdateSingleCardOwnership(Guid cardId, string username);
    }
}