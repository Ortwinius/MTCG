using MTCG.Models.Card;
using MTCG.Repositories.DL;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities;
using MTCG.Utilities.Exceptions.CustomExceptions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories
{
    public class DeckRepository : IDeckRepository
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
        // used for a loser to reset his deck
        public void ResetDeck(int userId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            try
            {
                var deleteCmd = new NpgsqlCommand(
                    "DELETE FROM deck_cards WHERE deck_id = @deck_id",
                    connection);
                DataLayer.AddParameter(deleteCmd, "deck_id", userId);
                deleteCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting deck: {ex.Message}");
                throw;
            }
        }
        /*
        Used so that cards of loser get transferred to winner
        */
        public void TransferDeckCardsOwnership(List<Guid> cardIds, int userId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            try
            {
                // update ownership for all cards   
                foreach (var cardId in cardIds)
                {
                    var command = new NpgsqlCommand(
                        "UPDATE cards " +
                        "SET owned_by = @owned_by " +
                        "WHERE card_id = @card_id", connection);

                    DataLayer.AddParameter(command, "card_id", cardId);
                    DataLayer.AddParameter(command, "owned_by", userId);

                    command.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                throw new DbTransactionException($"Error while trying update card ownership for user {userId}: {ex.Message}\n Rollback executed");
            }
        }
    }
}
