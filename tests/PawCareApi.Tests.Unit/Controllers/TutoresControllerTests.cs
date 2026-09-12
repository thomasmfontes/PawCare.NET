using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PawCareApi.Controllers;
using PawCareApi.Models;
using PawCareApi.Tests.Unit.Fixtures;

namespace PawCareApi.Tests.Unit.Controllers;

[Collection("UnitTestCollection")]
public class TutoresControllerTests
{
    private readonly TestDbContextFixture _fixture;

    public TutoresControllerTests(TestDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_QuandoCpfJaCadastrado_DeveRetornarBadRequest()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var tutorExistente = new Tutor { Cpf = "55555555555", Nome = "Marcos", Email = "marcos@email.com", Telefone = "11955555555" };
        context.Tutores.Add(tutorExistente);
        await context.SaveChangesAsync();

        var controller = new TutoresController(context);
        var novoTutor = new Tutor { Cpf = "55555555555", Nome = "Marcos Duplicado", Email = "outro@email.com", Telefone = "11999999999" };

        // Act
        var result = await controller.Create(novoTutor);

        // Assert
        var badRequest = result.Result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value.Should().Be("Já existe um tutor cadastrado com este CPF.");
    }

    [Fact]
    public async Task Create_QuandoDadosValidos_DeveRetornarCreated()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var controller = new TutoresController(context);
        var novoTutor = new Tutor { Cpf = "66666666666", Nome = "Juliana", Email = "juliana@email.com", Telefone = "11966666666" };

        // Act
        var result = await controller.Create(novoTutor);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var tutorCriado = createdResult.Value as Tutor;
        tutorCriado.Should().NotBeNull();
        tutorCriado!.Cpf.Should().Be("66666666666");
    }

    [Fact]
    public async Task GetByEmail_QuandoEmailNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var controller = new TutoresController(context);

        // Act
        var result = await controller.GetByEmail("inexistente@email.com");

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Tutor não encontrado.");
    }

    [Fact]
    public async Task Delete_QuandoTutorPossuiPets_DeveRetornarBadRequest()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var tutor = new Tutor { Cpf = "77777777777", Nome = "Camila", Email = "camila@email.com", Telefone = "11977777777" };
        var pet = new Pet { Id = 70, Nome = "Bidu", TutorCpf = tutor.Cpf, RacaId = 1 };
        tutor.Pets.Add(pet);

        context.Tutores.Add(tutor);
        await context.SaveChangesAsync();

        var controller = new TutoresController(context);

        // Act
        var result = await controller.Delete("77777777777");

        // Assert
        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value.Should().Be("Não é possível remover um tutor que possui pets cadastrados.");
    }

    [Fact]
    public async Task Delete_QuandoTutorNaoPossuiPets_DeveRetornarNoContent()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var tutor = new Tutor { Cpf = "88888888888", Nome = "Lucas", Email = "lucas@email.com", Telefone = "11988888888" };
        context.Tutores.Add(tutor);
        await context.SaveChangesAsync();

        var controller = new TutoresController(context);

        // Act
        var result = await controller.Delete("88888888888");

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);

        var tutorExcluido = await context.Tutores.FindAsync("88888888888");
        tutorExcluido.Should().BeNull();
    }
}
