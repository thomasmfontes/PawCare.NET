using FluentAssertions;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Exceptions;

namespace PawCareApi.Tests.Unit.Domain
{
    public class PetDomainTests
    {
        [Fact]
        public void InstanciarPet_ComDadosValidos_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange & Act
            var pet = new Pet("Thor", new DateTime(2022, 5, 10), 12.5, "Adulto", 1, "12345678901");

            // Assert
            pet.Nome.Should().Be("Thor");
            pet.DataNascimento.Should().Be(new DateTime(2022, 5, 10));
            pet.Peso.Should().Be(12.5);
            pet.StatusLongevidade.Should().Be("Adulto");
            pet.RacaId.Should().Be(1);
            pet.TutorCpf.Should().Be("12345678901");
        }

        [Fact]
        public void Validar_ComNomeVazio_DeveLancarDomainValidationException()
        {
            // Act
            Action act = () => new Pet("", DateTime.UtcNow.AddYears(-1), 10.0, "Adulto", 1, "12345678901");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*nome do pet é obrigatório*");
        }

        [Fact]
        public void Validar_ComPesoInvalido_DeveLancarDomainValidationException()
        {
            // Act
            Action act = () => new Pet("Rex", DateTime.UtcNow.AddYears(-1), -5.0, "Adulto", 1, "12345678901");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*peso do pet deve ser maior que zero*");
        }

        [Fact]
        public void CalcularStatusLongevidade_ComNascimentoRecente_DeveRetornarFilhote()
        {
            // Act
            var status = Pet.CalcularStatusLongevidade(DateTime.UtcNow.AddMonths(-4));

            // Assert
            status.Should().Be("Filhote");
        }
    }
}
