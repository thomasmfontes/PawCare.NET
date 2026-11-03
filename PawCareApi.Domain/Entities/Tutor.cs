using PawCareApi.Domain.Exceptions;

namespace PawCareApi.Domain.Entities
{
    public class Tutor
    {
        public string Cpf { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int QtdPets { get; set; }

        public ICollection<Pet> Pets { get; set; } = new List<Pet>();

        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();

        public Tutor() { }

        public Tutor(string cpf, string nome, string telefone, string email, int qtdPets = 0)
        {
            Validar(cpf, nome, telefone, email);

            Cpf = LimparCpf(cpf);
            Nome = nome.Trim();
            Telefone = telefone.Trim();
            Email = email.Trim().ToLowerInvariant();
            QtdPets = qtdPets >= 0 ? qtdPets : 0;
        }

        public void AtualizarDados(string nome, string telefone, string email, int qtdPets)
        {
            Validar(Cpf, nome, telefone, email);

            Nome = nome.Trim();
            Telefone = telefone.Trim();
            Email = email.Trim().ToLowerInvariant();
            QtdPets = qtdPets >= 0 ? qtdPets : 0;
        }

        public static void Validar(string cpf, string nome, string telefone, string email)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(cpf))
                erros.Add("O CPF do tutor é obrigatório.");

            if (string.IsNullOrWhiteSpace(nome))
                erros.Add("O nome do tutor é obrigatório.");

            if (string.IsNullOrWhiteSpace(telefone))
                erros.Add("O telefone do tutor é obrigatório.");

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                erros.Add("Um e-mail válido para o tutor é obrigatório.");

            if (erros.Any())
                throw new DomainValidationException(erros);
        }

        public static string LimparCpf(string cpf)
        {
            return new string(cpf.Where(char.IsDigit).ToArray());
        }
    }
}

