using MTCG.BusinessLogic.Services;
using MTCG.Models.ResponseObject;
using MTCG.Models.TradingDeal;
using MTCG.Utilities.Exceptions.CustomExceptions;
using System.Text.Json;

namespace MTCG.Server.Endpoints
{
    public class TradingsEndpoint : IHttpEndpoint
    {
                private readonly TradingService _tradingService;
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public TradingsEndpoint(TradingService tradingService, AuthService authService, UserService userService)
        {
            _tradingService = tradingService;
            _authService = authService;
            _userService = userService;
        }
        public ResponseObject HandleRequest(
            string method, 
            string path, 
            string? body, 
            Dictionary<string, string> headers, 
            Dictionary<string, string>? routeParams = null)
        {
            switch(method)
            {
                case "GET":
                    return GetTradings(headers);
                case "POST" when path == "/tradings":
                    return CreateTradingDeal(body, headers);
                case "POST" when routeParams != null && routeParams.ContainsKey("tradingdealId"):
                    return ExecuteTrade(routeParams["tradingdealId"], body, headers);
                case "DELETE" when routeParams != null && routeParams.ContainsKey("tradingdealId"):
                    return RemoveTradingDeal(routeParams["tradingdealId"], headers);
                default:
                    return new ResponseObject(405, "Method not allowed.");
            }
        }

        private ResponseObject GetTradings(Dictionary<string, string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);
                _authService.EnsureAuthenticated(token);

                var tradings = _tradingService.GetAllTradings();

                var jsonTrades = JsonSerializer.Serialize(tradings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                return new ResponseObject(200, jsonTrades);
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }
        /*
        Used to not only send an offer but if the requirements meet also transfer the cards 
        */
        private ResponseObject CreateTradingDeal(string? body, Dictionary<string, string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                var deal = JsonSerializer.Deserialize<TradingDeal>(body!);

                _tradingService.CreateTradingDeal(deal, user!);

                return new ResponseObject(201, "Trading deal successfully created.");
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }
        private ResponseObject RemoveTradingDeal(string tradingdealId, Dictionary<string, string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                if (!Guid.TryParse(tradingdealId, out var tradeId))
                {
                    return new ResponseObject(400, "Invalid trading deal ID.");
                }

                _tradingService.DeleteTradingDeal(tradeId, user);

                return new ResponseObject(200, "Trading deal removed successfully.");
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }

        private ResponseObject ExecuteTrade(string tradingdealId, string? body, Dictionary<string, string> headers)
        {
            try
            {
                var token = _authService.GetAuthToken(headers);
                var user = _userService.GetUserByToken(token);

                if (!Guid.TryParse(tradingdealId, out var tradeId))
                {
                    return new ResponseObject(400, "Invalid trading deal ID.");
                }

                var offeredCardId = JsonSerializer.Deserialize<Guid>(body!);
                _tradingService.ExecuteTrade(tradeId, offeredCardId, user!);

                return new ResponseObject(201, "Trade executed successfully.");
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }
    }
}