using Domain.Entities.Products;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Products;

/// <summary>
/// تكوين كيان المنتج
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.SellerId)
            .IsRequired();

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameAr)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.DescriptionAr)
            .HasMaxLength(2000);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.ApprovedAtUtc);

        builder.Property(x => x.RejectedReason)
            .HasMaxLength(1000);

        builder.Property(x => x.ApprovedBy)
            .HasMaxLength(100);

        // Value Objects - handle Specs as JSON string
        builder.OwnsOne(x => x.Specs, specsBuilder =>
        {
            specsBuilder.Property(s => s.JsonString)
                .HasColumnName("Specifications")
                .HasColumnType("nvarchar(max)");
            
            // تجاهل خاصية Value لأنها محسوبة
            specsBuilder.Ignore(s => s.Value);
        });

        // Images collection - ignore for now as it's a complex collection
        builder.Ignore(x => x.Images);

        // Relationships
        builder.HasOne<SellerProfile>()
            .WithMany()
            .HasForeignKey(x => x.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft Delete Configuration
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAtUtc);

        builder.HasQueryFilter(x => !x.IsDeleted);

        // Indexes
        builder.HasIndex(x => x.SellerId);

        builder.HasIndex(x => x.CategoryId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => new { x.SellerId, x.Status });

        builder.HasIndex(x => new { x.CategoryId, x.Status });
    }
}
