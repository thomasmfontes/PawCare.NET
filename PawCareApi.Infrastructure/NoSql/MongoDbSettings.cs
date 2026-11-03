namespace PawCareApi.Infrastructure.NoSql
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";
        public string DatabaseName { get; set; } = "PawCareNoSqlDb";
        public string ProntuariosCollectionName { get; set; } = "Prontuarios";
    }
}

