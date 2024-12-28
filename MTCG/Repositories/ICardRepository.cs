using MTCG.Models.Card;

namespace MTCG.Repositories
{
    public interface ICardRepository
    {
        ICard? GetCardById(Guid id);
        List<ICard>? GetUserCards(int userId);
    }
}