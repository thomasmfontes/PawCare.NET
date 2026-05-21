using System.Diagnostics;

namespace PawCareApi.Models;

public class MedicoEspecialista
{
    public long Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Especialidade { get; set; } = string.Empty;

    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}