using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users;

public class SellerProfileConfiguration : IEntityTypeConfiguration<SellerProfile>
{
    public void Configure(EntityTypeBuilder<SellerProfile> builder)
    {
        builder.ToTable("SellerProfiles");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CompanyNameAr)
            .HasMaxLength(200);

        builder.Property(x => x.TaxId)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion(
                v => v.Value,
                v => Domain.ValueObjects.Identity.TaxId.Create(v));

        builder.Property(x => x.VerificationStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.VerifiedAtUtc);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(x => x.VerifiedBy)
            .HasMaxLength(100);

        // Value Objects - Address
        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Address_Country");

            address.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Address_City");

            address.Property(a => a.Line1)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Address_Line1");

            address.Property(a => a.Line2)
                .HasMaxLength(100)
                .HasColumnName("Address_Line2");

            address.Property(a => a.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("Address_PostalCode");
        });

        // Relationships
        builder.HasOne<User>()
            .WithOne(u => u.SellerProfile)
            .HasForeignKey<SellerProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasIndex(x => x.TaxId)
            .IsUnique();

        builder.HasIndex(x => x.VerificationStatus);

        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
