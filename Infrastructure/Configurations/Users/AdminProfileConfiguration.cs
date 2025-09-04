using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users;

/// <summary>
/// تكوين EF Core لكيان ملف المدير
/// </summary>
public class AdminProfileConfiguration : IEntityTypeConfiguration<AdminProfile>
{
    public void Configure(EntityTypeBuilder<AdminProfile> builder)
    {
        builder.ToTable("AdminProfiles");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Permissions)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.PermissionsUpdatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.TotalActionsPerformed)
            .HasDefaultValue(0);

        builder.Property(x => x.LastActionType)
            .HasMaxLength(100);

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
            .HasForeignKey<AdminProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName("IX_AdminProfiles_UserId");

        builder.HasIndex(x => x.Department)
            .HasDatabaseName("IX_AdminProfiles_Department");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_AdminProfiles_Status");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_AdminProfiles_CreatedAtUtc");

        builder.HasIndex(x => new { x.Department, x.Status })
            .HasDatabaseName("IX_AdminProfiles_Department_Status");
    }
}
