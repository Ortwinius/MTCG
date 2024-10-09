﻿using MTCG.Models.Card;
using MTCG.Models.Card.Monster;
using MTCG.BusinessLogic;
using System;
using System.Net.WebSockets;

namespace MTCG.Models.Users
{
    public class User
    {

        #region Setup

        private int _coins; // default 
        private int _elo = 100; // default 
        private const int _maxDeckSize = 4; // default
        private List<ICard> _stack;
        private List<ICard> _deck;

        // TODO : change username & AuthToken & IsLoggedIn
        // setter to private <-> in conflict with authservice!
        public string Username { get; set; }
        public string HashedPassword { get; }
        public string AuthToken { get; set; }
        public bool IsLoggedIn { get; set; }
        public int Coins { get; private set; } = 20;
        public List<ICard> Stack { get; set; }
        public List<ICard> Deck { get; set; }
        // base constructor
        public User()
        {
            Username = "";
            HashedPassword = "";
            AuthToken = "";
            Stack = new List<ICard>();
            Deck = new List<ICard>();
            IsLoggedIn = false;
        }
        // TODO : make logic applicable that AuthService can be performed on this constructor?
        public User(string username, string hashedPassword, string authToken)
        {
            Username = username;
            HashedPassword = hashedPassword;
            AuthToken = authToken.ToString();
            Stack = new List<ICard>();
            Deck = new List<ICard>();
            IsLoggedIn = false;
        }
        #endregion

        #region Info
        // TODO Get User Info : Name, (opt) description : Http "GET users/{username}
        // 1. 200 OK
        // 2. 401 Unauthorized (only user or admin can do that)
        // 3. 404 Not found

        public void ShowUserInfo()
        {
            //if (!validateAction()) return;

            // TODO
            // Shows Elo, Coins, Wins, Losses

        }

        #endregion
    }
}
