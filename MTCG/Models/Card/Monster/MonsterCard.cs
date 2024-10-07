namespace MTCG.Models.Card.Monster
{
    public class MonsterCard : ICard
    {
        public Guid Id { get; private set; }
        public int Damage { get; } // not modifiable because its constant
        public string Name { get; private set; }
        public ElementType ElemType { get; private set; }
        
        public MonsterType MonType { get; private set; }
        // base constructor
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
        //public MonsterCard(string name, ElementType elementType, int damage)
        //{
        //    Id = Guid.NewGuid();
        //    Name = name;
        //    ElemType = elementType;
        //    Damage = damage;
        //}        

        public void attack(ICard other)
        {
            Console.WriteLine($"{Name} Monster attacking {other.Name} - Type ?");
            // TODO
        }
    }
}
