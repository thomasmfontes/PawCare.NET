using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCareApi.Models;

namespace PawCareApi.Mappings;

public class HistoricoClinicoMapping : IEntityTypeConfiguration<HistoricoClinico>
{
    public void Configure(EntityTypeBuilder<HistoricoClinico> builder)
    {
        builder.ToTable("HISTORICO_CLINICO");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(h => h.IdEvento)
            .HasColumnName("ID_EVENTO")
            .IsRequired();

        builder.Property(h => h.DataEvento)
            .HasColumnName("DATA_EVENTO")
            .IsRequired();

        builder.Property(h => h.DataVencimento)
            .HasColumnName("DATA_VENCIMENTO");

        builder.Property(h => h.Status)
            .HasColumnName("STATUS")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.ObservacoesIa)
            .HasColumnName("OBSERVACOES_IA")
            .HasMaxLength(500);
    }
}