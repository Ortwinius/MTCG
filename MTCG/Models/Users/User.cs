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

        // TODO : change username & AuthToken & IsLoggedIn
        // setter to private <-> in conflict with authservice!
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        public string? AuthToken { get; set; }
        public bool IsLoggedIn { get; set; }
        public int Coins { get; set; } = 20;
        public List<ICard>? Stack { get; set; }
        public List<ICard>? Deck { get; set; }
        [JsonConstructor]
        public User(string username, string password)
        {
            Username = username;
            Password = password;
            AuthToken = "";
            Stack = new List<ICard>();
            Deck = new List<ICard>();
            IsLoggedIn = false;
        }
        #endregion
    }
}
