using MTCG.Models.Users;
using MTCG.Models.Users.DTOs;
using MTCG.Repositories.DL;
using MTCG.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace MTCG.Repositories
{
    public class UserRepository : IUserRepository
    {
        public void AddUser(User user)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "INSERT INTO users (username, password, name) " +
                "VALUES (@username, @password, @name)", connection);

            DataLayer.AddParameter(cmd, "username", user.Username);
            DataLayer.AddParameter(cmd, "password", user.Password);
            DataLayer.AddParameter(cmd, "name", user.Username);

            cmd.ExecuteNonQuery();
        }
        public User? GetUserByUsername(string username)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();


            var cmd = new NpgsqlCommand(
                "SELECT user_id, username, password, auth_token, coins " +
                "FROM users " +
                "WHERE username = @username", connection);

            DataLayer.AddParameter(cmd, "username", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    reader.GetInt32(0), // UserId
                    reader.GetString(1), // Username
                    reader.GetString(2), // Password
                    reader.IsDBNull(3) ? null : reader.GetString(3), // AuthToken
                    reader.GetInt32(4)  // Coins
                );
            }

            return null;
        }
        public User? GetUserByToken(string token)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();


            var cmd = new NpgsqlCommand(
                "SELECT user_id, username, password, auth_token, coins " +
                "FROM users " +
                "WHERE auth_token = @auth_token", connection);

            DataLayer.AddParameter(cmd, "auth_token", token);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    reader.GetInt32(0), // UserId
                    reader.GetString(1), // Username
                    reader.GetString(2), // Password
                    reader.GetString(3), // AuthToken
                    reader.GetInt32(4)  // Coins
                );
            }

            return null;
        }
        public UserStatsDTO? GetUserStatsByToken(string token)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "SELECT name, elo, wins, losses " +
                "FROM users " +
                "WHERE auth_token = @auth_token", connection);

            DataLayer.AddParameter(cmd, "auth_token", token);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UserStatsDTO(
                    reader.GetString(0),// Name
                    reader.GetInt32(1), // Elo
                    reader.GetInt32(2), // Wins
                    reader.GetInt32(3)  // Losses
                );
            }

            return null;
        }
        public UserDataDTO? GetUserDataByToken(string token)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "SELECT name, bio, img " +
                "FROM users " +
                "WHERE auth_token = @auth_token", connection);

            DataLayer.AddParameter(cmd, "auth_token", token);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UserDataDTO(
                    reader.IsDBNull(0) ? "" : reader.GetString(0), // Name
                    reader.IsDBNull(1) ? "" : reader.GetString(1), // Bio
                    reader.IsDBNull(2) ? "" : reader.GetString(2)  // Image
                );
            }

            return null;
        }
        public List<UserStatsDTO> GetAllUserStats(string token)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "SELECT name, elo, wins, losses " +
                "FROM users " +
                "ORDER BY elo DESC", connection);

            using var reader = cmd.ExecuteReader();
            var userStats = new List<UserStatsDTO>();
            while (reader.Read())
            {
                userStats.Add(new UserStatsDTO(
                    reader.IsDBNull(0) ? "" : reader.GetString(0), // Name
                    reader.GetInt32(1), // Elo
                    reader.GetInt32(2), // Wins
                    reader.GetInt32(3)  // Losses
                ));
            }

            return userStats;
        }
        public void UpdateUser(User user)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "UPDATE users SET " +
                "auth_token = @auth_token, " + // Hier das Komma hinzufügen
                "coins = @coins " +
                "WHERE username = @username", connection);


            // Add parameters
            DataLayer.AddParameter(cmd, "auth_token", user.AuthToken);
            DataLayer.AddParameter(cmd, "coins", user.Coins);
            DataLayer.AddParameter(cmd, "username", user.Username);
            // Execute query
            cmd.ExecuteNonQuery();
        }
        public void UpdateUserData(string username, UserDataDTO userData)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();
            var cmd = new NpgsqlCommand(
                "UPDATE users SET " +
                "name = @name, " +
                "bio = @bio, " +
                "img = @img " +
                "WHERE username = @username", connection);

            DataLayer.AddParameter(cmd, "name", userData.Name);
            DataLayer.AddParameter(cmd, "bio", userData.Bio);
            DataLayer.AddParameter(cmd, "img", userData.Image);
            DataLayer.AddParameter(cmd, "username", username);

            cmd.ExecuteNonQuery();
        }
        public void UpdateUserStats(string username, UserStatsDTO userStats)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();
            var cmd = new NpgsqlCommand(
                "UPDATE users SET " +
                "elo = @elo, " +
                "wins = @wins, " +
                "losses = @losses " +
                "WHERE username = @username", connection);

            DataLayer.AddParameter(cmd, "elo", userStats.Elo);
            DataLayer.AddParameter(cmd, "wins", userStats.Wins);
            DataLayer.AddParameter(cmd, "losses", userStats.Losses);
            DataLayer.AddParameter(cmd, "username", username);

            cmd.ExecuteNonQuery();
        }
        public bool ValidateToken(string token)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "SELECT 1 FROM users WHERE auth_token = @auth_token LIMIT 1",
                connection);

            DataLayer.AddParameter(cmd, "auth_token", token);

            // If a row is found, the token is valid
            return cmd.ExecuteScalar() != null;
        }

        public bool UserExists(string username)
        {
            return GetUserByUsername(username) != null;
        }
        public bool IsAdmin(string username)
        {
            var user = GetUserByUsername(username);
            return user != null && user.Username == "admin";
        }


    }
}
