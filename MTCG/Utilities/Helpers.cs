using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Models.Card;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTCG.Utilities
{
    public class Helpers
    {
        public static string CreateStandardJsonResponse(string message)
        {
            return JsonSerializer.Serialize(new { responseBody = message });
        }
        public static List<ICard> ParseCardsFromReader(NpgsqlDataReader cardReader)
        {
            var cards = new List<ICard>();

            while (cardReader.Read())
            {
                var cardId = cardReader.GetGuid(0);
                var name = cardReader.GetString(1);
                var type = cardReader.GetString(2);
                var element = Enum.Parse<ElementType>(cardReader.GetString(3));
                var damage = cardReader.GetInt32(4);

                if (type == "MonsterCard")
                {
                    var monsterType = Enum.Parse<MonsterType>(name);
                    cards.Add(new MonsterCard(cardId, name, element, damage));
                }
                else if (type == "SpellCard")
                {
                    var spellType = Enum.Parse<SpellType>(name);
                    cards.Add(new SpellCard(cardId, name, element, damage));
                }
            }

            return cards;
        }
    }
}
