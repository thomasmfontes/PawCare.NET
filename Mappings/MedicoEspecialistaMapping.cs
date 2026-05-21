using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Models;

namespace PawCareApi.Mappings;

public class MedicoEspecialistaMapping : IEntityTypeConfiguration<MedicoEspecialista>
{
    public void Configure(EntityTypeBuilder<MedicoEspecialista> builder)
    {
        builder.ToTable("MEDICO_ESPECIALISTA");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Especialidade)
            .HasColumnName("ESPECIALIDADE")
            .HasMaxLength(100)
            .IsRequired();
    }
}