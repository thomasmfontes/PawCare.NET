namespace PawCareApi.Domain.Entities
{
    public class MedicoEspecialista
    {
        public long Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Especialidade { get; set; } = string.Empty;

        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();

        public MedicoEspecialista() { }

        public MedicoEspecialista(string nome, string especialidade)
        {
            Nome = nome;
            Especialidade = especialidade;
        }
    }
}

