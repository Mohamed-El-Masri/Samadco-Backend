using Domain.Entities.Distributors;
using Infrastructure.Converters;
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

        builder.Property(x => x.ContactEmail)
            .IsRequired()
            .HasMaxLength(255)
            .HasConversion<EmailConverter>();

        builder.Property(x => x.ContactPhone)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<PhoneNumberConverter>();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Value Objects - Address
        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.Street)
                .HasMaxLength(200)
                .HasColumnName("Address_Street");

            address.Property(a => a.City)
                .HasMaxLength(100)
                .HasColumnName("Address_City");

            address.Property(a => a.State)
                .HasMaxLength(100)
                .HasColumnName("Address_State");

            address.Property(a => a.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("Address_PostalCode");

            address.Property(a => a.Country)
                .HasMaxLength(100)
                .HasColumnName("Address_Country");

            address.Property(a => a.Latitude)
                .HasColumnType("decimal(10,8)")
                .HasColumnName("Address_Latitude");

            address.Property(a => a.Longitude)
                .HasColumnType("decimal(11,8)")
                .HasColumnName("Address_Longitude");
        });

        // Indexes
        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => x.Region);

        builder.HasIndex(x => x.IsActive);

        builder.HasIndex(x => x.ContactEmail);

        builder.HasIndex(x => new { x.Region, x.IsActive });
    }
}
