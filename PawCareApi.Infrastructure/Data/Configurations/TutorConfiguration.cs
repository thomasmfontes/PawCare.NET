using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Infrastructure.Data.Configurations
{
    public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
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
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(t => t.Telefone)
                .HasColumnName("TELEFONE")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(t => t.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(t => t.QtdPets)
                .HasColumnName("QTD_PETS")
                .IsRequired();

            builder.HasIndex(t => t.Email);
            builder.HasIndex(t => t.Nome);
        }
    }
}

