using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Prontuarios;
using PawCareApi.Application.Interfaces;
using PawCareApi.Application.Services;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Interfaces;

namespace PawCareApi.Tests.Unit.Services
{
    public class ProntuarioServiceTests
    {
        private readonly Mock<IProntuarioRepository> _repoMock;
        private readonly Mock<IHateoasService> _hateoasMock;
        private readonly Mock<ILogger<ProntuarioService>> _loggerMock;
        private readonly ProntuarioService _service;

        public ProntuarioServiceTests()
        {
            _repoMock = new Mock<IProntuarioRepository>();
            _hateoasMock = new Mock<IHateoasService>();
            _loggerMock = new Mock<ILogger<ProntuarioService>>();

            _hateoasMock.Setup(h => h.EnriquecerProntuario(It.IsAny<ProntuarioResponseDto>(), It.IsAny<string>()))
                .Returns<ProntuarioResponseDto, string>((dto, url) => dto);
            _hateoasMock.Setup(h => h.EnriquecerListaProntuarios(It.IsAny<PageResult<ProntuarioResponseDto>>(), It.IsAny<string>()))
                .Returns<PageResult<ProntuarioResponseDto>, string>((p, url) => p);

            _service = new ProntuarioService(_repoMock.Object, _hateoasMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoProntuarioExiste_DeveRetornarSucesso()
        {
            // Arrange
            var prontuario = new ProntuarioNoSql(1, "Rex", "12345678901", "Dr. Silva", "Consulta", "Vômito", "Gastroenterite")
            {
                Id = "66f00112233445566778899a"
            };
            _repoMock.Setup(r => r.ObterPorIdAsync("66f00112233445566778899a")).ReturnsAsync(prontuario);

            // Act
            var result = await _service.ObterPorIdAsync("66f00112233445566778899a", "http://localhost");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data!.NomePet.Should().Be("Rex");
        }

        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveSalvarERetornarCreated()
        {
            // Arrange
            var dto = new ProntuarioRequestDto
            {
                PetId = 5,
                NomePet = "Luna",
                TutorCpf = "98765432100",
                VeterinarioNome = "Dra. Maria",
                TipoAtendimento = "Vacinação",
                QueixaPrincipal = "Vacina anual",
                Diagnostico = "Animal saudável"
            };

            var criado = new ProntuarioNoSql(5, "Luna", "98765432100", "Dra. Maria", "Vacinação", "Vacina anual", "Animal saudável")
            {
                Id = "66f00112233445566778899b"
            };

            _repoMock.Setup(r => r.CriarAsync(It.IsAny<ProntuarioNoSql>())).ReturnsAsync(criado);

            // Act
            var result = await _service.CriarAsync(dto, "http://localhost");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(201);
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be("66f00112233445566778899b");
        }
    }
}

