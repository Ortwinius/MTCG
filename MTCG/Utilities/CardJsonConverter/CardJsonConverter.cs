using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;

namespace MTCG.Utilities.CardJsonConverter
{
    public class CardJsonConverter : JsonConverter<ICard>
    {
        public override ICard Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                using var jsonDocument = JsonDocument.ParseValue(ref reader);
                var root = jsonDocument.RootElement;

                // Extract common properties
                var id = root.GetProperty("id").GetGuid();
                var name = root.GetProperty("name").GetString() ?? string.Empty;
                var damage = root.GetProperty("damage").GetInt32();
                var element = Enum.Parse<ElementType>(root.GetProperty("element").GetString() ?? "Normal");

                // Determine card type
                var type = root.GetProperty("type").GetString();
                if (type == "MonsterCard")
                {
                    var monsterType = Enum.Parse<MonsterType>(root.GetProperty("monType").GetString() ?? "Goblin");
                    return new MonsterCard
                    {
                        Id = id,
                        Name = name,
                        Damage = damage,
                        ElemType = element,
                        MonType = monsterType
                    };
                }
                else if (type == "SpellCard")
                {
                    var spellType = Enum.Parse<SpellType>(root.GetProperty("spellType").GetString() ?? "RegularSpell");
                    return new SpellCard
                    {
                        Id = id,
                        Name = name,
                        Damage = damage,
                        ElemType = element,
                        SpellType = spellType
                    };
                }
                else
                {
                    throw new JsonException($"Unknown card type: {type}");
                }
            }
            catch (Exception ex)
            {
                throw new JsonException("Error during card deserialization.", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, ICard value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
