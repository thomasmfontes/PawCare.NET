using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Models;

namespace PawCareApi.Mappings;

public class EventoMapping : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.ToTable("EVENTO");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Tipo)
            .HasColumnName("TIPO")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(e => e.IdPet)
            .HasColumnName("ID_PET")
            .IsRequired();

        builder.Property(e => e.IdTutor)
            .HasColumnName("ID_TUTOR")
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(e => e.IdMedico)
            .HasColumnName("ID_MEDICO")
            .IsRequired();

        builder.Property(e => e.IdClinica)
            .HasColumnName("ID_CLINICA")
            .IsRequired();

        builder.HasOne(e => e.Pet)
            .WithMany(p => p.Eventos)
            .HasForeignKey(e => e.IdPet);

        builder.HasOne(e => e.Tutor)
            .WithMany(t => t.Eventos)
            .HasForeignKey(e => e.IdTutor);

        builder.HasOne(e => e.Medico)
            .WithMany(m => m.Eventos)
            .HasForeignKey(e => e.IdMedico);

        builder.HasOne(e => e.Clinica)
            .WithMany(c => c.Eventos)
            .HasForeignKey(e => e.IdClinica);

        builder.HasOne(e => e.HistoricoClinico)
            .WithOne(h => h.Evento)
            .HasForeignKey<HistoricoClinico>(h => h.IdEvento);
    }
}