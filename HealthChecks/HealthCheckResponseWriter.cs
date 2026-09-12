using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace PawCareApi.HealthChecks;

public static class HealthCheckResponseWriter
{
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = $"{report.TotalDuration.TotalMilliseconds:F2}ms",
            timestamp = DateTime.UtcNow,
            entries = report.Entries.Select(e => new
            {
                component = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = $"{e.Value.Duration.TotalMilliseconds:F2}ms",
                exception = e.Value.Exception?.Message,
                data = e.Value.Data
            })
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return context.Response.WriteAsync(json);
    }
}
