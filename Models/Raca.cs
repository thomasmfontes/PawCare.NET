namespace PawCareApi.Models;

public class Raca
{
    public long Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Especie { get; set; } = string.Empty;

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}