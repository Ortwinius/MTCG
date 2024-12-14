using MTCG.Models.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MTCG.Models.Package
{
    public class Package
    {
        [JsonPropertyName("package_id")]
        public Guid Id { get; set; }
        [JsonPropertyName("cards")]
        public List<ICard>? Cards { get; set; }
        [JsonPropertyName("price")]
        public int Price { get; } = 5;
    }
}
