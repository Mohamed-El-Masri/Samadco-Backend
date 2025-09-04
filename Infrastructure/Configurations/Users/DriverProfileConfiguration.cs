using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users;

/// <summary>
/// تكوين EF Core لكيان ملف السائق
/// </summary>
public class DriverProfileConfiguration : IEntityTypeConfiguration<DriverProfile>
{
    public void Configure(EntityTypeBuilder<DriverProfile> builder)
    {
        builder.ToTable("DriverProfiles");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.LicenseNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LicenseExpiryDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.VerifiedBy)
            .HasMaxLength(100);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);

        builder.Property(x => x.TotalDeliveries)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalEarnings)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.AverageRating)
            .HasPrecision(3, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalRatings)
            .HasDefaultValue(0);

        builder.Property(x => x.IsAvailable)
            .HasDefaultValue(true);

        builder.Property(x => x.VehicleType)
            .HasMaxLength(50);

        builder.Property(x => x.VehicleModel)
            .HasMaxLength(50);

        builder.Property(x => x.VehiclePlateNumber)
            .HasMaxLength(20);

        builder.Property(x => x.VehicleColor)
            .HasMaxLength(30);

        builder.Property(x => x.VehicleCapacityKg)
            .HasPrecision(8, 2);

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
            .WithOne()
            .HasForeignKey<DriverProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName("IX_DriverProfiles_UserId");

        builder.HasIndex(x => x.LicenseNumber)
            .IsUnique()
            .HasDatabaseName("IX_DriverProfiles_LicenseNumber");

        builder.HasIndex(x => x.VehiclePlateNumber)
            .HasDatabaseName("IX_DriverProfiles_VehiclePlateNumber");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_DriverProfiles_Status");

        builder.HasIndex(x => x.IsAvailable)
            .HasDatabaseName("IX_DriverProfiles_IsAvailable");

        builder.HasIndex(x => x.AverageRating)
            .HasDatabaseName("IX_DriverProfiles_AverageRating");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_DriverProfiles_CreatedAtUtc");
    }
}
