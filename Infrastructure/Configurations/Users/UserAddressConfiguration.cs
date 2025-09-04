using Domain.Entities.Users;
using Domain.ValueObjects.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users;

/// <summary>
/// تكوين EF Core لكيان عنوان المستخدم
/// </summary>
public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("UserAddresses");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Label)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsDefault)
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.DeliveryInstructions)
            .HasMaxLength(500);

        builder.Property(x => x.ContactPersonName)
            .HasMaxLength(100);

        builder.Property(x => x.ContactPersonPhone)
            .HasMaxLength(20);

        // Address Value Object Configuration
        builder.OwnsOne(x => x.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Country");

            addressBuilder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("City");

            addressBuilder.Property(a => a.Line1)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("AddressLine1");

            addressBuilder.Property(a => a.Line2)
                .HasMaxLength(200)
                .HasColumnName("AddressLine2");

            addressBuilder.Property(a => a.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("PostalCode");
        });

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Soft Delete Configuration
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAtUtc);

        builder.HasQueryFilter(x => !x.IsDeleted);

        // Relationships
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_UserAddresses_UserId");

        builder.HasIndex(x => new { x.UserId, x.Type })
            .HasDatabaseName("IX_UserAddresses_UserId_Type");

        builder.HasIndex(x => new { x.UserId, x.IsDefault })
            .HasDatabaseName("IX_UserAddresses_UserId_IsDefault");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_UserAddresses_CreatedAtUtc");
    }
}
