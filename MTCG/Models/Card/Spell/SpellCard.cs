using MTCG.Models.Card.Monster;
using System.Xml.Linq;

namespace MTCG.Models.Card.Spell
{
    public class SpellCard : ICard
    {
        public Guid Id { get; private set; }
        public int Damage { get; } // not modifiable because its constant
        public string Name { get; private set; }
        public ElementType ElemType { get; private set; }
        public SpellType? SpellType { get; private set; }

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
