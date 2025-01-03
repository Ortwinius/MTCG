using MTCG.Models.TradingDeal;
using MTCG.Repositories.DL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories
{
    public class TradingRepository : ITradingRepository
    {
        public void ExecuteTrade(Guid tradeId, Guid offeredCardId)
        {
            throw new NotImplementedException();
        }

        public void CreateTradingDeal(TradingDeal deal)
        {
            throw new NotImplementedException();
        }

        public List<TradingDeal> GetAllTradings()
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "SELECT trade_id, card_to_trade, card_type, min_damage " +
                "FROM tradings",
                connection);

            using var reader = cmd.ExecuteReader();
            var tradingDeals = new List<TradingDeal>();

            while (reader.Read())
            {
                var deal = new TradingDeal
                {
                    Id = reader.GetGuid(reader.GetOrdinal("trade_id")),
                    CardToTrade = reader.GetGuid(reader.GetOrdinal("card_to_trade")),
                    Type = reader.GetString(reader.GetOrdinal("card_type")),
                    MinDamage = reader.GetInt32(reader.GetOrdinal("min_damage"))
                };

                tradingDeals.Add(deal);
            }

            return tradingDeals;
        }

        public TradingDeal? GetTradingById(Guid tradeId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "SELECT trade_id, card_to_trade, card_type, min_damage " +
                "FROM tradings " +
                "WHERE trade_id = @trade_id",
                connection);

            DataLayer.AddParameter(cmd, "trade_id", tradeId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new TradingDeal
                {
                    Id = reader.GetGuid(reader.GetOrdinal("trade_id")),
                    CardToTrade = reader.GetGuid(reader.GetOrdinal("card_to_trade")),
                    Type = reader.GetString(reader.GetOrdinal("card_type")),
                    MinDamage = reader.GetInt32(reader.GetOrdinal("min_damage"))
                };
            }
            return null;
        }

    }
}
