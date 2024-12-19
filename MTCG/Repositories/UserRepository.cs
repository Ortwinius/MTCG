using MTCG.Models.Users;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace MTCG.Repositories
{
    public class UserRepository
    {
        public void AddUser(User user)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "INSERT INTO users (username, password) " +
                "VALUES (@username, @password)", connection);

            DataLayer.AddParameter(cmd, "username", user.Username);
            DataLayer.AddParameter(cmd, "password", user.Password);

            cmd.ExecuteNonQuery();
        }
        public User? GetUserByUsername(string username)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();


            var cmd = new NpgsqlCommand(
                "SELECT username, password, auth_token, coin, elo " +
                "FROM users " +
                "WHERE username = @username", connection);

            DataLayer.AddParameter(cmd, "username", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    reader.GetString(0), // Username
                    reader.GetString(1), // Password
                    reader.IsDBNull(2) ? null : reader.GetString(2), // AuthToken
                    reader.GetInt32(3),  // Coins
                    reader.GetInt32(4)   // Elo
                );
            }

            // return null if no user was found
            return null;
        }        
        public User? GetUserByAuthtoken(string authtoken)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();


            var cmd = new NpgsqlCommand(
                "SELECT username, password, auth_token, coin, elo " +
                "FROM users " +
                "WHERE auth_token = @auth_token", connection);

            DataLayer.AddParameter(cmd, "auth_token", authtoken);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    reader.GetString(0), // Username
                    reader.GetString(1), // Password
                    reader.IsDBNull(2) ? null : reader.GetString(2), // AuthToken
                    reader.GetInt32(3),  // Coins
                    reader.GetInt32(4)   // Elo
                );
            }

            // return null if no user was found
            return null;
        }
        public void UpdateUser(User user)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "UPDATE users " +
                "SET password = @password, auth_token = @auth_token, coin = @coin, elo = @elo " +
                "WHERE username = @username", connection);

            DataLayer.AddParameter(cmd, "username", user.Username);
            DataLayer.AddParameter(cmd, "password", user.Password);
            DataLayer.AddParameter(cmd, "auth_token", user.AuthToken ?? (object)DBNull.Value);
            DataLayer.AddParameter(cmd, "coin", user.Coins);
            DataLayer.AddParameter(cmd, "elo", user.Elo);

            cmd.ExecuteNonQuery();
        }
        public void DeleteUser(string username)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var cmd = new NpgsqlCommand(
                "DELETE FROM users " +
                "WHERE username = @username", connection);

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

        public bool IsCardInUserStack(User user, Guid cardId)
        {
            // TODO: Implement stack-related logic with the Cards table.
            return true;
        }
    }
}
