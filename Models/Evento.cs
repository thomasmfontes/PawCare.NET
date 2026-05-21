namespace PawCareApi.Models;

public class Evento
{
    public long Id { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public long IdPet { get; set; }

    public string IdTutor { get; set; } = string.Empty;

    public long IdMedico { get; set; }

    public long IdClinica { get; set; }

    public Pet? Pet { get; set; }

    public Tutor? Tutor { get; set; }

    public MedicoEspecialista? Medico { get; set; }

    public Clinica? Clinica { get; set; }

    public HistoricoClinico? HistoricoClinico { get; set; }
}