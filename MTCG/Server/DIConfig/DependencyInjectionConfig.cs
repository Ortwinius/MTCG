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
    public static class DependencyInjectionConfig
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Repositories (Transient)
            services.AddTransient<UserRepository>();
            services.AddTransient<PackageRepository>();

            // Services (Singleton)
            services.AddSingleton<AuthService>(sp =>
                AuthService.GetInstance(sp.GetRequiredService<UserRepository>()));
            services.AddSingleton<PackageService>(sp =>
                PackageService.GetInstance(sp.GetRequiredService<PackageRepository>()));

            // Endpoints (Transient)
            services.AddTransient<UsersEndPoint>();
            services.AddTransient<SessionsEndpoint>();

            return services.BuildServiceProvider();
        }
    }

}
