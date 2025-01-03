namespace MTCG.Utilities.Exceptions.CustomExceptions
{
    [Serializable]
    internal class TradeNotFoundException : Exception
    {
        public TradeNotFoundException()
        {
        }

        public TradeNotFoundException(string? message) : base(message)
        {
        }

        public TradeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}