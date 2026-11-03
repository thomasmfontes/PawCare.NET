namespace PawCareApi.Domain.Exceptions
{
    public class DomainValidationException : Exception
    {
        public IReadOnlyList<string> Erros { get; }

        public DomainValidationException(string message) : base(message)
        {
            Erros = new List<string> { message }.AsReadOnly();
        }

        public DomainValidationException(IEnumerable<string> erros)
            : base($"Ocorreram erros de validação de domínio: {string.Join("; ", erros)}")
        {
            Erros = erros.ToList().AsReadOnly();
        }
    }
}
