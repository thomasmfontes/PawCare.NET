using System.Diagnostics;

namespace PawCareApi.Models;

public class Clinica
{
    public long Id { get; set; }

    public string NomeCnpj { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Bairro { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public bool Atendimento24h { get; set; }

    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}