using MTCG.Models.Card.Monster;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MTCG.Models.Card.Spell
{
    public class SpellCard : ICard
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        [JsonPropertyName("Damage")]
        public int Damage { get; set; }
        [JsonPropertyName("Element")]
        public ElementType ElemType { get; set; }
        [JsonPropertyName("SpellType")]
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
        public SpellCard(Guid id, string name, ElementType elemType, int damage, SpellType spellType)
        {
            Id = id;
            Name = name;
            ElemType = elemType;
            Damage = damage;
            SpellType = spellType;
        }
    }
}
