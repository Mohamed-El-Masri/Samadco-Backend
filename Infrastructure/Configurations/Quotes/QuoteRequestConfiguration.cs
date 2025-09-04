using Domain.Entities.Quotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Quotes;

/// <summary>
/// تكوين كيان طلب التسعيرة
/// </summary>
public class QuoteRequestConfiguration : IEntityTypeConfiguration<QuoteRequest>
{
    public void Configure(EntityTypeBuilder<QuoteRequest> builder)
    {
        builder.ToTable("QuoteRequests");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // CartSnapshot Value Object Configuration
        builder.OwnsOne(x => x.CartSnapshot, snapshotBuilder =>
        {
            snapshotBuilder.Property(s => s.JsonData)
                .IsRequired()
                .HasColumnName("CartSnapshotJson")
                .HasColumnType("nvarchar(max)");

            snapshotBuilder.Property(s => s.ItemsCount)
                .IsRequired()
                .HasColumnName("CartSnapshotItemsCount");

            snapshotBuilder.Property(s => s.SnapshotTakenAtUtc)
                .IsRequired()
                .HasColumnName("CartSnapshotTakenAtUtc");
        });

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_QuoteRequests_UserId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_QuoteRequests_Status");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_QuoteRequests_CreatedAtUtc");

        builder.HasIndex(x => new { x.UserId, x.Status })
            .HasDatabaseName("IX_QuoteRequests_UserId_Status");
    }
}
