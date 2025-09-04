using Domain.Entities.Offers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Offers;

/// <summary>
/// تكوين كيان العرض
/// </summary>
public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.TitleAr)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.DescriptionAr)
            .HasMaxLength(2000);

        builder.Property(x => x.ActiveFromUtc)
            .IsRequired();

        builder.Property(x => x.ActiveToUtc)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        // Relationships
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Offers_Status");

        builder.HasIndex(x => x.ActiveFromUtc)
            .HasDatabaseName("IX_Offers_ActiveFromUtc");

        builder.HasIndex(x => x.ActiveToUtc)
            .HasDatabaseName("IX_Offers_ActiveToUtc");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_Offers_CreatedAtUtc");

        builder.HasIndex(x => new { x.Status, x.ActiveFromUtc, x.ActiveToUtc })
            .HasDatabaseName("IX_Offers_Status_ActivePeriod");
    }
}
