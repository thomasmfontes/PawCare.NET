using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PawCareApi.Application.DTOs.Pets;
using PawCareApi.Tests.Integration.Fixtures;

namespace PawCareApi.Tests.Integration.Endpoints;

[Collection("IntegrationTestCollection")]
public class PetsEndpointTests
{
    private readonly HttpClient _client;

    public PetsEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> ObterTokenJwtAsync(string username = "admin", string password = "Admin@123")
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { username, password });
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.GetProperty("data").GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task GetAll_QuandoChamado_DeveRetornarStatus200ComPageResultEHateoas()
    {
        // Act
        var response = await _client.GetAsync("/api/pets?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        root.GetProperty("success").GetBoolean().Should().BeTrue();
        var data = root.GetProperty("data");

        data.GetProperty("pageNumber").GetInt32().Should().Be(1);
        data.GetProperty("pageSize").GetInt32().Should().Be(10);
        data.GetProperty("totalItems").GetInt32().Should().BeGreaterThan(0);

        var items = data.GetProperty("items").EnumerateArray().ToList();
        items.Should().NotBeEmpty();

        var primeiroPet = items.First();
        primeiroPet.TryGetProperty("links", out var linksProp).Should().BeTrue();
        var links = linksProp.EnumerateArray().ToList();
        links.Should().NotBeEmpty();
        links.Should().Contain(l => l.GetProperty("rel").GetString() == "self");
        links.Should().Contain(l => l.GetProperty("rel").GetString() == "prontuarios-nosql");
    }

    [Fact]
    public async Task GetById_QuandoIdExiste_DeveRetornarStatus200ComDadosDoPetEHateoas()
    {
        // Act
        var response = await _client.GetAsync("/api/pets/500");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var data = doc.RootElement.GetProperty("data");

        data.GetProperty("id").GetInt64().Should().Be(500);
        data.GetProperty("nome").GetString().Should().Be("Max");

        var links = data.GetProperty("links").EnumerateArray().ToList();
        links.Should().NotBeEmpty();
        links.Should().Contain(l => l.GetProperty("rel").GetString() == "self");
    }

    [Fact]
    public async Task GetById_QuandoIdInexistente_DeveRetornarStatus404NotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/pets/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_SemAutenticacao_DeveRetornar401Unauthorized()
    {
        // Arrange
        var novoPet = new PetRequestDto
        {
            Nome = "Sem Auth",
            DataNascimento = DateTime.UtcNow.AddYears(-2),
            Peso = 10.0,
            StatusLongevidade = "Adulto",
            RacaId = 100,
            TutorCpf = "11122233344"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pets", novoPet);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ComAutenticacaoAdmin_DeveRetornar201CreatedComHateoas()
    {
        // Arrange
        var token = await ObterTokenJwtAsync("admin", "Admin@123");
        var clientAutenticado = _client;

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/pets")
        {
            Content = JsonContent.Create(new PetRequestDto
            {
                Nome = "Pipoca",
                DataNascimento = DateTime.UtcNow.AddYears(-1),
                Peso = 8.5,
                StatusLongevidade = "Adulto",
                RacaId = 100,
                TutorCpf = "11122233344"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await clientAutenticado.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var data = doc.RootElement.GetProperty("data");

        data.GetProperty("nome").GetString().Should().Be("Pipoca");
        data.GetProperty("links").EnumerateArray().Should().NotBeEmpty();
    }
}
