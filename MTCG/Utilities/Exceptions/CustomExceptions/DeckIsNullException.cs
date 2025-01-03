namespace MTCG.Utilities.Exceptions.CustomExceptions
{
    [Serializable]
    internal class DeckIsNullException : Exception
    {
        public DeckIsNullException()
        {
        }

        public DeckIsNullException(string? message) : base(message)
        {
        }

        public DeckIsNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}