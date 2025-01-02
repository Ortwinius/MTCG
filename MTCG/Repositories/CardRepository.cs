using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Repositories.DL;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities;
using MTCG.Utilities.CustomExceptions;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace MTCG.Repositories
{
    public class CardRepository : ICardRepository
    {
        public List<ICard>? GetUserCards(int userId)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "SELECT card_id, name, type, element, damage " +
                "FROM cards " +
                "WHERE cards.owned_by = @user_id", connection);

            DataLayer.AddParameter(command, "user_id", userId);

            using var reader = command.ExecuteReader();

            return DataLayer.ParseCardsFromReader(reader);
        }
        public ICard? GetCardById(Guid id)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "SELECT card_id, name, type, element, damage, owned_by " +
                "FROM cards " +
                "WHERE card_id = @card_id", connection);

            DataLayer.AddParameter(command, "card_id", id);

            using var reader = command.ExecuteReader();

            return DataLayer.ParseCardFromReader(reader);
        }

        public void AddCard(ICard card, int? ownerUserId = null)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "INSERT INTO cards (card_id, name, type, element_type, damage, owned_by) " +
                "VALUES (@card_id, @name, @type, @element_type, @damage, @owned_by)", connection);

            DataLayer.AddParameter(command, "card_id", card.Id);
            DataLayer.AddParameter(command, "name", card.Name);
            DataLayer.AddParameter(command, "type", card is MonsterCard ? "Monster" : "Spell");
            DataLayer.AddParameter(command, "element_type", card.ElemType.ToString());
            DataLayer.AddParameter(command, "damage", card.Damage);
            DataLayer.AddParameter(command, "owned_by", ownerUserId ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
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
        public List<ICard>? GetAllCards()
        {
            var cards = new List<ICard>();

            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "SELECT * " +
                "FROM cards", connection);

            using var reader = command.ExecuteReader();

            return DataLayer.ParseCardsFromReader(reader);
        }

    }
}
