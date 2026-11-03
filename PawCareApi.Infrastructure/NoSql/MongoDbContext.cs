using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Infrastructure.NoSql
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase? _database;
        private readonly MongoDbSettings _settings;
        public bool IsConnected { get; }

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;

            try
            {
                var mongoUrl = new MongoUrl(_settings.ConnectionString);
                var clientSettings = MongoClientSettings.FromUrl(mongoUrl);
                clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(2);
                clientSettings.ConnectTimeout = TimeSpan.FromSeconds(2);

                var client = new MongoClient(clientSettings);
                _database = client.GetDatabase(_settings.DatabaseName);
                IsConnected = true;
            }
            catch
            {
                IsConnected = false;
            }
        }

        public IMongoCollection<ProntuarioNoSql>? Prontuarios
            => _database?.GetCollection<ProntuarioNoSql>(_settings.ProntuariosCollectionName);

        public async Task<bool> PingAsync()
        {
            if (_database == null) return false;
            try
            {
                await _database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

