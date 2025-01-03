namespace MTCG.Utilities.Exceptions.CustomExceptions
{
    [Serializable]
    internal class TradeAlreadyExistsException : Exception
    {
        public TradeAlreadyExistsException()
        {
        }

        public TradeAlreadyExistsException(string? message) : base(message)
        {
        }

        public TradeAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}