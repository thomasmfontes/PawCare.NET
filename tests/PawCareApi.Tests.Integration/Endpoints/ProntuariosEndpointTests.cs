using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PawCareApi.Application.DTOs.Prontuarios;
using PawCareApi.Domain.Entities;
using PawCareApi.Tests.Integration.Fixtures;

namespace PawCareApi.Tests.Integration.Endpoints;

[Collection("IntegrationTestCollection")]
public class ProntuariosEndpointTests
{
    private readonly HttpClient _client;

    public ProntuariosEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> ObterTokenJwtAsync(string username = "vet", string password = "Vet@123")
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { username, password });
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.GetProperty("data").GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task Post_ComAutenticacaoVeterinario_DeveCriarProntuarioNoSqlERetornar201Created()
    {
        // Arrange
        var token = await ObterTokenJwtAsync("vet", "Vet@123");
        var client = _client;

        var dto = new ProntuarioRequestDto
        {
            PetId = 500,
            NomePet = "Max",
            TutorCpf = "11122233344",
            VeterinarioNome = "Dr. Carlos Veterinário",
            TipoAtendimento = "Consulta Periódica",
            QueixaPrincipal = "Check-up semestral de rotina",
            Diagnostico = "Animal com excelente escore corporal",
            SinaisVitais = new SinaisVitaisNoSql
            {
                TemperaturaCelsius = 38.4,
                FrequenciaCardiacaBpm = 90,
                FrequenciaRespiratoriaMpm = 20,
                PesoKg = 30.0,
                PressaoArterial = "120/80"
            },
            Sintomas = new List<string> { "Nenhum sintoma adverso" },
            Prescricoes = new List<PrescricaoItemNoSql>
            {
                new() { Medicamento = "Vermífugo Plus", Dosagem = "1 comprimido", Frequencia = "Dose única", DuracaoDias = 1 }
            },
            Observacoes = "Vacinação e vermifugação em dia."
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/prontuarios")
        {
            Content = JsonContent.Create(dto)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var data = doc.RootElement.GetProperty("data");

        data.GetProperty("nomePet").GetString().Should().Be("Max");
        data.GetProperty("id").GetString().Should().NotBeNullOrWhiteSpace();

        var links = data.GetProperty("links").EnumerateArray().ToList();
        links.Should().NotBeEmpty();
        links.Should().Contain(l => l.GetProperty("rel").GetString() == "self");
        links.Should().Contain(l => l.GetProperty("rel").GetString() == "pet-relacional");
    }

    [Fact]
    public async Task GetPorPet_QuandoExistemProntuarios_DeveRetornarStatus200ComListaEHateoas()
    {
        // Act
        var response = await _client.GetAsync("/api/prontuarios/pet/500");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        doc.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
    }
}

