using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Infrastructure.Data.Configurations
{
    public class ClinicaConfiguration : IEntityTypeConfiguration<Clinica>
    {
        public void Configure(EntityTypeBuilder<Clinica> builder)
        {
            builder.ToTable("CLINICA");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.NomeCnpj)
                .HasColumnName("NOME_CNPJ")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.Telefone)
                .HasColumnName("TELEFONE")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.Latitude)
                .HasColumnName("LATITUDE")
                .IsRequired();

            builder.Property(c => c.Longitude)
                .HasColumnName("LONGITUDE")
                .IsRequired();

            builder.Property(c => c.Bairro)
                .HasColumnName("BAIRRO")
                .HasMaxLength(100);

            builder.Property(c => c.Cidade)
                .HasColumnName("CIDADE")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Estado)
                .HasColumnName("ESTADO")
                .HasMaxLength(2)
                .IsRequired();

            builder.Property(c => c.Atendimento24h)
                .HasColumnName("ATENDIMENTO_24H")
                .IsRequired();

            builder.HasIndex(c => c.Cidade);
            builder.HasIndex(c => c.Atendimento24h);
        }
    }
}

