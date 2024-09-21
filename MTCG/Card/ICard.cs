// interface class for cards
namespace MTCG.Card
{
    public enum ElementType
    {
        Fire,
        Water,
        Normal
    }

    public interface ICard
    {
        int Damage { get; }
        string Name { get; }
        ElementType Type { get; }
    }
}
