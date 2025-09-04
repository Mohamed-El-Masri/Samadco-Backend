using Domain.Entities.Offers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Offers;

/// <summary>
/// تكوين كيان العرض
/// </summary>
public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.TitleAr)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.DescriptionAr)
            .HasMaxLength(2000);

        builder.Property(x => x.ActiveFromUtc)
            .IsRequired();

        builder.Property(x => x.ActiveToUtc)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        // Handle the Items collection as JSON for now
        builder.Property("_items")
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<Domain.Entities.Offers.OfferItem>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<Domain.Entities.Offers.OfferItem>())
            .HasColumnName("Items")
            .HasColumnType("nvarchar(max)");

        // Indexes
        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.ActiveFromUtc);

        builder.HasIndex(x => x.ActiveToUtc);

        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasIndex(x => new { x.Status, x.ActiveFromUtc, x.ActiveToUtc });
    }
}
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        // اسم الجدول
        builder.ToTable("Offers");

        // المفتاح الأساسي
        builder.HasKey(o => o.Id);

        // معلومات العرض
        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Description)
            .HasMaxLength(1000);

        builder.Property(o => o.SellerId)
            .IsRequired();

        // نوع العرض
        builder.Property(o => o.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // الأسعار والخصومات
        builder.Property(o => o.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(o => o.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.MinimumOrderAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.MaximumDiscountAmount)
            .HasPrecision(18, 2);

        // صلاحية العرض
        builder.Property(o => o.StartDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(o => o.EndDate)
            .IsRequired()
            .HasColumnType("datetime2");

        // حالة العرض
        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // معلومات الاستخدام
        builder.Property(o => o.MaxUsages)
            .HasDefaultValue(null);

        builder.Property(o => o.UsedCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(o => o.MaxUsagesPerUser)
            .HasDefaultValue(null);

        // الفعالية
        builder.Property(o => o.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // شروط العرض
        builder.Property(o => o.Terms)
            .HasMaxLength(2000);

        // علاقات
        builder.HasOne<Domain.Entities.Users.SellerProfile>()
            .WithMany()
            .HasForeignKey(o => o.SellerId)
            .OnDelete(DeleteBehavior.Cascade);

        // BaseEntity properties
        ConfigureBaseEntity(builder);

        // Soft Delete
        ConfigureSoftDelete(builder);

        // Indexes
        builder.HasIndex(o => o.SellerId)
            .HasDatabaseName("IX_Offers_SellerId");

        builder.HasIndex(o => o.Type)
            .HasDatabaseName("IX_Offers_Type");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("IX_Offers_Status");

        builder.HasIndex(o => o.StartDate)
            .HasDatabaseName("IX_Offers_StartDate");

        builder.HasIndex(o => o.EndDate)
            .HasDatabaseName("IX_Offers_EndDate");

        builder.HasIndex(o => o.IsActive)
            .HasDatabaseName("IX_Offers_IsActive");

        builder.HasIndex(o => new { o.Status, o.IsActive, o.StartDate, o.EndDate })
            .HasDatabaseName("IX_Offers_Status_IsActive_Dates");
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

    private static void ConfigureSoftDelete<T>(EntityTypeBuilder<T> builder) where T : class, Domain.Abstractions.ISoftDelete
    {
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName($"IX_{typeof(T).Name}s_IsDeleted");
    }
}
