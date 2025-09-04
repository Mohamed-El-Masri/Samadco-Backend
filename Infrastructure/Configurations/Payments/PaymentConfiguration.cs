using Domain.Entities.Orders;
using Domain.Entities.Payments;
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
        builder.ToTable("Payments");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Method)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.GatewayReference)
            .HasMaxLength(200);

        builder.Property(x => x.ErrorCode)
            .HasMaxLength(50);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(500);

        builder.Property(x => x.SucceededAtUtc);

        builder.Property(x => x.FailedAtUtc);

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        // Relationships
        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.OrderId)
            .HasDatabaseName("IX_Payments_OrderId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Payments_Status");

        builder.HasIndex(x => x.Method)
            .HasDatabaseName("IX_Payments_Method");

        builder.HasIndex(x => x.GatewayReference)
            .HasDatabaseName("IX_Payments_GatewayReference");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_Payments_CreatedAtUtc");

        builder.HasIndex(x => new { x.Status, x.Method })
            .HasDatabaseName("IX_Payments_Status_Method");

        builder.HasIndex(x => new { x.OrderId, x.Status })
            .HasDatabaseName("IX_Payments_OrderId_Status");
    }
}
