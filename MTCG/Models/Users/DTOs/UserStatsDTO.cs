using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models.Users.DTOs
{
    public class UserStatsDTO
    {
        public string? Name { get; set; }
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public UserStatsDTO(string? name, int elo, int wins, int losses)
        {
            Name = name;
            Elo = elo;
            Wins = wins;
            Losses = losses;
        }
    }

}
