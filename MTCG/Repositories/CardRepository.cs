using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace MTCG.Repositories
{
    public class CardRepository
    {
        public void AddCard(ICard card, int? ownerUserId = null)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "INSERT INTO cards (card_id, name, type, element_type, damage, owner_user_id) " +
                "VALUES (@card_id, @name, @type, @element_type, @damage, @owner_user_id)", connection);

            DataLayer.AddParameter(command, "card_id", card.Id);
            DataLayer.AddParameter(command, "name", card.Name);
            DataLayer.AddParameter(command, "type", card is MonsterCard ? "Monster" : "Spell");
            DataLayer.AddParameter(command, "element_type", card.ElemType.ToString());
            DataLayer.AddParameter(command, "damage", card.Damage);
            DataLayer.AddParameter(command, "owner_user_id", ownerUserId ?? (object) DBNull.Value);

            command.ExecuteNonQuery();
        }

        public ICard? GetCardById(Guid id)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "SELECT card_id, name, type, element_type, damage, owner_user_id " +
                "FROM cards " +
                "WHERE card_id = @card_id", connection);

            DataLayer.AddParameter(command, "card_id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var cardId = reader.GetGuid(0);
                var name = reader.GetString(1);
                var type = reader.GetString(2);
                var elementType = Enum.Parse<ElementType>(reader.GetString(3));
                var damage = reader.GetInt32(4);
                var ownerUserId = reader.IsDBNull(5) ? (int?) null : reader.GetInt32(5); // can be null

                ICard? card = type switch
                {
                    "Monster" => new MonsterCard(cardId, name, elementType, damage),
                    "Spell" => new SpellCard(cardId, name, elementType, damage),
                    _ => throw new InvalidOperationException($"Unknown card type: {type}")
                };

                return card;
            }

            return null; // Card not found
        }

        public void DeleteCard(Guid cardId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "DELETE FROM cards " +
                "WHERE card_id = @card_id", connection);

            DataLayer.AddParameter(command, "card_id", cardId);

            command.ExecuteNonQuery();
        }
        // maybe not needed?
        public List<ICard>? GetAllCards()
        {
            var cards = new List<ICard>();

            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "SELECT card_id, name, type, element_type, damage, owner_user_id " +
                "FROM cards", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var cardId = reader.GetGuid(0);
                var name = reader.GetString(1);
                var type = reader.GetString(2);
                var elementType = Enum.Parse<ElementType>(reader.GetString(3));
                var damage = reader.GetInt32(4);
                var ownerUserId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);

                ICard? card = type switch
                {
                    "Monster" => new MonsterCard(cardId, name, elementType, damage),
                    "Spell" => new SpellCard(cardId, name, elementType, damage),
                    _ => throw new InvalidOperationException($"Unknown card type: {type}")
                };

                if (card != null) cards.Add(card);
            }

            return cards;
        }

        public void UpdateCardOwnership(Guid cardId, int? newOwnerUserId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "UPDATE cards " +
                "SET owner_user_id = @new_owner_user_id " +
                "WHERE card_id = @card_id", connection);

            DataLayer.AddParameter(command, "card_id", cardId);
            DataLayer.AddParameter(command, "new_owner_user_id", newOwnerUserId ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        // probably not needed
        public ICard? GetRandomCard()
        {
            var allCards = GetAllCards();
            if (allCards!.Count == 0) return null;

            Random random = new Random();
            return allCards[random.Next(allCards.Count)];
        }

    }
}
