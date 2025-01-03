using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Models.TradingDeal;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.CustomExceptions;
using System;
using System.Collections.Generic;

namespace MTCG.BusinessLogic.Services
{
    public class TradingService
    {
        private static TradingService? _instance;
        private readonly ITradingRepository _tradingRepository;
        private readonly IDeckRepository _deckRepository;
        private readonly ICardRepository _cardRepository;
        private TradingService(ITradingRepository tradingRepository, IDeckRepository deckRepository, ICardRepository cardRepository)
        {
            _tradingRepository = tradingRepository;
            _deckRepository = deckRepository;
            _cardRepository = cardRepository;
        }

        public static TradingService GetInstance(ITradingRepository tradingRepository, IDeckRepository deckRepository, ICardRepository cardRepository)
        {
            if(_instance == null)
            {
                _instance = new TradingService(tradingRepository, deckRepository, cardRepository);
            }
            return _instance;
        }

        public static void ResetInstance() => _instance = null;

        public List<TradingDeal>? GetAllTradings()
        {
            return _tradingRepository.GetAllTradings();
        }

        /*
        CreateTradingDeal:
        Gets user cards and then checks if
        - user has any cards
        - user has the card he wants to trade
        - user is not using the card in his deck
        if all checks pass, the trade is created
        */
        public void CreateTradingDeal(TradingDeal deal, User user)
        {
            var userCards = _cardRepository.GetUserCards(user.UserId);

            if(userCards == null)
            {
                throw new InvalidTradeException("User doesnt have any cards to trade.");
            }

            var userDeck = _deckRepository.GetDeckOfUser(user.UserId);

            if (!userCards.Exists(c => c.Id == deal.CardToTrade))
            {
                throw new InvalidTradeException("Card does not belong to the user.");
            }
            if (_deckRepository.GetDeckOfUser(user.UserId)!.Exists(c => c.Id == deal.CardToTrade))
            {
                throw new InvalidTradeException("You cannot trade cards which you currently use in your deck");
            }

            _tradingRepository.CreateTradingDeal(deal);
        }

        public void DeleteTradingDeal(Guid tradeId, User? user)
        {
            throw new NotImplementedException();
        }
        /*
        ExecuteTrade:
        Used to execute trade but only if
        - trade exists
        - user is not trading with himself
        - the offered card meets the requirements
        */
        public void ExecuteTrade(Guid tradeId, Guid offeredCardId, User user)
        {
            var trade = _tradingRepository.GetTradingById(tradeId);

            if (trade == null)
            {
                throw new TradeNotFoundException();
            }

            if (_cardRepository.GetUserCards(user.UserId)!.Exists(c => c.Id == offeredCardId))
            {
                throw new InvalidTradeException("You cannot trade with yourself.");
            }

            if (!ValidateOfferedCard(trade, offeredCardId))
            {
                throw new InvalidTradeException("The offered card does not meet the requirements.");
            }

            //_tradingRepository.ExecuteTrade(tradeId, offeredCardId, user.UserId);

            throw new NotImplementedException();
        }

        // check if the card is of the correct type and has the required damage
        private bool ValidateOfferedCard(TradingDeal trade, Guid offeredCardId)
        {
            var card = _cardRepository.GetCardById(offeredCardId);

            if (card == null)
            {
                return false;
            }

            var isCorrectType = trade.Type!.ToLower() switch
            {
                "monster" => card is MonsterCard,
                "spell" => card is SpellCard,
                _ => false
            };

            return isCorrectType && card.Damage >= trade.MinDamage;
        }
    }
}
