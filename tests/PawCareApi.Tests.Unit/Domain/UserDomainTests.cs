using FluentAssertions;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Tests.Unit.Domain
{
    public class UserDomainTests
    {
        [Fact]
        public void InstanciarUser_ComDadosValidos_DeveAtribuirPropriedadesCorretamente()
        {
            // Act
            var user = new User("admin", "hash123", "Administrador", "Admin");

            // Assert
            user.Username.Should().Be("admin");
            user.PasswordHash.Should().Be("hash123");
            user.Nome.Should().Be("Administrador");
            user.Role.Should().Be("Admin");
            user.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
}

