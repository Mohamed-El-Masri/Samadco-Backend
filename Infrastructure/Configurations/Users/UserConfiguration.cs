using Domain.Entities.Users;
using Domain.Enums.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasConversion(
                v => v.Value,
                v => Domain.ValueObjects.Identity.Email.Create(v));

        builder.Property(x => x.Phone)
            .HasMaxLength(20)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? Domain.ValueObjects.Identity.PhoneNumber.Create(v) : null);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.LastLoginUtc);

        // Handle Roles collection as JSON string
        builder.Property<List<UserRole>>("_roles")
            .HasField("_roles")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                roles => string.Join(",", roles.Select(r => r.ToString())),
                rolesString => rolesString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => Enum.Parse<UserRole>(r)).ToList())
            .HasColumnName("Roles")
            .HasColumnType("nvarchar(500)");

        // Soft Delete Configuration
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAtUtc);

        builder.HasQueryFilter(x => !x.IsDeleted);

        // Indexes
        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
