using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Models;

namespace PawCareApi.Mappings;

public class PetMapping : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("PET");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.DataNascimento)
            .HasColumnName("DATA_NASCIMENTO")
            .IsRequired();

        builder.Property(p => p.Peso)
            .HasColumnName("PESO")
            .IsRequired();

        builder.Property(p => p.StatusLongevidade)
            .HasColumnName("STATUS_LONGEVIDADE")
            .HasMaxLength(50);

        builder.Property(p => p.RacaId)
            .HasColumnName("RACA_ID")
            .IsRequired();

        builder.Property(p => p.TutorCpf)
            .HasColumnName("TUTOR_CPF")
            .HasMaxLength(14)
            .IsRequired();

        builder.HasOne(p => p.Raca)
            .WithMany(r => r.Pets)
            .HasForeignKey(p => p.RacaId);

        builder.HasOne(p => p.Tutor)
            .WithMany(t => t.Pets)
            .HasForeignKey(p => p.TutorCpf);
    }
}