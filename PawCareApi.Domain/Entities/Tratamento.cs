namespace PawCareApi.Domain.Entities
{
    public class Tratamento
    {
        public long Id { get; set; }

        public long IdPet { get; set; }

        public string NomeMedicamento { get; set; } = string.Empty;

        public string Frequencia { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }

        public DateTime? DataFinal { get; set; }

        public Pet? Pet { get; set; }

        public Tratamento() { }

        public Tratamento(long idPet, string nomeMedicamento, string frequencia, DateTime dataInicio, DateTime? dataFinal = null)
        {
            IdPet = idPet;
            NomeMedicamento = nomeMedicamento;
            Frequencia = frequencia;
            DataInicio = dataInicio;
            DataFinal = dataFinal;
        }
    }
}

