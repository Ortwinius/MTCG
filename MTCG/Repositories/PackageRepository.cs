using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Models.Users;
using MTCG.Utilities;
using MTCG.Utilities.CustomExceptions;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MTCG.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        public bool AddPackage(List<ICard> cards)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            if(!CheckCardAlreadyExists(cards))
            {
                return false;
            }
            using var transaction = connection.BeginTransaction();
            try
            {
                // Insert a new package and get its ID
                using var packageCommand = new NpgsqlCommand(
                    "INSERT INTO packages (price) " +
                    "VALUES (5) RETURNING package_id",
                    connection, transaction);

                int packageId = (int)packageCommand.ExecuteScalar()!;

                // Loop through cards and add them to the database
                foreach (var card in cards)
                {
                    // Insert card into the `cards` table
                    using var cardCommand = new NpgsqlCommand(
                        "INSERT INTO cards (card_id, name, type, element, damage) " +
                        "VALUES (@id, @name, @type, @element, @damage) " +
                        "ON CONFLICT (card_id) DO NOTHING",
                        connection, transaction);

                    // Add parameters for the card
                    DataLayer.AddParameter(cardCommand, "@id", card.Id);
                    DataLayer.AddParameter(cardCommand, "@damage", card.Damage);
                    DataLayer.AddParameter(cardCommand, "@element", card.ElemType.ToString());

                    if (card is MonsterCard monsterCard)
                    {
                        DataLayer.AddParameter(cardCommand, "@type", "MonsterCard");
                        DataLayer.AddParameter(cardCommand, "@name", monsterCard.MonType.ToString()!);
                    }
                    else if (card is SpellCard spellCard)
                    {
                        DataLayer.AddParameter(cardCommand, "@type", "SpellCard");
                        DataLayer.AddParameter(cardCommand, "@name", spellCard.SpellType?.ToString()!);
                    }

                    cardCommand.ExecuteNonQuery();

                    // Link the card to the package in the `package_cards` table
                    using var packageCardCommand = new NpgsqlCommand(
                        "INSERT INTO package_cards (package_id, card_id) " +
                        "VALUES (@package_id, @card_id)",
                        connection, transaction);

                    DataLayer.AddParameter(packageCardCommand, "@package_id", packageId);
                    DataLayer.AddParameter(packageCardCommand, "@card_id", card.Id);

                    packageCardCommand.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return false;
            }

            using var debugCommand = new NpgsqlCommand("SELECT package_id FROM packages ORDER BY package_id ASC", connection);
            using var reader = debugCommand.ExecuteReader();
            Console.WriteLine("[DEBUG] Current package order in the database:");
            while (reader.Read())
            {
                Console.WriteLine($"[DEBUG] Package ID: {reader["package_id"]}");
            }


            return true;
        }

        /*
         * AcquirePackage
        get one package of packages
        if no package is available return null
        if package is available return the cards in the package and
        delete the package from packages, package_cards from package_cards and NOT FROM cards
        instead update the owner of the cards
        */
        public List<ICard>? AcquirePackage(int userId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            //Console.WriteLine("Db transaction starting");
            using var transaction = connection.BeginTransaction();
            try
            {
                // Paket abrufen
                // "SELECT package_id FROM packages ORDER BY package_id ASC LIMIT 1"
                using var packageCommand = new NpgsqlCommand(
                    "SELECT package_id FROM packages ORDER BY package_id ASC LIMIT 1 FOR UPDATE",
                    connection, transaction);

                var packageId = packageCommand.ExecuteScalar() as int?;
                if (packageId == null)
                {
                    return null; // Keine Pakete verfügbar
                }

                // Karten aus dem Paket lesen
                using var cardCommand = new NpgsqlCommand(
                    "SELECT c.card_id, c.name, c.type, c.element, c.damage " +
                    "FROM cards c " +
                    "JOIN package_cards pc ON c.card_id = pc.card_id " +
                    "WHERE pc.package_id = @package_id",
                    connection, transaction);

                DataLayer.AddParameter(cardCommand, "@package_id", packageId);

                List<ICard> cards;
                using (var cardReader = cardCommand.ExecuteReader())
                {
                    cards = DataLayer.ParseCardsFromReader(cardReader);
                }

                // Kartenbesitz aktualisieren
                var cardIds = cards.Select(card => card.Id).ToList();

                // update ownership
                UpdatePackageOwnership(cardIds, userId);

                // Paket löschen
                using var deletePackageCommand = new NpgsqlCommand(
                    "DELETE FROM packages WHERE package_id = @package_id",
                    connection, transaction);

                DataLayer.AddParameter(deletePackageCommand, "@package_id", packageId);
                deletePackageCommand.ExecuteNonQuery();

                transaction.Commit();
                return cards;
            }
            catch
            {
                transaction.Rollback();
                return null;
            }
        }
        // checks if any card to insert is already in a package
        public bool CheckCardAlreadyExists(List<ICard> cards)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            foreach (var card in cards)
            {
                using var cmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM cards WHERE card_id = @card_id",
                    connection);

                DataLayer.AddParameter(cmd, "card_id", card.Id);

                if ((long)cmd.ExecuteScalar() != 0)
                {
                    return false;
                }
            }

            return true;
        }

        // updates ownership of 5 cards (in cards not packages !)
        public void UpdatePackageOwnership(List<Guid> cardIds, int userId)
        {
            //Console.WriteLine("Updating ownership");
            using var connection = DataLayer.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            { 
                // update ownership for all cards   
                foreach (var cardId in cardIds)
                {
                    var command = new NpgsqlCommand(
                        "UPDATE cards " +
                        "SET owned_by = @owned_by " +
                        "WHERE card_id = @card_id", connection, transaction);

                    DataLayer.AddParameter(command, "card_id", cardId);
                    DataLayer.AddParameter(command, "owned_by", userId);

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw new DbTransactionException($"Error while trying update card ownership for user {userId}. Rollback executed");
            }
        }

        public void UpdateSingleCardOwnership(Guid cardId, int userId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "UPDATE cards " +
                "SET owned_by = @owned_by" +
                "WHERE card_id = @card_id", connection);

            DataLayer.AddParameter(command, "card_id", cardId);
            DataLayer.AddParameter(command, "owned_by", userId);

            command.ExecuteNonQuery();
        }
    }
}
