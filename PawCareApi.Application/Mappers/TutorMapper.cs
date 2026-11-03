using PawCareApi.Application.DTOs.Tutores;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Application.Mappers
{
    public static class TutorMapper
    {
        public static TutorResponseDto ToResponseDto(this Tutor tutor)
        {
            return new TutorResponseDto
            {
                Cpf = tutor.Cpf,
                Nome = tutor.Nome,
                Telefone = tutor.Telefone,
                Email = tutor.Email,
                QtdPets = tutor.QtdPets
            };
        }

        public static Tutor ToEntity(this TutorRequestDto dto)
        {
            return new Tutor(
                dto.Cpf,
                dto.Nome,
                dto.Telefone,
                dto.Email,
                dto.QtdPets
            );
        }
    }
}

