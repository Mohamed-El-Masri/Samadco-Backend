using Domain.Entities.Distributors;
using Domain.ValueObjects.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Distributors;

/// <summary>
/// تكوين كيان الموزع
/// </summary>
public class DistributorConfiguration : IEntityTypeConfiguration<Distributor>
{
    public void Configure(EntityTypeBuilder<Distributor> builder)
    {
        builder.ToTable("Distributors");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NameAr)
            .HasMaxLength(200);

        builder.Property(x => x.Region)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.RegionAr)
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        // Value Objects
        builder.OwnsOne(x => x.ContactEmail, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(320)
                .HasColumnName("ContactEmail");
        });

        builder.OwnsOne(x => x.ContactPhone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("ContactPhone");
        });

        // Address Value Object Configuration
        builder.OwnsOne(x => x.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Country)
                .HasMaxLength(100)
                .HasColumnName("AddressCountry");

            addressBuilder.Property(a => a.City)
                .HasMaxLength(100)
                .HasColumnName("AddressCity");

            addressBuilder.Property(a => a.Line1)
                .HasMaxLength(200)
                .HasColumnName("AddressLine1");

            addressBuilder.Property(a => a.Line2)
                .HasMaxLength(200)
                .HasColumnName("AddressLine2");

            addressBuilder.Property(a => a.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("AddressPostalCode");
        });

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Distributors_Name");

        builder.HasIndex(x => x.Region)
            .HasDatabaseName("IX_Distributors_Region");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Distributors_IsActive");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_Distributors_CreatedAtUtc");
    }
}
