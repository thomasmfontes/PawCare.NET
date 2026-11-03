using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PawCareApi.Tests.Integration.Fixtures;

namespace PawCareApi.Tests.Integration.Endpoints;

[Collection("IntegrationTestCollection")]
public class AuthEndpointsTests
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ComCredenciaisValidasAdmin_DeveRetornar200ETokenJwt()
    {
        // Arrange
        var payload = new { username = "admin", password = "Admin@123" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        root.GetProperty("success").GetBoolean().Should().BeTrue();
        var data = root.GetProperty("data");
        data.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
        data.GetProperty("role").GetString().Should().Be("Admin");
        data.GetProperty("tokenType").GetString().Should().Be("Bearer");
    }

    [Fact]
    public async Task Login_ComCredenciaisValidasVeterinario_DeveRetornar200EPerfilVeterinario()
    {
        // Arrange
        var payload = new { username = "vet", password = "Vet@123" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var data = doc.RootElement.GetProperty("data");

        data.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
        data.GetProperty("role").GetString().Should().Be("Veterinario");
    }

    [Fact]
    public async Task Login_ComSenhaIncorreta_DeveRetornar401Unauthorized()
    {
        // Arrange
        var payload = new { username = "admin", password = "SenhaErrada_999" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

