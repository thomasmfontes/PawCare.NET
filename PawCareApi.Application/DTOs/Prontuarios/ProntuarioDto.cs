using System.ComponentModel.DataAnnotations;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Application.DTOs.Prontuarios
{
    public class ProntuarioRequestDto
    {
        [Required(ErrorMessage = "O identificador do pet é obrigatório.")]
        [Range(1, long.MaxValue, ErrorMessage = "O ID do pet deve ser maior que zero.")]
        public long PetId { get; set; }

        [Required(ErrorMessage = "O nome do pet é obrigatório.")]
        public string NomePet { get; set; } = string.Empty;

        public string TutorCpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do veterinário responsável é obrigatório.")]
        public string VeterinarioNome { get; set; } = string.Empty;

        public string TipoAtendimento { get; set; } = "Consulta de Rotina";

        [Required(ErrorMessage = "A queixa principal é obrigatória.")]
        public string QueixaPrincipal { get; set; } = string.Empty;

        public string Diagnostico { get; set; } = string.Empty;

        public SinaisVitaisNoSql SinaisVitais { get; set; } = new();

        public List<string> Sintomas { get; set; } = new();

        public List<PrescricaoItemNoSql> Prescricoes { get; set; } = new();

        public string Observacoes { get; set; } = string.Empty;
    }

    public class ProntuarioResponseDto : ResourceWithLinks
    {
        public string Id { get; set; } = string.Empty;
        public long PetId { get; set; }
        public string NomePet { get; set; } = string.Empty;
        public string TutorCpf { get; set; } = string.Empty;
        public string VeterinarioNome { get; set; } = string.Empty;
        public DateTime DataAtendimento { get; set; }
        public string TipoAtendimento { get; set; } = string.Empty;
        public string QueixaPrincipal { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public SinaisVitaisNoSql SinaisVitais { get; set; } = new();
        public List<string> Sintomas { get; set; } = new();
        public List<PrescricaoItemNoSql> Prescricoes { get; set; } = new();
        public string Observacoes { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
    }
}

