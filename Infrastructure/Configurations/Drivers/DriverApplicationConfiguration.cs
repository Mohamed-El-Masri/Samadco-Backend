using Domain.Entities.Drivers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Drivers;

/// <summary>
/// تكوين كيان طلب انضمام السائق
/// </summary>
public class DriverApplicationConfiguration : IEntityTypeConfiguration<DriverApplication>
{
    public void Configure(EntityTypeBuilder<DriverApplication> builder)
    {
        builder.ToTable("DriverApplications");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.DecisionAtUtc);

        builder.Property(x => x.DecisionReason)
            .HasMaxLength(500);

        builder.Property(x => x.DecisionBy)
            .HasMaxLength(100);

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Value Objects - Vehicle Info
        builder.OwnsOne(x => x.VehicleInfo, vehicleBuilder =>
        {
            vehicleBuilder.Property(v => v.Type)
                .HasColumnName("VehicleType")
                .HasMaxLength(50)
                .IsRequired();

            vehicleBuilder.Property(v => v.Make)
                .HasColumnName("VehicleMake")
                .HasMaxLength(100)
                .IsRequired();

            vehicleBuilder.Property(v => v.Model)
                .HasColumnName("VehicleModel")
                .HasMaxLength(100)
                .IsRequired();

            vehicleBuilder.Property(v => v.Year)
                .HasColumnName("VehicleYear")
                .IsRequired();

            vehicleBuilder.Property(v => v.PlateNumber)
                .HasColumnName("VehiclePlateNumber")
                .HasMaxLength(20)
                .IsRequired();

            vehicleBuilder.Property(v => v.Color)
                .HasColumnName("VehicleColor")
                .HasMaxLength(50);
        });

        // License Images collection - configure as owned entities
        builder.OwnsMany(x => x.LicenseImages, imagesBuilder =>
        {
            imagesBuilder.ToTable("DriverApplicationLicenseImages");
            
            imagesBuilder.Property(i => i.Url)
                .HasMaxLength(500)
                .IsRequired();
                
            imagesBuilder.Property(i => i.Width);
            
            imagesBuilder.Property(i => i.Height);
            
            imagesBuilder.Property(i => i.MimeType)
                .HasMaxLength(100);
        });

        // Relationships
        builder.HasOne<Domain.Entities.Users.User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_DriverApplications_UserId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_DriverApplications_Status");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_DriverApplications_CreatedAtUtc");

        builder.HasIndex(x => new { x.UserId, x.Status })
            .HasDatabaseName("IX_DriverApplications_UserId_Status");
    }
}
