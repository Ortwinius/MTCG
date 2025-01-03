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
using Microsoft.Extensions.DependencyInjection;
using MTCG.Server.Endpoints;
using MTCG.Server.RequestHandler;
using System.Text.RegularExpressions;

namespace MTCG.Utilities
{
    public class Helpers
    {
        public static string CreateStandardJsonResponse(string message)
        {
            return JsonSerializer.Serialize(new { responseBody = message });
        }

        // Converts a route template (e.g., "users/{username}") to a regex pattern
        public static string ConvertToRegexPattern(string routeTemplate)
        {
            if (string.IsNullOrEmpty(routeTemplate))
                throw new ArgumentException("Route template cannot be null or empty.");

            // Replace route parameters (e.g., {username}) with named groups
            string pattern = "^" + routeTemplate
                .Replace("{", "(?<")  // Start named group
                .Replace("}", ">[^/]+)") + "$"; // End named group and match until the next "/"
            return pattern;
        }

        // Extract route parameters (e.g., {username}) from a matched path
        public static Dictionary<string, string> ExtractRouteParameters(string path, string pattern)
        {
            var match = Regex.Match(path, pattern);
            if (!match.Success)
                throw new InvalidOperationException("The provided path does not match the expected pattern.");

            var routeParams = new Dictionary<string, string>();
            foreach (var groupName in match.Groups.Keys)
            {
                if (groupName != "0" && match.Groups[groupName].Success)
                {
                    routeParams[groupName] = match.Groups[groupName].Value;
                }
            }
            return routeParams;
        }
    }
}
