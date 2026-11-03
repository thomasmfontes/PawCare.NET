using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PawCareApi.Application.DTOs.Tutores;
using PawCareApi.Tests.Integration.Fixtures;

namespace PawCareApi.Tests.Integration.Endpoints;

[Collection("IntegrationTestCollection")]
public class TutoresEndpointTests
{
    private readonly HttpClient _client;

    public TutoresEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> ObterTokenJwtAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "Admin@123" });
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.GetProperty("data").GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task GetAll_QuandoChamado_DeveRetornarStatus200ComPageResult()
    {
        // Act
        var response = await _client.GetAsync("/api/tutores?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        root.GetProperty("success").GetBoolean().Should().BeTrue();
        var data = root.GetProperty("data");
        data.GetProperty("items").EnumerateArray().Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByCpf_QuandoCpfExiste_DeveRetornarStatus200ComTutorEHateoas()
    {
        // Act
        var response = await _client.GetAsync("/api/tutores/11122233344");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var data = doc.RootElement.GetProperty("data");

        data.GetProperty("cpf").GetString().Should().Be("11122233344");
        data.GetProperty("nome").GetString().Should().Be("Tutor Teste Integracao");

        var links = data.GetProperty("links").EnumerateArray().ToList();
        links.Should().NotBeEmpty();
        links.Should().Contain(l => l.GetProperty("rel").GetString() == "self");
        links.Should().Contain(l => l.GetProperty("rel").GetString() == "pets");
    }

    [Fact]
    public async Task GetByCpf_QuandoCpfInexistente_DeveRetornarStatus404NotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/tutores/99988877766");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
