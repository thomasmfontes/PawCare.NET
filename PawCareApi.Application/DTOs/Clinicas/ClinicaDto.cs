using System.ComponentModel.DataAnnotations;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.DTOs.Clinicas
{
    public class ClinicaRequestDto
    {
        [Required(ErrorMessage = "O nome ou CNPJ da clínica é obrigatório.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres.")]
        public string NomeCnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string Telefone { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado (UF) é obrigatório.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O estado deve ter 2 caracteres (UF).")]
        public string Estado { get; set; } = string.Empty;

        public bool Atendimento24h { get; set; }
    }

    public class ClinicaResponseDto : ResourceWithLinks
    {
        public long Id { get; set; }
        public string NomeCnpj { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool Atendimento24h { get; set; }
    }
}

