using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PawCareApi.HealthChecks;

public class ExternalServicesHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;

    public ExternalServicesHealthCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simulação / Verificação de conectividade com serviço externo de telemetria/notificações
            // Usamos timeout curto para não impactar o health check
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(2));

            // Verifica disponibilidade geral de rede/serviço externo
            var response = await _httpClient.GetAsync("https://httpbin.org/status/200", cts.Token);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Serviço externo de notificações/integração respondendo com sucesso.");
            }

            return HealthCheckResult.Degraded($"Serviço externo retornou status code {response.StatusCode}.");
        }
        catch (Exception ex)
        {
            // Caso o ambiente local/FIAP esteja sem acesso à internet, reporta Degraded com aviso em vez de derrubar a aplicação
            return HealthCheckResult.Degraded(
                "Serviço externo temporariamente indisponível ou sem conectividade com a internet.",
                ex);
        }
    }
}
