using MTCG.Models.Card.Monster;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MTCG.Models.Card.Spell
{
    public class SpellCard : ICard
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("damage")]
        public int Damage { get; set; }
        [JsonPropertyName("element")]
        public ElementType ElemType { get; set; }
        [JsonPropertyName("spellType")]
        public SpellType? SpellType { get; set; }

        public SpellCard()
        {
            Id = Guid.NewGuid();
            Name = "";
            ElemType = ElementType.Normal;
            Damage = 0;
        }
        public SpellCard(SpellType spellType, ElementType elementType, int damage)
        {
            Id = Guid.NewGuid();
            SpellType = spellType;
            ElemType = elementType;
            Damage = damage;

            Name = spellType.ToString(); // disgusting
        }

        // for db
        public SpellCard(Guid id, string name, ElementType elemType, int damage)
        {
            Id = id;
            Name = name;
            ElemType = elemType;
            Damage = damage;
        }
        public void attack(ICard other)
        {
            Console.WriteLine($"{Name} Spell attacking {other.Name} - Type ?");
            // TODO
        }
    }
}
