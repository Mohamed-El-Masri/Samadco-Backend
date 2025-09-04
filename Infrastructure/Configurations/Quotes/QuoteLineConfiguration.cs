using Domain.Entities.Quotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Quotes;

/// <summary>
/// تكوين كيان سطر عرض السعر
/// </summary>
public class QuoteLineConfiguration : IEntityTypeConfiguration<QuoteLine>
{
    public void Configure(EntityTypeBuilder<QuoteLine> builder)
    {
        builder.ToTable("QuoteLines");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.QuoteId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ProductSnapshot)
            .HasColumnType("nvarchar(max)");

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.QuoteId)
            .HasDatabaseName("IX_QuoteLines_QuoteId");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_QuoteLines_ProductId");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_QuoteLines_CreatedAtUtc");
    }
}
