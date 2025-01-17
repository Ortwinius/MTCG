﻿using MTCG.Models.Card;

namespace MTCG.Repositories.Interfaces
{
    public interface ICardRepository
    {
        ICard? GetCardById(Guid id);
        List<ICard>? GetUserCards(int userId);
        int GetOwnerOfCard(Guid cardToTrade);
        void UpdateCardOwnership(Guid id, int userId);
    }
}