namespace PawCareApi.Models;

public class Tutor
{
    public string Cpf { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int QtdPets { get; set; }

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}