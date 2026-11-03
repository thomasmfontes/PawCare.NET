using PawCareApi.Domain.Exceptions;

namespace PawCareApi.Domain.Entities
{
    public class Pet
    {
        public long Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public DateTime DataNascimento { get; set; }

        public double Peso { get; set; }

        public string StatusLongevidade { get; set; } = string.Empty;

        public long RacaId { get; set; }

        public string TutorCpf { get; set; } = string.Empty;

        public Raca? Raca { get; set; }

        public Tutor? Tutor { get; set; }

        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();

        public ICollection<Tratamento> Tratamentos { get; set; } = new List<Tratamento>();

        public Pet() { }

        public Pet(string nome, DateTime dataNascimento, double peso, string statusLongevidade, long racaId, string tutorCpf)
        {
            Validar(nome, dataNascimento, peso, racaId, tutorCpf);

            Nome = nome;
            DataNascimento = dataNascimento;
            Peso = peso;
            StatusLongevidade = string.IsNullOrWhiteSpace(statusLongevidade) ? CalcularStatusLongevidade(dataNascimento) : statusLongevidade;
            RacaId = racaId;
            TutorCpf = tutorCpf;
        }

        public void AtualizarDados(string nome, DateTime dataNascimento, double peso, string statusLongevidade, long racaId, string tutorCpf)
        {
            Validar(nome, dataNascimento, peso, racaId, tutorCpf);

            Nome = nome;
            DataNascimento = dataNascimento;
            Peso = peso;
            StatusLongevidade = string.IsNullOrWhiteSpace(statusLongevidade) ? CalcularStatusLongevidade(dataNascimento) : statusLongevidade;
            RacaId = racaId;
            TutorCpf = tutorCpf;
        }

        public static void Validar(string nome, DateTime dataNascimento, double peso, long racaId, string tutorCpf)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(nome))
                erros.Add("O nome do pet é obrigatório.");

            if (dataNascimento > DateTime.UtcNow.AddDays(1))
                erros.Add("A data de nascimento do pet não pode estar no futuro.");

            if (peso <= 0)
                erros.Add("O peso do pet deve ser maior que zero.");

            if (racaId <= 0)
                erros.Add("O identificador da raça (RacaId) deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(tutorCpf))
                erros.Add("O CPF do tutor é obrigatório.");

            if (erros.Any())
                throw new DomainValidationException(erros);
        }

        public static string CalcularStatusLongevidade(DateTime dataNascimento)
        {
            var idadeAnos = (DateTime.UtcNow - dataNascimento).TotalDays / 365.25;
            if (idadeAnos < 1) return "Filhote";
            if (idadeAnos < 7) return "Adulto";
            return "Idoso";
        }
    }
}

