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

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.LastTouchedUtc)
            .IsRequired();

        builder.Property(x => x.IsLocked)
            .HasDefaultValue(false);

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Relationships
        builder.HasOne<Domain.Entities.Users.User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<CartItem>()
            .WithOne()
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Carts_UserId");

        builder.HasIndex(x => x.IsLocked)
            .HasDatabaseName("IX_Carts_IsLocked");

        builder.HasIndex(x => x.LastTouchedUtc)
            .HasDatabaseName("IX_Carts_LastTouchedUtc");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_Carts_CreatedAtUtc");
    }
}
