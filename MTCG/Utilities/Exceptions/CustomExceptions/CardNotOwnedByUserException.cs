namespace MTCG.Utilities.Exceptions.CustomExceptions
{
    [Serializable]
    internal class CardNotOwnedByUserException : Exception
    {
        public CardNotOwnedByUserException()
        {
        }

        public CardNotOwnedByUserException(string? message) : base(message)
        {
        }

        public CardNotOwnedByUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}