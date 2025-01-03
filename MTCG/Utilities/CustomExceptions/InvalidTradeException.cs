﻿
namespace MTCG.BusinessLogic.Services
{
    [Serializable]
    internal class InvalidTradeException : Exception
    {
        public InvalidTradeException()
        {
        }

        public InvalidTradeException(string? message) : base(message)
        {
        }

        public InvalidTradeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}