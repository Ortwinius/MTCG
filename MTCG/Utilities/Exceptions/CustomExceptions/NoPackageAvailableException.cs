namespace MTCG.Utilities.Exceptions.CustomExceptions
{
    [Serializable]
    internal class NoPackageAvailableException : Exception
    {
        public NoPackageAvailableException()
        {
        }

        public NoPackageAvailableException(string? message) : base(message)
        {
        }

        public NoPackageAvailableException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}