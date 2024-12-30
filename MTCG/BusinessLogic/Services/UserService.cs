using MTCG.Models.Users.DTOs;
using MTCG.Models.Users;
using MTCG.Repositories;
using MTCG.Utilities.CustomExceptions;
using Microsoft.AspNetCore.Identity;

namespace MTCG.BusinessLogic.Services
{
    // TODO
    public class UserService
    {
        private static UserService? _instance;
        private readonly UserRepository _userRepository;

        private UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public static UserService GetInstance(UserRepository userRepository)
        {
            if (_instance == null)
            {
                _instance = new UserService(userRepository);
            }
            return _instance;
        }
        // Get user by username
        public User? GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }
        public User? GetUserByToken(string authtoken)
        {
            var user = _userRepository.GetUserByToken(authtoken);
            if (user == null)
            {
                throw new UnauthorizedException();
            }
            return user;
        }
        // Get user data by token
        public UserDataDTO? GetUserDataByToken(string authToken)
        {
            var userData = _userRepository.GetUserDataByToken(authToken);
            if (userData == null)
            {
                throw new UnauthorizedException();
            }
            return userData;
        }
        public void UpdateUserData(string username, UserDataDTO userData)
        {
            _userRepository.UpdateUserData(username, userData);
        }
        // get user stats by token
        public UserStatsDTO GetUserStatsByToken(string authToken)
        {
            var userStats = _userRepository.GetUserStatsByToken(authToken);
            if (userStats == null)
            {
                throw new UnauthorizedException();
            }
            return userStats;
        }
        public List<UserStatsDTO>? GetAllUserStats(string authToken)
        {
            var scoreboard = _userRepository.GetAllUserStats(authToken);
            return scoreboard;
        }
    }
}