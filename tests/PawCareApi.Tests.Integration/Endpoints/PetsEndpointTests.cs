using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PawCareApi.Models;
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

    [Fact]
    public async Task GetAll_QuandoChamado_DeveRetornarStatus200ComListaDePets()
    {
        // Arrange
        var requestUri = "/api/pets";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var pets = await response.Content.ReadFromJsonAsync<List<Pet>>();
        pets.Should().NotBeNull();
        pets!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_QuandoIdExiste_DeveRetornarStatus200ComDadosDoPet()
    {
        // Arrange
        var requestUri = "/api/pets/500";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var pet = await response.Content.ReadFromJsonAsync<Pet>();
        pet.Should().NotBeNull();
        pet!.Id.Should().Be(500);
        pet.Nome.Should().Be("Max");
    }

    [Fact]
    public async Task GetById_QuandoIdInexistente_DeveRetornarStatus404NotFound()
    {
        // Arrange
        var requestUri = "/api/pets/999999";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_QuandoPayloadValido_DeveRetornarStatus201CreatedComHeaderLocation()
    {
        // Arrange
        var novoPet = new Pet
        {
            Nome = "Barthô",
            DataNascimento = new DateTime(2021, 6, 15),
            Peso = 8.5,
            StatusLongevidade = "Adulto",
            TutorCpf = "11122233344",
            RacaId = 100
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pets", novoPet);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var petCriado = await response.Content.ReadFromJsonAsync<Pet>();
        petCriado.Should().NotBeNull();
        petCriado!.Nome.Should().Be("Barthô");
        petCriado.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Create_QuandoTutorInexistente_DeveRetornarStatus400BadRequest()
    {
        // Arrange
        var petInvalido = new Pet
        {
            Nome = "Fantasma",
            DataNascimento = DateTime.UtcNow,
            Peso = 5.0,
            StatusLongevidade = "Filhote",
            TutorCpf = "00000000000", // CPF não cadastrado
            RacaId = 100
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pets", petInvalido);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var mensagem = await response.Content.ReadAsStringAsync();
        mensagem.Should().Contain("Tutor informado não existe.");
    }

    [Fact]
    public async Task Delete_QuandoPetExiste_DeveRetornarStatus204NoContent()
    {
        // Arrange - Primeiro cria um pet para garantir que podemos excluí-lo
        var petParaExcluir = new Pet
        {
            Nome = "Pet Temporário",
            DataNascimento = DateTime.UtcNow,
            Peso = 4.2,
            StatusLongevidade = "Filhote",
            TutorCpf = "11122233344",
            RacaId = 100
        };

        var postResponse = await _client.PostAsJsonAsync("/api/pets", petParaExcluir);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var petCriado = await postResponse.Content.ReadFromJsonAsync<Pet>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/pets/{petCriado!.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verifica que o pet foi de fato removido
        var getResponse = await _client.GetAsync($"/api/pets/{petCriado.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
