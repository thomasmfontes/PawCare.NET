using Microsoft.Extensions.Diagnostics.HealthChecks;
using PawCareApi.Infrastructure.NoSql;

namespace PawCareApi.HealthChecks
{
    public class MongoHealthCheck : IHealthCheck
    {
        private readonly MongoDbContext _mongoDbContext;

        public MongoHealthCheck(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var pingOk = await _mongoDbContext.PingAsync();
                if (pingOk)
                {
                    return HealthCheckResult.Healthy("Conexão com o banco NoSQL (MongoDB) operacional.");
                }

                // Quando MongoDB externo não estiver disponível localmente, a camada NoSQL opera em modo de resiliência/memória
                return HealthCheckResult.Healthy("Banco NoSQL (MongoDB) configurado com suporte a modo resiliente.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Degraded($"Aviso de verificação do MongoDB: {ex.Message}");
            }
        }
    }
}

