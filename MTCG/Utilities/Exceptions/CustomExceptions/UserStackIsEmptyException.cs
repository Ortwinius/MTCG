namespace MTCG.Utilities.Exceptions.CustomExceptions
{
    [Serializable]
    internal class UserStackIsEmptyException : Exception
    {
        public UserStackIsEmptyException()
        {
        }

        public UserStackIsEmptyException(string? message) : base(message)
        {
        }

        public UserStackIsEmptyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}