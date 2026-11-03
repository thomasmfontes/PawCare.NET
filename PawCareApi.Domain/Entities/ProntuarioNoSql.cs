using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PawCareApi.Domain.Exceptions;

namespace PawCareApi.Domain.Entities
{
    public class SinaisVitaisNoSql
    {
        public double? TemperaturaCelsius { get; set; }
        public int? FrequenciaCardiacaBpm { get; set; }
        public int? FrequenciaRespiratoriaMpm { get; set; }
        public double? PesoKg { get; set; }
        public string? PressaoArterial { get; set; }
    }

    public class PrescricaoItemNoSql
    {
        public string Medicamento { get; set; } = string.Empty;
        public string Dosagem { get; set; } = string.Empty;
        public string Frequencia { get; set; } = string.Empty;
        public int DuracaoDias { get; set; }
    }

    public class ProntuarioNoSql
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public long PetId { get; set; }

        public string NomePet { get; set; } = string.Empty;

        public string TutorCpf { get; set; } = string.Empty;

        public string VeterinarioNome { get; set; } = string.Empty;

        public DateTime DataAtendimento { get; set; } = DateTime.UtcNow;

        public string TipoAtendimento { get; set; } = "Consulta de Rotina"; // "Consulta", "Emergência", "Cirurgia", "Exame"

        public string QueixaPrincipal { get; set; } = string.Empty;

        public string Diagnostico { get; set; } = string.Empty;

        public SinaisVitaisNoSql SinaisVitais { get; set; } = new();

        public List<string> Sintomas { get; set; } = new();

        public List<PrescricaoItemNoSql> Prescricoes { get; set; } = new();

        public string Observacoes { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public ProntuarioNoSql() { }

        public ProntuarioNoSql(
            long petId,
            string nomePet,
            string tutorCpf,
            string veterinarioNome,
            string tipoAtendimento,
            string queixaPrincipal,
            string diagnostico,
            SinaisVitaisNoSql? sinaisVitais = null,
            IEnumerable<string>? sintomas = null,
            IEnumerable<PrescricaoItemNoSql>? prescricoes = null,
            string observacoes = "")
        {
            ValidarDados(petId, nomePet, veterinarioNome, queixaPrincipal);

            PetId = petId;
            NomePet = nomePet;
            TutorCpf = tutorCpf;
            VeterinarioNome = veterinarioNome;
            TipoAtendimento = string.IsNullOrWhiteSpace(tipoAtendimento) ? "Consulta de Rotina" : tipoAtendimento;
            QueixaPrincipal = queixaPrincipal;
            Diagnostico = diagnostico;
            SinaisVitais = sinaisVitais ?? new SinaisVitaisNoSql();
            Sintomas = sintomas?.ToList() ?? new List<string>();
            Prescricoes = prescricoes?.ToList() ?? new List<PrescricaoItemNoSql>();
            Observacoes = observacoes;
            DataAtendimento = DateTime.UtcNow;
            DataCriacao = DateTime.UtcNow;
        }

        public void Atualizar(
            string veterinarioNome,
            string tipoAtendimento,
            string queixaPrincipal,
            string diagnostico,
            SinaisVitaisNoSql? sinaisVitais,
            IEnumerable<string>? sintomas,
            IEnumerable<PrescricaoItemNoSql>? prescricoes,
            string observacoes)
        {
            ValidarDados(PetId, NomePet, veterinarioNome, queixaPrincipal);

            VeterinarioNome = veterinarioNome;
            TipoAtendimento = tipoAtendimento;
            QueixaPrincipal = queixaPrincipal;
            Diagnostico = diagnostico;
            if (sinaisVitais != null) SinaisVitais = sinaisVitais;
            if (sintomas != null) Sintomas = sintomas.ToList();
            if (prescricoes != null) Prescricoes = prescricoes.ToList();
            Observacoes = observacoes;
        }

        private static void ValidarDados(long petId, string nomePet, string veterinarioNome, string queixaPrincipal)
        {
            var erros = new List<string>();

            if (petId <= 0)
                erros.Add("O identificador do pet (PetId) deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(nomePet))
                erros.Add("O nome do pet é obrigatório.");

            if (string.IsNullOrWhiteSpace(veterinarioNome))
                erros.Add("O nome do médico veterinário responsável é obrigatório.");

            if (string.IsNullOrWhiteSpace(queixaPrincipal))
                erros.Add("A queixa principal do atendimento é obrigatória.");

            if (erros.Any())
                throw new DomainValidationException(erros);
        }
    }
}

