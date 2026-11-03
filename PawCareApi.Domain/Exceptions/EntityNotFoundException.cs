namespace PawCareApi.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string EntityName { get; }
        public object? Key { get; }

        public EntityNotFoundException(string entityName, object? key)
            : base($"Entidade '{entityName}' com identificador '{key}' não foi encontrada.")
        {
            EntityName = entityName;
            Key = key;
        }

        public EntityNotFoundException(string message) : base(message)
        {
            EntityName = "Entidade";
        }
    }
}

