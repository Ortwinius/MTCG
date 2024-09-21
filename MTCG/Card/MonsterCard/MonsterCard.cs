

namespace MTCG.Card.MonsterCard
{
    public class MonsterCard : ICard
    {
        public int Damage { get; }
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
