using MTCG.Models.Users;
using MTCG.Models.Users.DTOs;

namespace MTCG.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void AddUser(User user);
        User? GetUserByUsername(string username);
        User? GetUserByToken(string token);
        bool UserExists(string username);
        bool ValidateToken(string token);
        bool IsAdmin(string username);
        UserDataDTO? GetUserDataByToken(string token);
        UserStatsDTO? GetUserStatsByToken(string token);
        List<UserStatsDTO> GetAllUserStats(string token);
        void UpdateUser(User user);
        void UpdateUserData(string username, UserDataDTO userData);
        void UpdateUserStats(string username, UserStatsDTO userStats);

    }
}