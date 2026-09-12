using FluentAssertions;
using PawCareApi.Models;

namespace PawCareApi.Tests.Unit.Domain;

public class TutorDomainTests
{
    [Fact]
    public void InstanciarTutor_ComDadosValidos_DeveInicializarPropriedadesCorretamente()
    {
        // Arrange
        var cpf = "98765432100";
        var nome = "Mariana Costa";
        var email = "mariana@email.com";
        var telefone = "11999998888";
        var qtdPets = 2;

        // Act
        var tutor = new Tutor
        {
            Cpf = cpf,
            Nome = nome,
            Email = email,
            Telefone = telefone,
            QtdPets = qtdPets
        };

        // Assert
        tutor.Cpf.Should().Be(cpf);
        tutor.Nome.Should().Be(nome);
        tutor.Email.Should().Be(email);
        tutor.Telefone.Should().Be(telefone);
        tutor.QtdPets.Should().Be(qtdPets);
    }

    [Fact]
    public void AdicionarPets_NaColecaoDePets_DeveAumentarTamanhoDaLista()
    {
        // Arrange
        var tutor = new Tutor { Cpf = "98765432100", Nome = "Mariana Costa" };
        var pet1 = new Pet { Id = 1, Nome = "Mel", TutorCpf = tutor.Cpf };
        var pet2 = new Pet { Id = 2, Nome = "Luna", TutorCpf = tutor.Cpf };

        // Act
        tutor.Pets.Add(pet1);
        tutor.Pets.Add(pet2);

        // Assert
        tutor.Pets.Should().HaveCount(2);
        tutor.Pets.Should().Contain(p => p.Nome == "Mel");
        tutor.Pets.Should().Contain(p => p.Nome == "Luna");
    }
}
