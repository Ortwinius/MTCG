using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models.Users.DTOs
{
    /*
     Used to map User objects to User DTOs (e.g. JSON responses)
     */
    public static class UserMapping
    {
        public static UserDataDTO ToUserDataDTO(User user)
        {
            return new UserDataDTO(
                user.Name,
                user.Bio,
                user.Image
            );
        }
        public static UserStatsDTO ToUserStatsDTO(User user)
        {
            return new UserStatsDTO(
                user.Name,
                user.Elo,
                user.Wins,
                user.Losses
            );
        }
    }
}