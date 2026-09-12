using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PawCareApi.Models;
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

    [Fact]
    public async Task GetAll_QuandoChamado_DeveRetornarStatus200ComListaDeTutores()
    {
        // Arrange
        var requestUri = "/api/tutores";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tutores = await response.Content.ReadFromJsonAsync<List<Tutor>>();
        tutores.Should().NotBeNull();
        tutores!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByCpf_QuandoCpfExiste_DeveRetornarStatus200ComTutor()
    {
        // Arrange
        var requestUri = "/api/tutores/11122233344";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tutor = await response.Content.ReadFromJsonAsync<Tutor>();
        tutor.Should().NotBeNull();
        tutor!.Cpf.Should().Be("11122233344");
    }

    [Fact]
    public async Task GetByCpf_QuandoCpfInexistente_DeveRetornarStatus404NotFound()
    {
        // Arrange
        var requestUri = "/api/tutores/99988877766";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_QuandoCpfDuplicado_DeveRetornarStatus400BadRequest()
    {
        // Arrange
        var tutorDuplicado = new Tutor
        {
            Cpf = "11122233344", // Já cadastrado pelo seed
            Nome = "Clone",
            Email = "clone@email.com",
            Telefone = "11999990000"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tutores", tutorDuplicado);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Já existe um tutor cadastrado com este CPF.");
    }
}
