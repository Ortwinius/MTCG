namespace MTCG.Utilities.Exceptions.CustomExceptions
{
    [Serializable]
    internal class InvalidDeckSizeException : Exception
    {
        public InvalidDeckSizeException()
        {
        }

        public InvalidDeckSizeException(string? message) : base(message)
        {
        }

        public InvalidDeckSizeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}