using Domain.Entities.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Carts;

/// <summary>
/// تكوين كيان السلة
/// </summary>
public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.FinalAmount)
            .HasPrecision(18, 2);

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Relationships
        builder.HasMany<CartItem>()
            .WithOne()
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Carts_UserId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Carts_Status");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_Carts_CreatedAtUtc");
    }
}
