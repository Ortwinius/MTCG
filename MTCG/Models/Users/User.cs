using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.BusinessLogic;
using System;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

namespace MTCG.Models.Users
{
    public class User
    {

        #region Setup
        //[JsonPropertyName("Id")]
        public int UserId { get; set; }
        [JsonPropertyName("Username")]
        public string Username { get; set; }
        [JsonPropertyName("Password")]
        public string Password { get; set; }
        public string? AuthToken { get; set; }
        public bool IsLoggedIn { get; set; } = false;
        public int Coins { get; set; } = 20;
        public int Elo { get; set; } = 100;
        public Dictionary<string, ICard>? Stack { get; set; }
        public Dictionary<string, ICard>? Deck { get; set; }
        [JsonConstructor]
        public User(string username, string password)
        {
            Username = username;
            Password = password;
            AuthToken = "";
            Stack = new();
            Deck = new();
            IsLoggedIn = false;
        }
        // for db
        public User(string username, string password, string? authToken, int coins, int elo)
        {
            Username = username;
            Password = password;
            AuthToken = authToken;
            Coins = coins;
            Elo = elo;
            Stack = new();
            Deck = new();
        }        
        public User(int userId, string username, string password, string? authToken, int coins, int elo)
        {
            UserId = userId;
            Username = username;
            Password = password;
            AuthToken = authToken;
            Coins = coins;
            Elo = elo;
            Stack = new();
            Deck = new();
        }
        #endregion
    }
}
