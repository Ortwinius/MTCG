﻿using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Repositories.DL;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.Exceptions.CustomExceptions;
using Npgsql;

namespace MTCG.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        public bool AddPackage(List<ICard> cards)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            if(!CheckPackageAlreadyExists(cards))
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
            catch(Exception ex)
            {
                transaction.Rollback();
                throw new DbTransactionException($"Error while trying to add package: {ex.Message}\n Rollback executed");
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

            using var transaction = connection.BeginTransaction();
            try
            {
                using var packageCommand = new NpgsqlCommand(
                    "SELECT package_id " +
                    "FROM packages " +
                    "ORDER BY package_id ASC " +
                    "LIMIT 1 FOR UPDATE",
                    connection, transaction);

                var packageId = packageCommand.ExecuteScalar() as int?;
                if (packageId == null)
                {
                    return null; // no package available
                }

                // get cards from package
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

                // update ownership
                var cardIds = cards.Select(card => card.Id).ToList();
                UpdatePackageOwnership(cardIds, userId);

                // delete package from available packages
                using var deletePackageCommand = new NpgsqlCommand(
                    "DELETE FROM packages WHERE package_id = @package_id",
                    connection, transaction);

                DataLayer.AddParameter(deletePackageCommand, "@package_id", packageId);
                deletePackageCommand.ExecuteNonQuery();

                transaction.Commit();
                return cards;
            }
            catch(Exception ex) 
            {
                transaction.Rollback();
                throw new DbTransactionException($"Db Error while trying to buy package: " + ex.Message);
            }
        }
        /*
        Used to check if package cards are already present int db. 
        Placeholders are used to parse ids into the query but not in hardcoded length
         */
        public bool CheckPackageAlreadyExists(List<ICard> cards)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var ids = cards.Select(card => card.Id).ToList();
            
            var placeholders = string.Join(", ", ids.Select((_, i) => $"@id{i}"));
            var query = $"SELECT COUNT(*) FROM cards WHERE card_id IN ({placeholders});";

            using var cmd = new NpgsqlCommand(query, connection);

            for (int i = 0; i < ids.Count; i++)
            {
                DataLayer.AddParameter(cmd, $"@id{i}", ids[i]);
            }

            return (long)cmd.ExecuteScalar()! == 0;
        }

        // updates ownership of 5 cards (in cards not packages !)
        public void UpdatePackageOwnership(List<Guid> cardIds, int userId)
        {
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
            catch(Exception ex) 
            {
                transaction.Rollback();
                throw new DbTransactionException($"Error while trying update card ownership for user {userId}: {ex.Message}\n Rollback executed");
            }
        }
    }
}
