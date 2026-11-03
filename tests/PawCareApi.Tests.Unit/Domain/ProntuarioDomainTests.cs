using FluentAssertions;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Exceptions;

namespace PawCareApi.Tests.Unit.Domain
{
    public class ProntuarioDomainTests
    {
        [Fact]
        public void InstanciarProntuarioNoSql_ComDadosValidos_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange
            var sinaisVitais = new SinaisVitaisNoSql
            {
                TemperaturaCelsius = 38.5,
                FrequenciaCardiacaBpm = 95,
                FrequenciaRespiratoriaMpm = 22,
                PesoKg = 15.2
            };

            var sintomas = new List<string> { "Febre moderada", "Apatia" };
            var prescricoes = new List<PrescricaoItemNoSql>
            {
                new() { Medicamento = "Amoxicilina", Dosagem = "250mg", Frequencia = "12/12h", DuracaoDias = 7 }
            };

            // Act
            var prontuario = new ProntuarioNoSql(
                10,
                "Bob",
                "12345678901",
                "Dr. Veterinário Silva",
                "Consulta de Rotina",
                "Animal prostrado",
                "Gastroenterite leve",
                sinaisVitais,
                sintomas,
                prescricoes,
                "Repouso e hidratação"
            );

            // Assert
            prontuario.PetId.Should().Be(10);
            prontuario.NomePet.Should().Be("Bob");
            prontuario.VeterinarioNome.Should().Be("Dr. Veterinário Silva");
            prontuario.Sintomas.Should().HaveCount(2);
            prontuario.Prescricoes.Should().HaveCount(1);
            prontuario.SinaisVitais.TemperaturaCelsius.Should().Be(38.5);
        }

        [Fact]
        public void InstanciarProntuarioNoSql_ComPetIdInvalido_DeveLancarDomainValidationException()
        {
            // Act
            Action act = () => new ProntuarioNoSql(
                0, "Bob", "12345678901", "Dr. Silva", "Consulta", "Febre", "Infeccao");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*PetId*");
        }

        [Fact]
        public void InstanciarProntuarioNoSql_ComQueixaVazia_DeveLancarDomainValidationException()
        {
            // Act
            Action act = () => new ProntuarioNoSql(
                1, "Bob", "12345678901", "Dr. Silva", "Consulta", "", "Infeccao");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*queixa principal*");
        }
    }
}

