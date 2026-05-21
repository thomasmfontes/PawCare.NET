using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Models;

namespace PawCareApi.Mappings;

public class ClinicaMapping : IEntityTypeConfiguration<Clinica>
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
            .HasMaxLength(20);

        builder.Property(c => c.Latitude)
            .HasColumnName("LATITUDE");

        builder.Property(c => c.Longitude)
            .HasColumnName("LONGITUDE");

        builder.Property(c => c.Bairro)
            .HasColumnName("BAIRRO")
            .HasMaxLength(80);

        builder.Property(c => c.Cidade)
            .HasColumnName("CIDADE")
            .HasMaxLength(80);

        builder.Property(c => c.Estado)
            .HasColumnName("ESTADO")
            .HasMaxLength(2);

        builder.Property(c => c.Atendimento24h)
            .HasColumnName("ATENDIMENTO_24H")
            .HasConversion<int>()
            .HasColumnType("NUMBER(1)");
    }
}