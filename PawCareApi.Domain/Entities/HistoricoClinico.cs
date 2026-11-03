namespace PawCareApi.Domain.Entities
{
    public class HistoricoClinico
    {
        public long Id { get; set; }

        public long IdEvento { get; set; }

        public DateTime DataEvento { get; set; }

        public DateTime? DataVencimento { get; set; }

        public string Status { get; set; } = string.Empty;

        public string ObservacoesIa { get; set; } = string.Empty;

        public Evento? Evento { get; set; }

        public HistoricoClinico() { }

        public HistoricoClinico(long idEvento, DateTime dataEvento, string status, string observacoesIa = "", DateTime? dataVencimento = null)
        {
            IdEvento = idEvento;
            DataEvento = dataEvento;
            Status = status;
            ObservacoesIa = observacoesIa;
            DataVencimento = dataVencimento;
        }
    }
}

