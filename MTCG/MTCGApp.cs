using MTCG.Server;
using MTCG.Repositories;
using MTCG.BusinessLogic.Services;
using MTCG.Server.DI;

namespace MTCG
{
    class MTCGApp
    {
        static void Main(string[] args)
        {
            var serviceProvider = DIConfig.ConfigureServices();

            var serverController = new ServerController(serviceProvider);

            serverController.ListenAsync();
        }
    }

}