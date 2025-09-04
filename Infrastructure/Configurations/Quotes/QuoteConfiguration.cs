using Domain.Entities.Quotes;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Quotes;

/// <summary>
/// تكوين كيان عرض السعر
/// </summary>
public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.ToTable("Quotes");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.QuoteRequestId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.TotalBeforeTax)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Tax)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Shipping)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Total)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        // Handle the Lines collection as JSON for now
        builder.Property("_lines")
            .HasField("_lines")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<Domain.Entities.Quotes.QuoteLine>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<Domain.Entities.Quotes.QuoteLine>())
            .HasColumnName("Lines")
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne<QuoteRequest>()
            .WithMany()
            .HasForeignKey(x => x.QuoteRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.QuoteRequestId);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.ExpiresAtUtc);

        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasIndex(x => new { x.UserId, x.ExpiresAtUtc });
    }
}
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        // اسم الجدول
        builder.ToTable("Quotes");

        // المفتاح الأساسي
        builder.HasKey(q => q.Id);

        // معلومات عرض السعر
        builder.Property(q => q.QuoteNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(q => q.QuoteRequestId)
            .IsRequired();

        builder.Property(q => q.SellerId)
            .IsRequired();

        // حالة عرض السعر
        builder.Property(q => q.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // المبالغ المالية
        builder.Property(q => q.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(q => q.TaxAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(q => q.DiscountAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(q => q.DeliveryFee)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(q => q.FinalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        // مواعيد مهمة
        builder.Property(q => q.ValidUntil)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(q => q.EstimatedDeliveryDate)
            .HasColumnType("date");

        builder.Property(q => q.AcceptedAt)
            .HasColumnType("datetime2");

        builder.Property(q => q.RejectedAt)
            .HasColumnType("datetime2");

        // شروط إضافية
        builder.Property(q => q.PaymentTerms)
            .HasMaxLength(500);

        builder.Property(q => q.DeliveryTerms)
            .HasMaxLength(500);

        builder.Property(q => q.WarrantyTerms)
            .HasMaxLength(500);

        builder.Property(q => q.AdditionalNotes)
            .HasMaxLength(1000);

        // أسباب الرفض
        builder.Property(q => q.RejectionReason)
            .HasMaxLength(500);

        // علاقات
        builder.HasOne<QuoteRequest>()
            .WithMany()
            .HasForeignKey(q => q.QuoteRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Entities.Users.SellerProfile>()
            .WithMany()
            .HasForeignKey(q => q.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        // BaseEntity properties
        ConfigureBaseEntity(builder);

        // Soft Delete
        ConfigureSoftDelete(builder);

        // Indexes
        builder.HasIndex(q => q.QuoteNumber)
            .IsUnique()
            .HasDatabaseName("IX_Quotes_QuoteNumber");

        builder.HasIndex(q => q.QuoteRequestId)
            .HasDatabaseName("IX_Quotes_QuoteRequestId");

        builder.HasIndex(q => q.SellerId)
            .HasDatabaseName("IX_Quotes_SellerId");

        builder.HasIndex(q => q.Status)
            .HasDatabaseName("IX_Quotes_Status");

        builder.HasIndex(q => q.ValidUntil)
            .HasDatabaseName("IX_Quotes_ValidUntil");

        builder.HasIndex(q => new { q.QuoteRequestId, q.SellerId })
            .HasDatabaseName("IX_Quotes_QuoteRequestId_SellerId");

        builder.HasIndex(q => new { q.SellerId, q.Status })
            .HasDatabaseName("IX_Quotes_SellerId_Status");
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
