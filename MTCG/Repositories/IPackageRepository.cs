using MTCG.Models.Card;

namespace MTCG.Repositories
{
    public interface IPackageRepository
    {
        List<ICard>? AcquirePackage(int userId);
        bool AddPackage(List<ICard> cards);
        bool CheckCardAlreadyExists(List<ICard> cards);
        void UpdatePackageOwnership(List<Guid> cardIds, int userId);
    }
}