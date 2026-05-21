using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Models;

namespace PawCareApi.Mappings;

public class RacaMapping : IEntityTypeConfiguration<Raca>
{
    public void Configure(EntityTypeBuilder<Raca> builder)
    {
        builder.ToTable("RACA");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Especie)
            .HasColumnName("ESPECIE")
            .HasMaxLength(50)
            .IsRequired();
    }
}