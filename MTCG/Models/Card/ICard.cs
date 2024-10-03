﻿// interface class for cards
namespace MTCG.Models.Card
{
    public enum ElementType
    {
        Fire,
        Water,
        Normal
    }

    public interface ICard
    {
        Guid Id { get; }
        int Damage { get; }
        string Name { get; }
        ElementType Type { get; }

        void attack(ICard other);
    }
}
