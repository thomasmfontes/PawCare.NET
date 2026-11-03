using FluentAssertions;
using PawCareApi.Application.DTOs.Pets;
using PawCareApi.Application.DTOs.Prontuarios;
using PawCareApi.Application.DTOs.Tutores;
using PawCareApi.Application.Services;

namespace PawCareApi.Tests.Unit.Services
{
    public class HateoasServiceTests
    {
        private readonly HateoasService _hateoasService;

        public HateoasServiceTests()
        {
            _hateoasService = new HateoasService();
        }

        [Fact]
        public void EnriquecerPet_DeveAdicionarLinksSelfUpdateDeleteEProntuarios()
        {
            // Arrange
            var pet = new PetResponseDto
            {
                Id = 1,
                Nome = "Thor",
                TutorCpf = "12345678901"
            };

            // Act
            var enriquecido = _hateoasService.EnriquecerPet(pet, "http://localhost:5000");

            // Assert
            enriquecido.Links.Should().NotBeEmpty();
            enriquecido.Links.Should().Contain(l => l.Rel == "self" && l.Href == "http://localhost:5000/api/pets/1");
            enriquecido.Links.Should().Contain(l => l.Rel == "update" && l.Method == "PUT");
            enriquecido.Links.Should().Contain(l => l.Rel == "delete" && l.Method == "DELETE");
            enriquecido.Links.Should().Contain(l => l.Rel == "tutor");
            enriquecido.Links.Should().Contain(l => l.Rel == "prontuarios-nosql");
        }

        [Fact]
        public void EnriquecerProntuario_DeveAdicionarLinksSelfEHierarquicos()
        {
            // Arrange
            var prontuario = new ProntuarioResponseDto
            {
                Id = "66f00112233445566778899a",
                PetId = 1,
                NomePet = "Thor"
            };

            // Act
            var enriquecido = _hateoasService.EnriquecerProntuario(prontuario, "http://localhost:5000");

            // Assert
            enriquecido.Links.Should().Contain(l => l.Rel == "self" && l.Href.EndsWith("/api/prontuarios/66f00112233445566778899a"));
            enriquecido.Links.Should().Contain(l => l.Rel == "pet-relacional");
        }

        [Fact]
        public void EnriquecerTutor_DeveAdicionarLinksSelfUpdateDeleteEPets()
        {
            // Arrange
            var tutor = new TutorResponseDto
            {
                Cpf = "12345678901",
                Nome = "Thomas"
            };

            // Act
            var enriquecido = _hateoasService.EnriquecerTutor(tutor, "http://localhost:5000");

            // Assert
            enriquecido.Links.Should().Contain(l => l.Rel == "self" && l.Href.EndsWith("/api/tutores/12345678901"));
            enriquecido.Links.Should().Contain(l => l.Rel == "pets");
        }
    }
}

