using Domain.Entities.Offers;
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
        // اسم الجدول
        builder.ToTable("OfferItems");

        // المفتاح الأساسي
        builder.HasKey(oi => oi.Id);

        // معلومات العنصر
        builder.Property(oi => oi.OfferId)
            .IsRequired();

        builder.Property(oi => oi.ProductId)
            .IsRequired();

        // الأسعار والخصومات
        builder.Property(oi => oi.OriginalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(oi => oi.OfferPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(oi => oi.DiscountPercentage)
            .HasPrecision(5, 2);

        // الكمية المتاحة
        builder.Property(oi => oi.AvailableQuantity)
            .HasDefaultValue(null);

        builder.Property(oi => oi.SoldQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        // الفعالية
        builder.Property(oi => oi.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // ملاحظات
        builder.Property(oi => oi.Notes)
            .HasMaxLength(500);

        // علاقات
        builder.HasOne<Offer>()
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Entities.Products.Product>()
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // BaseEntity properties
        ConfigureBaseEntity(builder);

        // Indexes
        builder.HasIndex(oi => oi.OfferId)
            .HasDatabaseName("IX_OfferItems_OfferId");

        builder.HasIndex(oi => oi.ProductId)
            .HasDatabaseName("IX_OfferItems_ProductId");

        builder.HasIndex(oi => oi.IsActive)
            .HasDatabaseName("IX_OfferItems_IsActive");

        builder.HasIndex(oi => new { oi.OfferId, oi.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_OfferItems_OfferId_ProductId");

        builder.HasIndex(oi => new { oi.OfferId, oi.IsActive })
            .HasDatabaseName("IX_OfferItems_OfferId_IsActive");
    }

    private static void ConfigureBaseEntity<T>(EntityTypeBuilder<T> builder) where T : Domain.Common.BaseEntity
    {
        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.UpdatedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(e => e.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(36);

        // Domain Events ignored (not persisted)
        builder.Ignore(e => e.DomainEvents);
    }
}
