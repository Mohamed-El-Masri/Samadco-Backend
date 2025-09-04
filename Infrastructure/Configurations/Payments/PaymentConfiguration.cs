using Domain.Entities.Payments;
using Domain.Enums.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Payments;

/// <summary>
/// تكوين كيان الدفع
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // اسم الجدول
        builder.ToTable("Payments");

        // المفتاح الأساسي
        builder.HasKey(p => p.Id);

        // معلومات الدفع
        builder.Property(p => p.PaymentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.OrderId)
            .IsRequired();

        builder.Property(p => p.PayerId)
            .IsRequired();

        // المبالغ المالية
        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.ExchangeRate)
            .HasPrecision(18, 6)
            .HasDefaultValue(1.0m);

        // طريقة الدفع
        builder.Property(p => p.Method)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.Provider)
            .HasMaxLength(100);

        // حالة الدفع
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // معلومات المعاملة
        builder.Property(p => p.TransactionId)
            .HasMaxLength(200);

        builder.Property(p => p.ExternalTransactionId)
            .HasMaxLength(200);

        builder.Property(p => p.AuthorizationCode)
            .HasMaxLength(100);

        // تواريخ مهمة
        builder.Property(p => p.ProcessedAt)
            .HasColumnType("datetime2");

        builder.Property(p => p.CompletedAt)
            .HasColumnType("datetime2");

        builder.Property(p => p.FailedAt)
            .HasColumnType("datetime2");

        builder.Property(p => p.RefundedAt)
            .HasColumnType("datetime2");

        // معلومات إضافية
        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        builder.Property(p => p.RefundReason)
            .HasMaxLength(500);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        // الرسوم
        builder.Property(p => p.ProcessingFee)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        // علاقات
        builder.HasOne<Domain.Entities.Orders.Order>()
            .WithMany()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.Users.User>()
            .WithMany()
            .HasForeignKey(p => p.PayerId)
            .OnDelete(DeleteBehavior.Restrict);

        // BaseEntity properties
        ConfigureBaseEntity(builder);

        // Soft Delete
        ConfigureSoftDelete(builder);

        // Indexes
        builder.HasIndex(p => p.PaymentNumber)
            .IsUnique()
            .HasDatabaseName("IX_Payments_PaymentNumber");

        builder.HasIndex(p => p.OrderId)
            .HasDatabaseName("IX_Payments_OrderId");

        builder.HasIndex(p => p.PayerId)
            .HasDatabaseName("IX_Payments_PayerId");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Payments_Status");

        builder.HasIndex(p => p.Method)
            .HasDatabaseName("IX_Payments_Method");

        builder.HasIndex(p => p.TransactionId)
            .HasDatabaseName("IX_Payments_TransactionId");

        builder.HasIndex(p => p.ExternalTransactionId)
            .HasDatabaseName("IX_Payments_ExternalTransactionId");

        builder.HasIndex(p => new { p.OrderId, p.Status })
            .HasDatabaseName("IX_Payments_OrderId_Status");

        builder.HasIndex(p => new { p.PayerId, p.Status })
            .HasDatabaseName("IX_Payments_PayerId_Status");
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
