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

            var command = new NpgsqlCommand(
                "INSERT INTO users (username, password) " +
                "VALUES (@username, @password)", connection);

            DataLayer.AddParameterWithValue(command, "username", NpgsqlDbType.Varchar, user.Username);
            DataLayer.AddParameterWithValue(command, "password", NpgsqlDbType.Varchar, user.Password);
            
            command.ExecuteNonQuery();
        }
        public string? GetAuthTokenByUsername(string username)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "SELECT auth_token FROM users " +
                "WHERE username = @username", connection);

            DataLayer.AddParameterWithValue(command, "username", NpgsqlDbType.Varchar, username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetString(0);
            }

            return null;
        }
        public User? GetUserByUsername(string username)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();


            var command = new NpgsqlCommand(
                "SELECT username, password, auth_token, coin, elo FROM users " +
                "WHERE username = @username", connection);

            // TODO could be prone to error ? 
            DataLayer.AddParameterWithValue(command, "username", NpgsqlDbType.Varchar, username);

            using var reader = command.ExecuteReader();
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

            var command = new NpgsqlCommand(
                "UPDATE users " +
                "SET password = @password, auth_token = @auth_token, coin = @coin, elo = @elo " +
                "WHERE username = @username", connection);

            DataLayer.AddParameterWithValue(command, "username", NpgsqlDbType.Varchar, user.Username);
            DataLayer.AddParameterWithValue(command, "password", NpgsqlDbType.Varchar, user.Password);
            DataLayer.AddParameterWithValue(command, "auth_token", NpgsqlDbType.Varchar, user.AuthToken ?? (object)DBNull.Value);
            DataLayer.AddParameterWithValue(command, "coin", NpgsqlDbType.Integer, user.Coins);
            DataLayer.AddParameterWithValue(command, "elo", NpgsqlDbType.Integer, user.Elo);

            command.ExecuteNonQuery();
        }
        public void DeleteUser(string username)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "DELETE FROM users " +
                "WHERE username = @username", connection);

            command.Parameters.AddWithValue("username", username);

            command.ExecuteNonQuery();
        }

        public bool UserExists(string username)
        {
            return GetUserByUsername(username) != null;
        }

        public bool IsCardInUserStack(User user, Guid cardId)
        {
            // TODO: Implement stack-related logic with the Cards table.
            return true;
        }
    }
}
