using Domain.Entities.Orders;
using Domain.Entities.Quotes;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Orders;

/// <summary>
/// تكوين كيان الطلب
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.QuoteId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.PaymentStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.DepositAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.NationalIdImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(x => x.ConfirmedAtUtc);

        builder.Property(x => x.ProcessingStartedAtUtc);

        builder.Property(x => x.ShippedAtUtc);

        builder.Property(x => x.DeliveredAtUtc);

        builder.Property(x => x.CancelledAtUtc);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(500);

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        // Relationships
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Quote>()
            .WithMany()
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Orders_UserId");

        builder.HasIndex(x => x.QuoteId)
            .HasDatabaseName("IX_Orders_QuoteId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Orders_Status");

        builder.HasIndex(x => x.PaymentStatus)
            .HasDatabaseName("IX_Orders_PaymentStatus");

        builder.HasIndex(x => x.TrackingNumber)
            .HasDatabaseName("IX_Orders_TrackingNumber");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_Orders_CreatedAtUtc");

        builder.HasIndex(x => new { x.UserId, x.Status })
            .HasDatabaseName("IX_Orders_UserId_Status");

        builder.HasIndex(x => new { x.Status, x.PaymentStatus })
            .HasDatabaseName("IX_Orders_Status_PaymentStatus");
    }
}
