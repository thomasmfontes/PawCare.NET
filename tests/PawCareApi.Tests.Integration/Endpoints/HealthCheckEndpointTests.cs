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
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
        content.Should().NotBeNullOrWhiteSpace();

        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        root.TryGetProperty("status", out var statusProp).Should().BeTrue();
        statusProp.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetHealthLive_QuandoApiEstiverEmExecucao_DeveRetornarStatus200OkEHealthyComTagLive()
    {
        // Arrange (Correção do apontamento da Sprint 3: Self check com tag 'live')
        var endpoint = "/health/live";

        // Act
        var response = await _client.GetAsync(endpoint);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNullOrWhiteSpace();

        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        root.TryGetProperty("status", out var statusProp).Should().BeTrue();
        statusProp.GetString().Should().Be("Healthy");

        // Verifica que há registros vinculados à tag live (self check)
        root.TryGetProperty("entries", out var entriesProp).Should().BeTrue();
        var entries = entriesProp.EnumerateArray().ToList();
        entries.Should().NotBeEmpty();
        entries.Should().Contain(e => e.GetProperty("component").GetString() == "self");
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
