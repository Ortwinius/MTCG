using MTCG.Models.Card.Monster;
using MTCG.Models.Card.Spell;
using MTCG.Models.Card;
using Npgsql;
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
        public static string CreateStandardJsonResponse(string message)
        {
            return JsonSerializer.Serialize(new { responseBody = message });
        }

        public static string ExtractUsernameFromPath(string path)
        {
            var segments = path.Split('/');
            return segments.Length > 2 ? segments[2] : throw new ArgumentException("Invalid path");
        }
    }
}
