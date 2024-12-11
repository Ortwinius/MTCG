using MTCG.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLogic.Services
{
    public class PackageService
    {
        private static PackageService _instance;
        private readonly PackageRepository _packageRepository; 

        private PackageService(PackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }
        public static PackageService GetInstance(PackageRepository packageRepository)
        {
            if (_instance == null)
            {
                _instance = new PackageService(packageRepository);
            }
            return _instance;
        }
    }
}
