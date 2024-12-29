using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.BusinessLogic;
using System;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

namespace MTCG.Models.Users
{
    /*
     User Constructor
     */
    public class User
    {

        #region UserCredentials
        public int UserId { get; set; }
        [JsonPropertyName("Username")]
        public string Username { get; set; }
        [JsonPropertyName("Password")]
        public string Password { get; set; }
        public string? AuthToken { get; set; }
        #endregion
        #region UserData
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }
        #endregion
        #region UserStats
        public int Coins { get; set; } = 20;
        public int Elo { get; set; } = 100;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        #endregion

        [JsonConstructor]
        public User(string username, string password)
        {
            Username = username;
            Password = password;
            AuthToken = "";
        }

        /*
         Constructor for whole User object with credentials, data and stats
         */
        public User(int userId, string username, string password, string? authToken, string? name, string? bio, string? image, int coins, int elo, int wins, int losses)
        {
            UserId = userId;
            Username = username;
            Password = password;
            AuthToken = authToken;
            Name = name;
            Bio = bio;
            Image = image;
            Coins = coins;
            Elo = elo;
            Wins = wins;
            Losses = losses;
        }
        /*
         Constructor for only userid, username, password, authtoken and coins
         Used for example for packages and trades
         */
        public User(int userId, string username, string password, string? authToken, int coins)
        {
            UserId = userId;
            Username = username;
            Password = password;
            AuthToken = authToken;
            Coins = coins;
        }
    }
}
