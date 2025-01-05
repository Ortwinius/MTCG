using System.Diagnostics.Eventing.Reader;
using System.Text.Json.Serialization;

namespace MTCG.Models.Card.Monster
{
    public class MonsterCard : ICard
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        [JsonPropertyName("Damage")]
        public int Damage { get; set; } 
        [JsonPropertyName("Element")]
        public ElementType ElemType { get; set; }

        [JsonPropertyName("MonType")]
        public MonsterType? MonType { get; set; } 
        // base constructor
        public MonsterCard()
        {
            Id = Guid.NewGuid();
            Name = "";
            ElemType = ElementType.Normal;
        }

        // for db
        public MonsterCard(Guid id, string name, ElementType elemType, int damage, MonsterType monType)
        {
            Id = id;
            Name = name; 
            ElemType = elemType; 
            Damage = damage;
            MonType = monType;
        }
    }
}
