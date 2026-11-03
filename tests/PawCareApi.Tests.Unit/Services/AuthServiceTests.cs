using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PawCareApi.Application.DTOs.Auth;
using PawCareApi.Application.Interfaces;
using PawCareApi.Application.Services;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Interfaces;

namespace PawCareApi.Tests.Unit.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJwtTokenGenerator> _jwtMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtMock = new Mock<IJwtTokenGenerator>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            _service = new AuthService(_userRepoMock.Object, _jwtMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ComCredenciaisValidas_DeveRetornarTokenJWTESucesso()
        {
            // Arrange
            var user = new User("admin", "hash123", "Administrador", "Admin") { Id = 1 };
            _userRepoMock.Setup(r => r.ValidarCredenciaisAsync("admin", "Admin@123")).ReturnsAsync(user);
            _jwtMock.Setup(j => j.GenerateToken(user)).Returns(("jwt.mock.token", DateTime.UtcNow.AddHours(4)));

            var request = new LoginRequestDto { Username = "admin", Password = "Admin@123" };

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data!.Token.Should().Be("jwt.mock.token");
            result.Data.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task LoginAsync_ComCredenciaisInvalidas_DeveRetornarUnauthorized()
        {
            // Arrange
            _userRepoMock.Setup(r => r.ValidarCredenciaisAsync("admin", "senhaerrada")).ReturnsAsync((User?)null);

            var request = new LoginRequestDto { Username = "admin", Password = "senhaerrada" };

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(401);
            result.Message.Should().Contain("inválidos");
        }
    }
}

