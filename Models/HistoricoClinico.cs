namespace PawCareApi.Models;

public class HistoricoClinico
{
    public long Id { get; set; }

    public long IdEvento { get; set; }

    public DateTime DataEvento { get; set; }

    public DateTime? DataVencimento { get; set; }

    public string Status { get; set; } = string.Empty;

    public string ObservacoesIa { get; set; } = string.Empty;

    public Evento? Evento { get; set; }
}