using PawCareApi.Application.DTOs.Pets;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Application.Mappers
{
    public static class PetMapper
    {
        public static PetResponseDto ToResponseDto(this Pet pet)
        {
            return new PetResponseDto
            {
                Id = pet.Id,
                Nome = pet.Nome,
                DataNascimento = pet.DataNascimento,
                Peso = pet.Peso,
                StatusLongevidade = pet.StatusLongevidade,
                RacaId = pet.RacaId,
                RacaNome = pet.Raca?.Nome,
                Especie = pet.Raca?.Especie,
                TutorCpf = pet.TutorCpf,
                TutorNome = pet.Tutor?.Nome
            };
        }

        public static Pet ToEntity(this PetRequestDto dto)
        {
            return new Pet(
                dto.Nome,
                dto.DataNascimento,
                dto.Peso,
                dto.StatusLongevidade,
                dto.RacaId,
                dto.TutorCpf
            );
        }
    }
}

