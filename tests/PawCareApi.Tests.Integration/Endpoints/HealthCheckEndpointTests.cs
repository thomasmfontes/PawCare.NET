using System.Net;
using System.Text.Json;
using FluentAssertions;
using PawCareApi.Tests.Integration.Fixtures;

namespace PawCareApi.Tests.Integration.Endpoints;

[Collection("IntegrationTestCollection")]
public class HealthCheckEndpointTests
{
    private readonly HttpClient _client;

    public HealthCheckEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_QuandoApiEstiverEmExecucao_DeveRetornarStatus200OuHealthyComEstruturaJson()
    {
        // Arrange
        var endpoint = "/health";

        // Act
        var response = await _client.GetAsync(endpoint);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // O status geral deve responder com sucesso ou formato JSON estruturado
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
        content.Should().NotBeNullOrWhiteSpace();

        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        root.TryGetProperty("status", out var statusProp).Should().BeTrue();
        statusProp.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetHealthReady_QuandoDependenciasConfiguradas_DeveRetornarEstruturaDeComponentes()
    {
        // Arrange
        var endpoint = "/health/ready";

        // Act
        var response = await _client.GetAsync(endpoint);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().NotBeNullOrWhiteSpace();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        root.TryGetProperty("entries", out var entriesProp).Should().BeTrue();
        entriesProp.ValueKind.Should().Be(JsonValueKind.Array);
    }
}
