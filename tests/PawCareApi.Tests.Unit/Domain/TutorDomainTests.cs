using FluentAssertions;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Exceptions;

namespace PawCareApi.Tests.Unit.Domain
{
    public class TutorDomainTests
    {
        [Fact]
        public void InstanciarTutor_ComDadosValidos_DeveAtribuirPropriedadesECpfLimpo()
        {
            // Act
            var tutor = new Tutor("123.456.789-01", "Thomas Fontes", "11987654321", "thomas@fiap.com.br", 2);

            // Assert
            tutor.Cpf.Should().Be("12345678901");
            tutor.Nome.Should().Be("Thomas Fontes");
            tutor.Telefone.Should().Be("11987654321");
            tutor.Email.Should().Be("thomas@fiap.com.br");
            tutor.QtdPets.Should().Be(2);
        }

        [Fact]
        public void Validar_ComEmailInvalido_DeveLancarDomainValidationException()
        {
            // Act
            Action act = () => new Tutor("12345678901", "Carlos", "1199998888", "emailinvalido");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*e-mail válido*");
        }

        [Fact]
        public void Validar_ComCpfVazio_DeveLancarDomainValidationException()
        {
            // Act
            Action act = () => new Tutor("", "Carlos", "1199998888", "carlos@gmail.com");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*CPF do tutor é obrigatório*");
        }
    }
}
