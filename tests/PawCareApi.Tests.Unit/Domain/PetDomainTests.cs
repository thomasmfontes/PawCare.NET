using FluentAssertions;
using PawCareApi.Models;

namespace PawCareApi.Tests.Unit.Domain;

public class PetDomainTests
{
    [Fact]
    public void InstanciarPet_ComDadosValidos_DeveAtribuirPropriedadesCorretamente()
    {
        // Arrange
        var nome = "Thor";
        var nascimento = new DateTime(2022, 5, 10);
        var peso = 12.5;
        var status = "Adulto";
        var tutorCpf = "12345678901";
        var racaId = 1L;

        // Act
        var pet = new Pet
        {
            Nome = nome,
            DataNascimento = nascimento,
            Peso = peso,
            StatusLongevidade = status,
            TutorCpf = tutorCpf,
            RacaId = racaId
        };

        // Assert
        pet.Nome.Should().Be(nome);
        pet.DataNascimento.Should().Be(nascimento);
        pet.Peso.Should().Be(peso);
        pet.StatusLongevidade.Should().Be(status);
        pet.TutorCpf.Should().Be(tutorCpf);
        pet.RacaId.Should().Be(racaId);
    }

    [Fact]
    public void AssociarRelacionamentos_ComTutorERacaValidos_DeveVincularObjetos()
    {
        // Arrange
        var tutor = new Tutor { Cpf = "12345678901", Nome = "Carlos Silva" };
        var raca = new Raca { Id = 1, Nome = "Golden Retriever", Especie = "Canina" };
        var pet = new Pet { Nome = "Rex" };

        // Act
        pet.Tutor = tutor;
        pet.Raca = raca;

        // Assert
        pet.Tutor.Should().NotBeNull();
        pet.Tutor.Nome.Should().Be("Carlos Silva");
        pet.Raca.Should().NotBeNull();
        pet.Raca.Nome.Should().Be("Golden Retriever");
    }
}
