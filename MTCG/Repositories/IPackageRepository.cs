using MTCG.Models.Card;

namespace MTCG.Repositories
{
    public interface IPackageRepository
    {
        bool AcquirePackage();
        void AddPackage(List<ICard> cards);
    }
}