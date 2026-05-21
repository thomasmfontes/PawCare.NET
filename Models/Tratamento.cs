namespace PawCareApi.Models;

public class Tratamento
{
    public long Id { get; set; }

    public long IdPet { get; set; }

    public string NomeMedicamento { get; set; } = string.Empty;

    public string Frequencia { get; set; } = string.Empty;

    public DateTime DataInicio { get; set; }

    public DateTime? DataFinal { get; set; }

    public Pet? Pet { get; set; }
}