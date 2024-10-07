using MTCG.Models.Card.Monster;
using System.Xml.Linq;

namespace MTCG.Models.Card.Spell
{
    public class SpellCard : ICard
    {
        public Guid Id { get; private set; }
        public int Damage { get; } // not modifiable because its constant
        public string Name { get; private set; }
        public SpellType SpellType { get; private set; }
        public ElementType ElemType { get; private set; }

        //public SpellCard(string name, ElementType elementType, int damage)
        //{
        //    Id = Guid.NewGuid();
        //    Name = name;
        //    ElemType = elementType;
        //    Damage = damage;
        //}        
        public SpellCard(SpellType spellType, ElementType elementType, int damage)
        {
            Id = Guid.NewGuid();
            SpellType = spellType;
            ElemType = elementType;
            Damage = damage;

            Name = spellType.ToString(); // disgusting
        }
        public void attack(ICard other)
        {
            Console.WriteLine($"{Name} Spell attacking {other.Name} - Type ?");
            // TODO
        }
    }
}
