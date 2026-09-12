using System.Diagnostics.Metrics;

namespace PawCareApi.Diagnostics;

public static class PawCareMetrics
{
    public const string MeterName = "PawCareApi.Metrics";
    private static readonly Meter Meter = new(MeterName, "1.0.0");

    // Contador de requisições processadas
    public static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>(
        "pawcare_requests_total",
        description: "Contagem total de requisições processadas pela PawCare API");

    // Contador de erros ocorridos
    public static readonly Counter<long> ErrorCounter = Meter.CreateCounter<long>(
        "pawcare_errors_total",
        description: "Contagem total de erros ou falhas em requisições na PawCare API");

    // Histograma de tempo de processamento/resposta de operações de negócio
    public static readonly Histogram<double> ResponseDuration = Meter.CreateHistogram<double>(
        "pawcare_operation_duration_seconds",
        unit: "s",
        description: "Duração das operações da PawCare API em segundos");
}
