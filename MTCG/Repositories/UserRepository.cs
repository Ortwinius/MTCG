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
                "INSERT INTO Users (Username, Password) " +
                "VALUES (@Username, @Password)", connection);

            DataLayer.AddParameterWithValue(command, "Username", NpgsqlDbType.Varchar, user.Username);
            DataLayer.AddParameterWithValue(command, "Password", NpgsqlDbType.Varchar, user.Password);
            //                                                                                                                                                                                                                                                                                                                                                                         .AddParameterWithValue(command, "Coin", NpgsqlDbType.Integer, user.Coins);
            //DataLayer.AddParameterWithValue(command, "Elo", NpgsqlDbType.Integer, user.Elo);

            command.ExecuteNonQuery();
        }

        public User? GetUserByUsername(string username)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "SELECT Username, Password, Coin, Elo FROM Users " +
                "WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("Username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    reader.GetString(0), // Username
                    reader.GetString(1), // Password
                    reader.GetInt32(2),  // Coins
                    reader.GetInt32(3)   // Elo
                );
            }

            return null;
        }

        public void UpdateUser(User user)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "UPDATE Users " +
                "SET Password = @Password, Coin = @Coin, Elo = @Elo " +
                "WHERE Username = @Username", connection);

            DataLayer.AddParameterWithValue(command, "Username", NpgsqlDbType.Varchar, user.Username);
            DataLayer.AddParameterWithValue(command, "Password", NpgsqlDbType.Varchar, user.Password);
            DataLayer.AddParameterWithValue(command, "Coin", NpgsqlDbType.Integer, user.Coins);
            DataLayer.AddParameterWithValue(command, "Elo", NpgsqlDbType.Integer, user.Elo);

            command.ExecuteNonQuery();
        }

        public bool UserExists(string username)
        {
            return GetUserByUsername(username) != null;
        }

        public void DeleteUser(string username)
        {
            using var connection = DataLayer.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                "DELETE FROM Users " +
                "WHERE Username = @Username", connection);

            command.Parameters.AddWithValue("Username", username);

            command.ExecuteNonQuery();
        }

        public bool IsCardInUserStack(User user, Guid cardId)
        {
            // TODO: Implement stack-related logic with the Cards table.
            return true;
        }
    }
}
