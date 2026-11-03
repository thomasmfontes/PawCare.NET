using PawCareApi.Domain.Exceptions;

namespace PawCareApi.Domain.Entities
{
    public class Clinica
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

        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();

        public Clinica() { }

        public Clinica(
            string nomeCnpj,
            string telefone,
            double latitude,
            double longitude,
            string bairro,
            string cidade,
            string estado,
            bool atendimento24h)
        {
            Validar(nomeCnpj, telefone, cidade, estado);

            NomeCnpj = nomeCnpj;
            Telefone = telefone;
            Latitude = latitude;
            Longitude = longitude;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Atendimento24h = atendimento24h;
        }

        public void AtualizarDados(
            string nomeCnpj,
            string telefone,
            double latitude,
            double longitude,
            string bairro,
            string cidade,
            string estado,
            bool atendimento24h)
        {
            Validar(nomeCnpj, telefone, cidade, estado);

            NomeCnpj = nomeCnpj;
            Telefone = telefone;
            Latitude = latitude;
            Longitude = longitude;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Atendimento24h = atendimento24h;
        }

        public static void Validar(string nomeCnpj, string telefone, string cidade, string estado)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(nomeCnpj))
                erros.Add("O nome ou CNPJ da clínica é obrigatório.");

            if (string.IsNullOrWhiteSpace(telefone))
                erros.Add("O telefone da clínica é obrigatório.");

            if (string.IsNullOrWhiteSpace(cidade))
                erros.Add("A cidade da clínica é obrigatória.");

            if (string.IsNullOrWhiteSpace(estado))
                erros.Add("O estado (UF) da clínica é obrigatório.");

            if (erros.Any())
                throw new DomainValidationException(erros);
        }
    }
}

