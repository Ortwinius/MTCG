

namespace MTCG.Card.SpellCard
{
    public class SpellCard : ICard
    {
        public int Damage { get; } // not modifiable because its constant
        public string Name { get; private set; } 
        public ElementType Type { get; private set; }

        public SpellCard(string name, ElementType type, int damage)
        {
            Name = name;
            Type = type;
            Damage = damage;
        }
    }
}
