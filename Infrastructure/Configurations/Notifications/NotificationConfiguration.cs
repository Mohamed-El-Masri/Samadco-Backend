using Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Notifications;

/// <summary>
/// تكوين كيان الإشعار
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.TitleAr)
            .HasMaxLength(200);

        builder.Property(x => x.Body)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.BodyAr)
            .HasMaxLength(1000);

        // Handle JsonSpec Data as owned entity
        builder.OwnsOne(x => x.Data, dataBuilder =>
        {
            dataBuilder.Property(s => s.JsonString)
                .HasColumnName("Data")
                .HasColumnType("nvarchar(max)");
            
            // تجاهل خاصية Value لأنها محسوبة
            dataBuilder.Ignore(s => s.Value);
        });

        // Base Entity Properties
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Relationships
        builder.HasOne<Domain.Entities.Users.User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Notifications_UserId");

        builder.HasIndex(x => x.Type)
            .HasDatabaseName("IX_Notifications_Type");

        builder.HasIndex(x => x.ReadAtUtc)
            .HasDatabaseName("IX_Notifications_ReadAtUtc");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("IX_Notifications_CreatedAtUtc");

        builder.HasIndex(x => new { x.UserId, x.ReadAtUtc })
            .HasDatabaseName("IX_Notifications_UserId_ReadAtUtc");
    }
}
