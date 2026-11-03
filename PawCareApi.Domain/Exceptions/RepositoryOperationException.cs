namespace PawCareApi.Domain.Exceptions
{
    public class RepositoryOperationException : Exception
    {
        public string Operation { get; }

        public RepositoryOperationException(string operation, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            Operation = operation;
        }
    }
}

