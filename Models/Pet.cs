namespace PawCareApi.Models;

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
}