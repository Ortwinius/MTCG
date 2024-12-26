namespace MTCG.Utilities.CustomExceptions
{
    [Serializable]
    internal class DbTransactionException : Exception
    {
        public DbTransactionException()
        {
        }

        public DbTransactionException(string? message) : base(message)
        {
        }

        public DbTransactionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}