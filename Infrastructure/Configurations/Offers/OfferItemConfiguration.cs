using Domain.Entities.Offers;
using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Offers;

/// <summary>
/// تكوين كيان عنصر العرض
/// </summary>
public class OfferItemConfiguration : IEntityTypeConfiguration<OfferItem>
{
    public void Configure(EntityTypeBuilder<OfferItem> builder)
    {
        builder.ToTable("OfferItems");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.OfferId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.SnapshotName)
            .HasMaxLength(200);

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        // Relationships
        builder.HasOne<Offer>()
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.OfferId)
            .HasDatabaseName("IX_OfferItems_OfferId");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_OfferItems_ProductId");

        builder.HasIndex(x => new { x.OfferId, x.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_OfferItems_OfferId_ProductId");
    }
}
