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
        public int Damage { get; set;  } 
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
        public MonsterCard(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            ElemType = ElementType.Normal;
            Damage = 0;
        }
        public MonsterCard(MonsterType monsterType, ElementType elementType, int damage)
        {
            Id = Guid.NewGuid();
            MonType = monsterType;
            ElemType = elementType;
            Damage = damage;

            Name = monsterType.ToString(); // disgusting
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

        public void attack(ICard other)
        {
            Console.WriteLine($"{Name} Monster attacking {other.Name} - Type ?");
            // TODO
        }
    }
}
