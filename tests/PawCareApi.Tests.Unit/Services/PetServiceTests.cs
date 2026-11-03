using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Pets;
using PawCareApi.Application.Interfaces;
using PawCareApi.Application.Services;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Interfaces;

namespace PawCareApi.Tests.Unit.Services
{
    public class PetServiceTests
    {
        private readonly Mock<IPetRepository> _petRepoMock;
        private readonly Mock<IHateoasService> _hateoasMock;
        private readonly Mock<ILogger<PetService>> _loggerMock;
        private readonly PetService _service;

        public PetServiceTests()
        {
            _petRepoMock = new Mock<IPetRepository>();
            _hateoasMock = new Mock<IHateoasService>();
            _loggerMock = new Mock<ILogger<PetService>>();

            // Setup default Hateoas pass-through
            _hateoasMock.Setup(h => h.EnriquecerPet(It.IsAny<PetResponseDto>(), It.IsAny<string>()))
                .Returns<PetResponseDto, string>((dto, url) => dto);
            _hateoasMock.Setup(h => h.EnriquecerListaPets(It.IsAny<PageResult<PetResponseDto>>(), It.IsAny<string>()))
                .Returns<PageResult<PetResponseDto>, string>((p, url) => p);

            _service = new PetService(_petRepoMock.Object, _hateoasMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoPetExiste_DeveRetornarSucessoComPet()
        {
            // Arrange
            var pet = new Pet("Thor", new DateTime(2021, 1, 1), 15.0, "Adulto", 1, "12345678901") { Id = 1 };
            _petRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(pet);

            // Act
            var result = await _service.ObterPorIdAsync(1, "http://localhost");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data!.Nome.Should().Be("Thor");
            _petRepoMock.Verify(r => r.ObterPorIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoPetNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            _petRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Pet?)null);

            // Act
            var result = await _service.ObterPorIdAsync(99, "http://localhost");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Contain("não foi encontrado");
        }

        [Fact]
        public async Task CriarAsync_ComDtoValido_DeveSalvarERetornarCreated()
        {
            // Arrange
            var dto = new PetRequestDto
            {
                Nome = "Max",
                DataNascimento = new DateTime(2022, 1, 1),
                Peso = 10.0,
                StatusLongevidade = "Adulto",
                RacaId = 1,
                TutorCpf = "12345678901"
            };

            var petCriado = new Pet("Max", dto.DataNascimento, 10.0, "Adulto", 1, "12345678901") { Id = 10 };
            _petRepoMock.Setup(r => r.CriarAsync(It.IsAny<Pet>())).ReturnsAsync(petCriado);

            // Act
            var result = await _service.CriarAsync(dto, "http://localhost");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(201);
            result.Data.Should().NotBeNull();
            result.Data!.Nome.Should().Be("Max");
            _petRepoMock.Verify(r => r.CriarAsync(It.IsAny<Pet>()), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_QuandoPetExiste_DeveRetornarNoContent()
        {
            // Arrange
            _petRepoMock.Setup(r => r.RemoverAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.RemoverAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(204);
        }
    }
}

