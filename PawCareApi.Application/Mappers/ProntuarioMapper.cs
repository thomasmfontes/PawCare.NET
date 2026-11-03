using PawCareApi.Application.DTOs.Prontuarios;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Application.Mappers
{
    public static class ProntuarioMapper
    {
        public static ProntuarioResponseDto ToResponseDto(this ProntuarioNoSql prontuario)
        {
            return new ProntuarioResponseDto
            {
                Id = prontuario.Id ?? string.Empty,
                PetId = prontuario.PetId,
                NomePet = prontuario.NomePet,
                TutorCpf = prontuario.TutorCpf,
                VeterinarioNome = prontuario.VeterinarioNome,
                DataAtendimento = prontuario.DataAtendimento,
                TipoAtendimento = prontuario.TipoAtendimento,
                QueixaPrincipal = prontuario.QueixaPrincipal,
                Diagnostico = prontuario.Diagnostico,
                SinaisVitais = prontuario.SinaisVitais ?? new SinaisVitaisNoSql(),
                Sintomas = prontuario.Sintomas ?? new List<string>(),
                Prescricoes = prontuario.Prescricoes ?? new List<PrescricaoItemNoSql>(),
                Observacoes = prontuario.Observacoes,
                DataCriacao = prontuario.DataCriacao
            };
        }

        public static ProntuarioNoSql ToEntity(this ProntuarioRequestDto dto)
        {
            return new ProntuarioNoSql(
                dto.PetId,
                dto.NomePet,
                dto.TutorCpf,
                dto.VeterinarioNome,
                dto.TipoAtendimento,
                dto.QueixaPrincipal,
                dto.Diagnostico,
                dto.SinaisVitais,
                dto.Sintomas,
                dto.Prescricoes,
                dto.Observacoes
            );
        }
    }
}

