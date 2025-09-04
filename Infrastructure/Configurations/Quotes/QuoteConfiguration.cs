using Domain.Entities.Quotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Quotes;

/// <summary>
/// تكوين كيان التسعيرة
/// </summary>
public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.ToTable("Quotes");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.QuoteRequestId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.DistributorId)
            .IsRequired();

        builder.Property(x => x.TotalBeforeTax)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Tax)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Shipping)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Total)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Relationships
        builder.HasOne<QuoteRequest>()
            .WithMany()
            .HasForeignKey(x => x.QuoteRequestId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Quotes_QuoteRequests");

        builder.HasOne<Domain.Entities.Users.User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Quotes_Users");

        builder.HasOne<Domain.Entities.Distributors.Distributor>()
            .WithMany()
            .HasForeignKey(x => x.DistributorId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Quotes_Distributors");

        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_QuoteLines_Quotes");

        // Indexes
        builder.HasIndex(x => x.QuoteRequestId)
            .HasDatabaseName("IX_Quotes_QuoteRequestId");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Quotes_UserId");

        builder.HasIndex(x => x.DistributorId)
            .HasDatabaseName("IX_Quotes_DistributorId");

        builder.HasIndex(x => x.ExpiresAtUtc)
            .HasDatabaseName("IX_Quotes_ExpiresAtUtc");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_Quotes_CreatedAtUtc");

        builder.HasIndex(x => new { x.QuoteRequestId, x.DistributorId })
            .HasDatabaseName("IX_Quotes_QuoteRequestId_DistributorId");
    }
}
