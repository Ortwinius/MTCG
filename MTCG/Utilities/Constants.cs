﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Utilities
{
    public static class Constants
    {
        public const string ServerUrl = "http://localhost:8000/";
        public const int ServerPort = 10001;
        public const int BattleQueueTimeout = 10;

        public const int MaxBattleRounds = 100;
        public const int StartElo = 100;
        public const int DeckSize = 4;
        public const int MaxStackSize = 20;
    }
}
