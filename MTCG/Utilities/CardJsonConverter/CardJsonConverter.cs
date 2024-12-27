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
                var id = root.GetProperty("Id").GetGuid();
                var name = root.GetProperty("Name").GetString() ?? string.Empty;
                var damage = root.GetProperty("Damage").GetInt32();
                var element = Enum.Parse<ElementType>(root.GetProperty("Element").GetString() ?? "Normal");

                // Determine card type
                var type = root.GetProperty("Type").GetString();
                if (type == "MonsterCard")
                {
                    var monsterType = Enum.Parse<MonsterType>(root.GetProperty("MonType").GetString() ?? "Goblin");
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
                    var spellType = Enum.Parse<SpellType>(root.GetProperty("SpellType").GetString() ?? "RegularSpell");
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
            writer.WriteStartObject();

            writer.WriteString("Id", value.Id.ToString());
            writer.WriteString("Name", value.Name);
            writer.WriteNumber("Damage", value.Damage);
            writer.WriteString("Element", value.ElemType.ToString());

            if (value is MonsterCard monsterCard)
            {
                writer.WriteString("Type", "MonsterCard");
                writer.WriteString("MonType", monsterCard.MonType.ToString());
            }
            else if (value is SpellCard spellCard)
            {
                writer.WriteString("Type", "SpellCard");
                writer.WriteString("SpellType", spellCard.SpellType.ToString());
            }
            else
            {
                throw new JsonException($"Unsupported card type: {value.GetType().Name}");
            }

            writer.WriteEndObject();
        }

    }
}
