using MTCG.Models.Card;

namespace MTCG.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        List<ICard>? AcquirePackage(int userId);
        bool AddPackage(List<ICard> cards);
        void UpdatePackageOwnership(List<Guid> cardIds, int userId);
    }
}