using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PawCareApi.Controllers;
using PawCareApi.Models;
using PawCareApi.Tests.Unit.Fixtures;

namespace PawCareApi.Tests.Unit.Controllers;

[Collection("UnitTestCollection")]
public class PetsControllerTests
{
    private readonly TestDbContextFixture _fixture;

    public PetsControllerTests(TestDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAll_QuandoExistemPets_DeveRetornarOkComListaDePets()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var tutor = new Tutor { Cpf = "11111111111", Nome = "João Silva", Email = "joao@email.com", Telefone = "11911111111" };
        var raca = new Raca { Id = 1, Nome = "Labrador", Especie = "Canina" };
        var pet = new Pet { Id = 10, Nome = "Bob", TutorCpf = tutor.Cpf, RacaId = raca.Id, StatusLongevidade = "Adulto" };

        context.Tutores.Add(tutor);
        context.Racas.Add(raca);
        context.Pets.Add(pet);
        await context.SaveChangesAsync();

        var controller = new PetsController(context);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var pets = okResult.Value as IEnumerable<Pet>;
        pets.Should().NotBeNull();
        pets!.Should().Contain(p => p.Nome == "Bob");
    }

    [Fact]
    public async Task GetById_QuandoPetExiste_DeveRetornarOkComPet()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var tutor = new Tutor { Cpf = "22222222222", Nome = "Ana Paula", Email = "ana@email.com", Telefone = "11922222222" };
        var raca = new Raca { Id = 2, Nome = "Siamês", Especie = "Felina" };
        var pet = new Pet { Id = 20, Nome = "Mia", TutorCpf = tutor.Cpf, RacaId = raca.Id, StatusLongevidade = "Filhote" };

        context.Tutores.Add(tutor);
        context.Racas.Add(raca);
        context.Pets.Add(pet);
        await context.SaveChangesAsync();

        var controller = new PetsController(context);

        // Act
        var result = await controller.GetById(20);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var petRetornado = okResult.Value as Pet;
        petRetornado.Should().NotBeNull();
        petRetornado!.Nome.Should().Be("Mia");
    }

    [Fact]
    public async Task GetById_QuandoPetNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var controller = new PetsController(context);

        // Act
        var result = await controller.GetById(999);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Pet não encontrado.");
    }

    [Fact]
    public async Task Create_QuandoTutorNaoExiste_DeveRetornarBadRequest()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var pet = new Pet { Nome = "Toby", TutorCpf = "99999999999", RacaId = 1 };
        var controller = new PetsController(context);

        // Act
        var result = await controller.Create(pet);

        // Assert
        var badRequest = result.Result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value.Should().Be("Tutor informado não existe.");
    }

    [Fact]
    public async Task Create_QuandoDadosValidos_DeveRetornarCreatedEIncrementarQtdPetsDoTutor()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var tutor = new Tutor { Cpf = "33333333333", Nome = "Roberto", Email = "roberto@email.com", Telefone = "11933333333", QtdPets = 0 };
        var raca = new Raca { Id = 3, Nome = "Poodle", Especie = "Canina" };
        context.Tutores.Add(tutor);
        context.Racas.Add(raca);
        await context.SaveChangesAsync();

        var controller = new PetsController(context);
        var novoPet = new Pet
        {
            Nome = "Pipoca",
            TutorCpf = tutor.Cpf,
            RacaId = raca.Id,
            StatusLongevidade = "Filhote"
        };

        // Act
        var result = await controller.Create(novoPet);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var petCriado = createdResult.Value as Pet;
        petCriado.Should().NotBeNull();
        petCriado!.Nome.Should().Be("Pipoca");

        // Valida que o tutor teve a quantidade de pets incrementada
        var tutorAtualizado = await context.Tutores.FindAsync(tutor.Cpf);
        tutorAtualizado!.QtdPets.Should().Be(1);
    }

    [Fact]
    public async Task Delete_QuandoPetExiste_DeveRetornarNoContentEDecrementarQtdPets()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var tutor = new Tutor { Cpf = "44444444444", Nome = "Fernanda", Email = "fer@email.com", Telefone = "11944444444", QtdPets = 1 };
        var raca = new Raca { Id = 4, Nome = "Beagle", Especie = "Canina" };
        var pet = new Pet { Id = 40, Nome = "Snoopy", TutorCpf = tutor.Cpf, RacaId = raca.Id, StatusLongevidade = "Adulto" };

        context.Tutores.Add(tutor);
        context.Racas.Add(raca);
        context.Pets.Add(pet);
        await context.SaveChangesAsync();

        var controller = new PetsController(context);

        // Act
        var result = await controller.Delete(40);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);

        var petRemovido = await context.Pets.FindAsync(40L);
        petRemovido.Should().BeNull();

        var tutorAtualizado = await context.Tutores.FindAsync(tutor.Cpf);
        tutorAtualizado!.QtdPets.Should().Be(0);
    }
}
