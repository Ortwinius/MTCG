using MTCG.Models.TradingDeal;

namespace MTCG.Repositories
{
    public interface ITradingRepository
    {
        void ExecuteTrade(Guid cardId, int userId);

        void CreateTradingDeal(TradingDeal deal);
        void DeleteTradingDeal(Guid tradeId);

        List<TradingDeal> GetAllTradings();
        TradingDeal? GetTradingById(Guid tradeId);
    }
}