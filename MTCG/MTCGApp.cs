using MTCG.Server;
using MTCG.Repositories;
using MTCG.BusinessLogic.Services;

namespace MTCG
{
    class MTCGApp
    {
        static void Main(string[] args)
        {
            ServerController serverController = new ServerController();
            serverController.Listen();
        }
    }

}