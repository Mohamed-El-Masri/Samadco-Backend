using Domain.Entities.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Carts;

/// <summary>
/// تكوين كيان عنصر السلة
/// </summary>
public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.CartId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.AddedAtUtc)
            .IsRequired();

        // Handle JsonSpec SelectedSpecs as owned entity
        builder.OwnsOne(x => x.SelectedSpecs, specsBuilder =>
        {
            specsBuilder.Property(s => s.JsonString)
                .HasColumnName("SelectedSpecs")
                .HasColumnType("nvarchar(max)");
            
            // تجاهل خاصية Value لأنها محسوبة
            specsBuilder.Ignore(s => s.Value);
        });

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Relationships
        builder.HasOne<Cart>()
            .WithMany()
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Entities.Products.Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.CartId)
            .HasDatabaseName("IX_CartItems_CartId");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_CartItems_ProductId");

        builder.HasIndex(x => new { x.CartId, x.ProductId })
            .HasDatabaseName("IX_CartItems_CartId_ProductId");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_CartItems_CreatedAtUtc");
    }
}
