using Domain.Entities.Orders;
using Domain.Entities.Users;
using Domain.Entities.Quotes;
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
            .HasConversion<int>();

        builder.Property(x => x.PaymentStatus)
            .IsRequired()
            .HasConversion<int>();

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
            .HasMaxLength(1000);

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
        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.QuoteId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.PaymentStatus);

        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasIndex(x => x.TrackingNumber);
    }
}
