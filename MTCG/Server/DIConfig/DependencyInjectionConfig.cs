using Microsoft.Extensions.DependencyInjection;
using MTCG.BusinessLogic.Services;
using MTCG.Repositories;
using MTCG.Server.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server.DIConfig
{
    // responsible for setting up the dependency injection container for the services
    public static class DependencyInjectionConfig
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Repositories (Transient)
            services.AddTransient<UserRepository>();
            services.AddTransient<PackageRepository>();
            services.AddTransient<CardRepository>();
            services.AddTransient<PackageRepository>();
            //services.AddTransient<DeckRepository>();

            // Services (Singleton)
            services.AddSingleton<AuthService>(sp =>
                AuthService.GetInstance(sp.GetRequiredService<UserRepository>()));
            services.AddSingleton<PackageService>(sp => 
                PackageService.GetInstance(sp.GetRequiredService<PackageRepository>(),
                                           sp.GetRequiredService<UserRepository>()));
            services.AddSingleton<CardService>(sp =>
                CardService.GetInstance(sp.GetRequiredService<CardRepository>()));
            //services.AddSingleton<DeckService>(sp =>
            //    DeckService.GetInstance(sp.GetRequiredService<DeckRepository>(),
            //                            sp.GetRequiredService<CardService>()));

            // Endpoints (Transient)
            services.AddTransient<UsersEndpoint>();
            services.AddTransient<SessionsEndpoint>();
            services.AddTransient<CardsEndpoint>();
            services.AddTransient<PackagesEndpoint>();
            //services.AddTransient<DeckEndpoint>();

            return services.BuildServiceProvider();
        }
    }

}
