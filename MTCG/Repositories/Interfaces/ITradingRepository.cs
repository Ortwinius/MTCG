using MTCG.Models.TradingDeal;

namespace MTCG.Repositories
{
    public interface ITradingRepository
    {
        void ExecuteTrade(Guid tradeId, Guid offeredCardId);

        void CreateTradingDeal(TradingDeal deal);

        List<TradingDeal> GetAllTradings();
        TradingDeal? GetTradingById(Guid tradeId);
    }
}