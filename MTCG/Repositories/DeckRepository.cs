using MTCG.Models.Card;
using MTCG.Utilities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories
{
    public class DeckRepository
    {
        // Configure Deck
        public List<ICard>? GetDeckOfUser(int userId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "SELECT c.card_id, c.name, c.type, c.element, c.damage " +
                "FROM cards c " +
                "JOIN deck_cards dc ON c.card_id = dc.card_id " + 
                "WHERE dc.deck_id = @deck_id", connection);       

            DataLayer.AddParameter(cmd, "deck_id", userId);

            using var reader = cmd.ExecuteReader();
            return DataLayer.ParseCardsFromReader(reader);
        }

        public void ConfigureDeck(int userId, List<Guid> cardIds)
        {
            Console.WriteLine("[Deck Repository] Trying to enter deck repos and configure deck");
            using var connection = DataLayer.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Überprüfe, ob ein Deck für den Benutzer existiert
                var checkDeckCmd = new NpgsqlCommand(
                    "INSERT INTO decks (deck_id) VALUES (@deck_id) " +
                    "ON CONFLICT (deck_id) DO NOTHING",
                    connection, transaction);
                DataLayer.AddParameter(checkDeckCmd, "deck_id", userId);
                checkDeckCmd.ExecuteNonQuery();

                // Entferne bestehende Deck-Konfiguration
                var deleteCmd = new NpgsqlCommand(
                    "DELETE FROM deck_cards WHERE deck_id = @deck_id",
                    connection, transaction);
                DataLayer.AddParameter(deleteCmd, "deck_id", userId);
                deleteCmd.ExecuteNonQuery();

                // Füge neue Karten hinzu
                foreach (var cardId in cardIds)
                {
                    var insertCmd = new NpgsqlCommand(
                        "INSERT INTO deck_cards (deck_id, card_id) VALUES (@deck_id, @card_id)",
                        connection, transaction);
                    DataLayer.AddParameter(insertCmd, "deck_id", userId);
                    DataLayer.AddParameter(insertCmd, "card_id", cardId);
                    insertCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error configuring deck: {ex.Message}");
                throw;
            }
        }
    }
}
