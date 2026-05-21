using Microsoft.EntityFrameworkCore;
using PawCareApi.Models;

namespace PawCareApi.Data;

public class PawCareContext : DbContext
{
    public PawCareContext(DbContextOptions<PawCareContext> options) : base(options)
    {
    }

    public DbSet<Tutor> Tutores { get; set; }
    public DbSet<Raca> Racas { get; set; }
    public DbSet<Pet> Pets { get; set; }

    public DbSet<Clinica> Clinicas { get; set; }
    public DbSet<MedicoEspecialista> MedicosEspecialistas { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<HistoricoClinico> HistoricosClinicos { get; set; }
    public DbSet<Tratamento> Tratamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PawCareContext).Assembly);
    }
}