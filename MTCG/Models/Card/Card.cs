// interface class for cards
using System.Text.Json.Serialization;

namespace MTCG.Models.Card
{
    public enum ElementType
    {
        Fire,
        Water,
        Normal
    }
    public enum MonsterType
    {
        Goblin = 0,
        Troll,
        FireTroll,
        RegularTroll,
        WaterElf,
        FireElf,
        RegularElf,
        Knight,
        Dragon,
        Ork,
        Kraken,
        Wizzard,
    }
    public enum SpellType
    {
        WaterSpell,
        FireSpell,
        RegularSpell,
    }
    public interface ICard
    {
        Guid Id { get; } 
        int Damage { get; }
        string Name { get; }
        ElementType ElemType { get; }
    }
}
