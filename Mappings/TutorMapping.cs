using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Models;

namespace PawCareApi.Mappings;

public class TutorMapping : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("TUTOR");

        builder.HasKey(t => t.Cpf);

        builder.Property(t => t.Cpf)
            .HasColumnName("CPF")
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(t => t.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Telefone)
            .HasColumnName("TELEFONE")
            .HasMaxLength(20);

        builder.Property(t => t.Email)
            .HasColumnName("EMAIL")
            .HasMaxLength(100);

        builder.Property(t => t.QtdPets)
            .HasColumnName("QTD_PETS");
    }
}