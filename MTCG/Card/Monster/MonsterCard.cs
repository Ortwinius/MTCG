

namespace MTCG.Card.Monster
{
    public class MonsterCard : ICard
    {
        public int Damage { get; } // not modifiable because its constant
        public string Name { get; private set; } 
        public ElementType Type { get; private set; }

        public MonsterCard(string name, ElementType type, int damage)
        {
            Name = name;
            Type = type;
            Damage = damage;
        }
    }
}
