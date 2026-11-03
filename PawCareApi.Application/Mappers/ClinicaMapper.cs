using PawCareApi.Application.DTOs.Clinicas;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Application.Mappers
{
    public static class ClinicaMapper
    {
        public static ClinicaResponseDto ToResponseDto(this Clinica clinica)
        {
            return new ClinicaResponseDto
            {
                Id = clinica.Id,
                NomeCnpj = clinica.NomeCnpj,
                Telefone = clinica.Telefone,
                Latitude = clinica.Latitude,
                Longitude = clinica.Longitude,
                Bairro = clinica.Bairro,
                Cidade = clinica.Cidade,
                Estado = clinica.Estado,
                Atendimento24h = clinica.Atendimento24h
            };
        }

        public static Clinica ToEntity(this ClinicaRequestDto dto)
        {
            return new Clinica(
                dto.NomeCnpj,
                dto.Telefone,
                dto.Latitude,
                dto.Longitude,
                dto.Bairro,
                dto.Cidade,
                dto.Estado,
                dto.Atendimento24h
            );
        }
    }
}

