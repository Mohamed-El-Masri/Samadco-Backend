using Domain.Abstractions.Repositories;
using Domain.Entities.Users;
using Domain.Enums.Users;
using Domain.ValueObjects.Identity;
using Infrastructure.Data;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// مستودع المستخدمين
/// </summary>
public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(SemadcoDbContext context) : base(context)
    {
    }

    public async Task<User?> GetWithSellerProfileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.SellerProfile)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Phone == phone, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(u => u.Phone == phone, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        // Since roles are stored as a collection, we need to check if the role exists in the collection
        return await DbSet
            .Where(u => u.Roles.Contains(role))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(u => u.Status == status)
            .ToListAsync(cancellationToken);
    }
}
