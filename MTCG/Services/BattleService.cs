﻿using MTCG.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
Singleton BattleService 
is a container for handling fights between cards
If userA attacks user B => get one random card of userA's and userB's userDeck
Those two cards fight each other depending on cardType(Monster/Spell), damage & effectivity
calculateHigherDamage() => checks who deals more damage and returns card
He who deals more damage wins and gets the card of the opponent - the opponent loses it
*/
namespace MTCG.Services
{
    // TODO : change constructor to private and add GetInstance
    public class BattleService
    {
        private static BattleService _instance;
        private readonly UserRepository _userRepository;


        public const int MaxBattleRounds = 100;
        public int currentRound { get; private set; } = 1;
        public BattleService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public static BattleService GetInstance(UserRepository userRepository)
        {
            if(_instance == null)
            {
                _instance = new BattleService(userRepository);
            }
            return _instance;
        }


    }
}
