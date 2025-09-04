using Domain.Entities.Quotes;
using Domain.Enums.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Quotes;

/// <summary>
/// تكوين كيان طلب عرض السعر
/// </summary>
public class QuoteRequestConfiguration : IEntityTypeConfiguration<QuoteRequest>
{
    public void Configure(EntityTypeBuilder<QuoteRequest> builder)
    {
        // اسم الجدول
        builder.ToTable("QuoteRequests");

        // المفتاح الأساسي
        builder.HasKey(qr => qr.Id);

        // معلومات طلب عرض السعر
        builder.Property(qr => qr.RequestNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(qr => qr.BuyerId)
            .IsRequired();

        builder.Property(qr => qr.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(qr => qr.Description)
            .IsRequired()
            .HasMaxLength(2000);

        // حالة الطلب
        builder.Property(qr => qr.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // مواعيد مهمة
        builder.Property(qr => qr.RequiredDate)
            .HasColumnType("date");

        builder.Property(qr => qr.ExpirationDate)
            .HasColumnType("datetime2");

        // معلومات إضافية
        builder.Property(qr => qr.Budget)
            .HasPrecision(18, 2);

        builder.Property(qr => qr.PreferredLocation)
            .HasMaxLength(200);

        builder.Property(qr => qr.SpecialRequirements)
            .HasMaxLength(1000);

        // إعدادات
        builder.Property(qr => qr.IsPublic)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(qr => qr.AllowPartialQuotes)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(qr => qr.MaxQuotes)
            .HasDefaultValue(null);

        // إحصائيات
        builder.Property(qr => qr.ViewsCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(qr => qr.QuotesCount)
            .IsRequired()
            .HasDefaultValue(0);

        // علاقات
        builder.HasOne<Domain.Entities.Users.User>()
            .WithMany()
            .HasForeignKey(qr => qr.BuyerId)
            .OnDelete(DeleteBehavior.Cascade);

        // BaseEntity properties
        ConfigureBaseEntity(builder);

        // Soft Delete
        ConfigureSoftDelete(builder);

        // Indexes
        builder.HasIndex(qr => qr.RequestNumber)
            .IsUnique()
            .HasDatabaseName("IX_QuoteRequests_RequestNumber");

        builder.HasIndex(qr => qr.BuyerId)
            .HasDatabaseName("IX_QuoteRequests_BuyerId");

        builder.HasIndex(qr => qr.Status)
            .HasDatabaseName("IX_QuoteRequests_Status");

        builder.HasIndex(qr => qr.ExpirationDate)
            .HasDatabaseName("IX_QuoteRequests_ExpirationDate");

        builder.HasIndex(qr => qr.IsPublic)
            .HasDatabaseName("IX_QuoteRequests_IsPublic");

        builder.HasIndex(qr => new { qr.Status, qr.IsPublic })
            .HasDatabaseName("IX_QuoteRequests_Status_IsPublic");

        builder.HasIndex(qr => new { qr.BuyerId, qr.Status })
            .HasDatabaseName("IX_QuoteRequests_BuyerId_Status");
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
