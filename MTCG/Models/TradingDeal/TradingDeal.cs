using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MTCG.Models.TradingDeal
{
    public class TradingDeal
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }
        [JsonPropertyName("CardToTrade")]
        public Guid CardToTrade { get; set; }
        [JsonPropertyName("Type")]
        public string? Type { get; set; }
        [JsonPropertyName("MinimumDamage")]
        public int MinDamage { get; set; } 
    }
}
