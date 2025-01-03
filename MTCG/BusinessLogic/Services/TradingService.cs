using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Models.TradingDeal;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.Exceptions.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Transactions;

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
        - trading deal with the deal.Id already exists
        - user has any cards
        - user has the card he wants to trade
        - user is not using the card in his deck
        if all checks pass, the trade is created
        */
        public void CreateTradingDeal(TradingDeal? deal, User user)
        {
            if(deal == null)
            {
                throw new InvalidTradeException(nameof(deal));
            }

            var tradingList = _tradingRepository.GetAllTradings();

            // check if trade already exists in db
            if(tradingList.Exists(t => t.Id == deal.Id))
            {
                throw new TradeAlreadyExistsException("Trade with this ID already exists.");
            }
            var userCards = _cardRepository.GetUserCards(user.UserId);

            if(userCards == null || !userCards.Any())
            {
                throw new InvalidTradeException("User doesnt have any cards to trade.");
            }

            var userDeck = _deckRepository.GetDeckOfUser(user.UserId);

            // check if card belongs to user
            if (!userCards.Exists(c => c.Id == deal.CardToTrade))
            {
                throw new InvalidTradeException("Card does not belong to the user.");
            }

            // check if card is used in deck
            if (userDeck != null && userDeck.Exists(c => c.Id == deal.CardToTrade))
            {
                throw new InvalidTradeException("You cannot trade cards which you currently use in your deck");
            }

            _tradingRepository.CreateTradingDeal(deal);
        }
        /*
        ExecuteTrade:
        Used to execute trade but only if
        - trade exists
        - user is not trading with himself
        - the offered card meets the requirements

        The trade.CardToTrade is the card which the user wants to get and the offered card is the card which the user wants to give
        If so trade is deleted and the card is transferred
        */
        public void ExecuteTrade(Guid tradeId, Guid offeredCardId, User user)
        {
            Console.WriteLine($"[TradingService] Executing trade with ID {tradeId}.");
            var trade = _tradingRepository.GetTradingById(tradeId);
            Console.WriteLine("Trade has been found and returned");

            if (trade == null)
            {
                Console.WriteLine($"[TradingService] Trade with ID {tradeId} not found in db.");
                throw new TradeNotFoundException();
            }

            var userCards = _cardRepository.GetUserCards(user.UserId);
            
            if (userCards == null || !userCards.Any())
            {
                Console.WriteLine($"[TradingService] User {user.Username} doesnt have any cards to trade.");
                throw new InvalidTradeException("User doesnt have any cards to trade.");
            }

            if (userCards.Exists(c => c.Id == trade.CardToTrade))
            {
                Console.WriteLine($"[TradingService] User {user.Username} is trying to trade with himself. His cards:");
                foreach (var card in userCards)
                {
                    Console.WriteLine($"[TradingService] {card.Id}");
                }
                Console.WriteLine($"[TradingService] Trade card: {trade.CardToTrade}");
                throw new InvalidTradeException("You cannot trade with yourself.");
            }

            Console.WriteLine("[TradingService] User doesnt already possess the card he wants to get!! Checking the card parameters");

            if (!ValidateOfferedCard(trade, offeredCardId))
            {
                Console.WriteLine($"[TradingService] Offered card {offeredCardId} does not meet the requirements.");
                Console.WriteLine($"[TradingService] Trade requirements: Type: {trade.Type}, MinDamage: {trade.MinDamage}");
                throw new InvalidTradeException("The offered card does not meet the requirements.");
            }

            using var transaction = new TransactionScope();

            try
            {
                Console.WriteLine($"[TradingService] Deleting trade with ID {tradeId}.");
                _tradingRepository.DeleteTradingDeal(tradeId);

                // change ownership of cards
                Console.WriteLine($"[TradingService] Transferring cards {trade.CardToTrade} and {offeredCardId}.");
                Console.WriteLine($"[TradingService] getting Id of trade creator");
                var tradeCreatorId = _cardRepository.GetOwnerOfCard(trade.CardToTrade);
                Console.WriteLine($"[TradingService] SUCCESSFUL getting Id of trade creator");
                Console.WriteLine($"[TradingService] updating cardownership 1");
                _cardRepository.UpdateCardOwnership(trade.CardToTrade, user.UserId);
                Console.WriteLine($"[TradingService] SUCCESSFUL updating cardownership 1");
                Console.WriteLine($"[TradingService] updating cardownership 2");
                _cardRepository.UpdateCardOwnership(offeredCardId, tradeCreatorId);
                Console.WriteLine($"[TradingService] SUCCESSFUL updating cardownership 2");

                Console.WriteLine("[TradingService] Committing transaction.");
                transaction.Complete();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TradingService] Error during trade execution: {ex.Message}");
                throw;
            }
        }
        public void DeleteTradingDeal(Guid tradeId, User? user)
        {
            Console.WriteLine($"[TradingService] Deleting trade with ID {tradeId}.");
            var trade = _tradingRepository.GetTradingById(tradeId);

            if (trade == null)
            {
                Console.WriteLine($"[TradingService] Trade with ID {tradeId} not found in db.");
                throw new TradeNotFoundException();
            }

            _tradingRepository.DeleteTradingDeal(tradeId);
        }

        // check if the card is of the correct type and has the required damage
        private bool ValidateOfferedCard(TradingDeal trade, Guid offeredCardId)
        {
            Console.WriteLine("[TradingService] Trying to check parameters of card");
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

            Console.WriteLine($"[TradingService] {offeredCardId} meets requirements? {isCorrectType && card.Damage >= trade.MinDamage}");

            return isCorrectType && card.Damage >= trade.MinDamage;
        }
    }
}
