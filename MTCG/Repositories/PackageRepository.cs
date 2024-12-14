using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MTCG.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        public void AddPackage(List<ICard> cards)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                // Insert a new package and get its ID
                using var packageCommand = new NpgsqlCommand(
                    "INSERT INTO packages (price) VALUES (5) RETURNING package_id",
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
                        DataLayer.AddParameter(cardCommand, "@name", monsterCard.MonType?.ToString() ?? "Unknown");
                    }
                    else if (card is SpellCard spellCard)
                    {
                        DataLayer.AddParameter(cardCommand, "@type", "SpellCard");
                        DataLayer.AddParameter(cardCommand, "@name", spellCard.SpellType?.ToString() ?? "Unknown");
                    }

                    cardCommand.ExecuteNonQuery();

                    // Link the card to the package in the `package_cards` table
                    using var packageCardCommand = new NpgsqlCommand(
                        "INSERT INTO package_cards (package_id, card_id) VALUES (@package_id, @card_id)",
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
                throw;
            }
        }
        public bool AcquirePackage()
        {
            return true;
        }
    }
}
