using Microsoft.Extensions.DependencyInjection;
using MTCG.Server.RequestHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server.Endpoints.Initializer
{
    public static class EndpointInitializer
    {
        public static void InitializeEndpoints(IServiceProvider serviceProvider, HttpRequestHandler requestHandler)
        {
            var usersEndpoint = serviceProvider.GetRequiredService<UsersEndpoint>();
            var sessionsEndpoint = serviceProvider.GetRequiredService<SessionsEndpoint>();
            var packagesEndpoint = serviceProvider.GetRequiredService<PackagesEndpoint>();
            var cardsEndpoint = serviceProvider.GetRequiredService<CardsEndpoint>();
            var deckEndpoint = serviceProvider.GetRequiredService<DeckEndpoint>();
            var statsEndpoint = serviceProvider.GetRequiredService<StatsEndpoint>();
            var scoreboardEndpoint = serviceProvider.GetRequiredService<ScoreboardEndpoint>();
            var battlesEndpoint = serviceProvider.GetRequiredService<BattlesEndpoint>();
            var tradingsEndpoint = serviceProvider.GetRequiredService<TradingsEndpoint>();

            // add endpoints to requestHandler
            requestHandler.AddEndpoint("/users", usersEndpoint);
            requestHandler.AddEndpoint("/users/{username}", usersEndpoint);
            requestHandler.AddEndpoint("/sessions", sessionsEndpoint);
            requestHandler.AddEndpoint("/packages", packagesEndpoint);
            requestHandler.AddEndpoint("/cards", cardsEndpoint);
            requestHandler.AddEndpoint("/transactions/packages", packagesEndpoint);
            requestHandler.AddEndpoint("/deck", deckEndpoint);
            requestHandler.AddEndpoint("/deck?format=plain", deckEndpoint);
            requestHandler.AddEndpoint("/stats", statsEndpoint);
            requestHandler.AddEndpoint("/scoreboard", scoreboardEndpoint);
            requestHandler.AddEndpoint("/battles", battlesEndpoint);
            requestHandler.AddEndpoint("/tradings", tradingsEndpoint);
            requestHandler.AddEndpoint("/tradings/{username}", tradingsEndpoint);

        }
    }
}
