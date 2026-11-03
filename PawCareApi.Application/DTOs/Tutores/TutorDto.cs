using System.ComponentModel.DataAnnotations;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.DTOs.Tutores
{
    public class TutorRequestDto
    {
        [Required(ErrorMessage = "O CPF do tutor é obrigatório.")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do tutor é obrigatório.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        public int QtdPets { get; set; }
    }

    public class TutorResponseDto : ResourceWithLinks
    {
        public string Cpf { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int QtdPets { get; set; }
    }
}

