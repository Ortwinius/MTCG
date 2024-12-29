using MTCG.Server;
using MTCG.Repositories;
using MTCG.BusinessLogic.Services;
using MTCG.Server.DIConfig;

namespace MTCG
{
    class MTCGApp
    {
        static void Main(string[] args)
        {
            var serviceProvider = DIConfig.ConfigureServices();

            var serverController = new ServerController(serviceProvider);

            serverController.Listen();
        }
    }

}