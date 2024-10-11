using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTCG.Utilities
{
    public class Helpers
    {
        public static string CreateJsonResponse(string message)
        {
            return JsonSerializer.Serialize(new { responseBody = message});
        }
    }
}
