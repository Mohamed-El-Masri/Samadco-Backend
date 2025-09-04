using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users;

/// <summary>
/// تكوين EF Core لكيان ملف المشتري
/// </summary>
public class BuyerProfileConfiguration : IEntityTypeConfiguration<BuyerProfile>
{
    public void Configure(EntityTypeBuilder<BuyerProfile> builder)
    {
        builder.ToTable("BuyerProfiles");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.PreferredLanguage)
            .HasMaxLength(10);

        builder.Property(x => x.PreferredCurrency)
            .HasMaxLength(10);

        builder.Property(x => x.CreditLimit)
            .HasPrecision(18, 2);

        builder.Property(x => x.CurrentCredit)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.TotalOrdersCount)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalOrdersValue)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

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
            .HasForeignKey<BuyerProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName("IX_BuyerProfiles_UserId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_BuyerProfiles_Status");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_BuyerProfiles_CreatedAtUtc");
    }
}
