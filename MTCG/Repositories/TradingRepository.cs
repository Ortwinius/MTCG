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
        public void ExecuteTrade(Guid cardId, int userId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "UPDATE cards " +
                "SET owned_by = @user_id " +
                "WHERE card_id = @card_id", connection);

            DataLayer.AddParameter(cmd, "user_id", userId);
            DataLayer.AddParameter(cmd, "card_id", cardId);

            cmd.ExecuteNonQuery();
        }

        public void CreateTradingDeal(TradingDeal deal)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "INSERT INTO tradings (trade_id, card_to_trade, card_type, min_damage) " +
                "VALUES (@trade_id, @card_to_trade, @card_type, @min_damage)",
                connection);

            DataLayer.AddParameter(cmd, "trade_id", deal.Id);
            DataLayer.AddParameter(cmd, "card_to_trade", deal.CardToTrade);
            DataLayer.AddParameter(cmd, "card_type", deal.Type);
            DataLayer.AddParameter(cmd, "min_damage", deal.MinDamage);

            cmd.ExecuteNonQuery();
        }
        public void DeleteTradingDeal(Guid tradeId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "DELETE FROM tradings " +
                "WHERE trade_id = @trade_id",
                connection);

            DataLayer.AddParameter(cmd, "trade_id", tradeId);

            cmd.ExecuteNonQuery();
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
                    Id = reader.GetGuid(0),
                    CardToTrade = reader.GetGuid(1),
                    Type = reader.GetString(2),
                    MinDamage = reader.GetInt32(3)
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
                "SELECT * " +
                "FROM tradings " +
                "WHERE trade_id = @trade_id",
                connection);

            DataLayer.AddParameter(cmd, "trade_id", tradeId);

            try
            {
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var trade = new TradingDeal
                    {
                        Id = reader.GetGuid(0),
                        CardToTrade = reader.GetGuid(1),
                        Type = reader.IsDBNull(2) ? null : reader.GetString(2),
                        MinDamage = reader.GetInt32(3)
                    };
                    return trade;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving trade from the database.", ex);
            }

            return null;
        }


    }
}
