using System.ComponentModel.DataAnnotations;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.DTOs.Pets
{
    public class PetRequestDto
    {
        [Required(ErrorMessage = "O nome do pet é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome do pet deve ter entre 2 e 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; }

        [Range(0.1, 200.0, ErrorMessage = "O peso deve ser entre 0.1kg e 200kg.")]
        public double Peso { get; set; }

        public string StatusLongevidade { get; set; } = string.Empty;

        [Range(1, long.MaxValue, ErrorMessage = "O identificador da raça deve ser válido.")]
        public long RacaId { get; set; }

        [Required(ErrorMessage = "O CPF do tutor é obrigatório.")]
        public string TutorCpf { get; set; } = string.Empty;
    }

    public class PetResponseDto : ResourceWithLinks
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public double Peso { get; set; }
        public string StatusLongevidade { get; set; } = string.Empty;
        public long RacaId { get; set; }
        public string? RacaNome { get; set; }
        public string? Especie { get; set; }
        public string TutorCpf { get; set; } = string.Empty;
        public string? TutorNome { get; set; }
    }
}

