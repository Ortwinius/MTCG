using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Models.Card;
using Npgsql;
using NpgsqlTypes;

namespace MTCG.Repositories
{
    public static class DataLayer
    {
        // Connection to the database
        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(
                "Server=localhost;" +
                "Username=postgres;" +
                "Password=postgres;" +
                "Database=mtcgdb"
                );
        }
        // Add a parameter to a command to prevent SQL injection
        public static void AddParameter(NpgsqlCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;

            if (value != null)
            {
                parameter.NpgsqlDbType = GetDbType(value);
            }

            command.Parameters.Add(parameter);
        }
        public static List<ICard> ParseCardsFromReader(NpgsqlDataReader cardReader)
        {
            var cards = new List<ICard>();

            while (cardReader.Read())
            {
                var card = ParseCardFromReader(cardReader);
                if (card != null)
                    cards.Add(card);
            }

            return cards;
        }

        public static ICard? ParseCardFromReader(NpgsqlDataReader cardReader)
        {
            var cardId = cardReader.GetGuid(0);
            var name = cardReader.GetString(1);
            var type = cardReader.GetString(2);
            var element = Enum.Parse<ElementType>(cardReader.GetString(3));
            var damage = cardReader.GetInt32(4);

            return type switch
            {
                "MonsterCard" => new MonsterCard(cardId, name, element, damage, Enum.Parse<MonsterType>(name)),
                "SpellCard" => new SpellCard(cardId, name, element, damage, Enum.Parse<SpellType>(name)),
                _ => throw new InvalidOperationException($"Unknown card type: {type}")
            };
        }
        // could be put in to helpers
        private static NpgsqlDbType GetDbType(object value)
        {
            return value switch
            {
                int => NpgsqlDbType.Integer,
                long => NpgsqlDbType.Bigint,
                string => NpgsqlDbType.Text,
                DateTime => NpgsqlDbType.Timestamp,
                bool => NpgsqlDbType.Boolean,
                Guid => NpgsqlDbType.Uuid,
                float => NpgsqlDbType.Real,
                double => NpgsqlDbType.Double,
                _ => throw new InvalidOperationException($"Unsupported parameter type: {value.GetType()}")
            };
        }
    }
}
