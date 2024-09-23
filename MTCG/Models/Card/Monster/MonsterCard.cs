using MTCG.Models.Card;

namespace MTCG.Models.Card.Monster
{
    public class MonsterCard : ICard
    {
        public Guid Id { get; private set; }
        public int Damage { get; } // not modifiable because its constant
        public string Name { get; private set; }
        public ElementType Type { get; private set; }

        public MonsterCard(string name, ElementType type, int damage)
        {
            Id = Guid.NewGuid();
            Name = name;
            Type = type;
            Damage = damage;
        }
    }
}
