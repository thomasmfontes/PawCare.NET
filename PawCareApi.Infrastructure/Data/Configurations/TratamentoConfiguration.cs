using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Infrastructure.Data.Configurations
{
    public class TratamentoConfiguration : IEntityTypeConfiguration<Tratamento>
    {
        public void Configure(EntityTypeBuilder<Tratamento> builder)
        {
            builder.ToTable("TRATAMENTO");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.IdPet)
                .HasColumnName("ID_PET")
                .IsRequired();

            builder.Property(t => t.NomeMedicamento)
                .HasColumnName("NOME_MEDICAMENTO")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Frequencia)
                .HasColumnName("FREQUENCIA")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.DataInicio)
                .HasColumnName("DATA_INICIO")
                .IsRequired();

            builder.Property(t => t.DataFinal)
                .HasColumnName("DATA_FINAL");

            builder.HasOne(t => t.Pet)
                .WithMany(p => p.Tratamentos)
                .HasForeignKey(t => t.IdPet);
        }
    }
}

