using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PawCareApi.Controllers;
using PawCareApi.Models;
using PawCareApi.Tests.Unit.Fixtures;

namespace PawCareApi.Tests.Unit.Controllers;

[Collection("UnitTestCollection")]
public class ClinicasControllerTests
{
    private readonly TestDbContextFixture _fixture;

    public ClinicasControllerTests(TestDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAtendimento24h_QuandoExistemClinicas24h_DeveRetornarApenasClinicasComAtendimento24h()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var clinica1 = new Clinica { Id = 1, NomeCnpj = "Clinica Central 24h", Atendimento24h = true, Cidade = "São Paulo", Estado = "SP" };
        var clinica2 = new Clinica { Id = 2, NomeCnpj = "Consultório Diurno", Atendimento24h = false, Cidade = "São Paulo", Estado = "SP" };
        context.Clinicas.AddRange(clinica1, clinica2);
        await context.SaveChangesAsync();

        var controller = new ClinicasController(context);

        // Act
        var result = await controller.GetAtendimento24h();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var clinicas24h = okResult.Value as IEnumerable<Clinica>;
        clinicas24h.Should().NotBeNull();
        clinicas24h!.Should().HaveCount(1);
        clinicas24h!.First().NomeCnpj.Should().Be("Clinica Central 24h");
    }

    [Fact]
    public async Task Delete_QuandoClinicaPossuiEventos_DeveRetornarBadRequest()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var clinica = new Clinica { Id = 10, NomeCnpj = "VetVida", Cidade = "São Paulo", Estado = "SP" };
        var evento = new Evento { Id = 1, Tipo = "Consulta", IdClinica = clinica.Id, IdPet = 1, IdTutor = "123", IdMedico = 1 };
        clinica.Eventos.Add(evento);

        context.Clinicas.Add(clinica);
        await context.SaveChangesAsync();

        var controller = new ClinicasController(context);

        // Act
        var result = await controller.Delete(10);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value.Should().Be("Não é possível remover uma clínica vinculada a eventos.");
    }
}
