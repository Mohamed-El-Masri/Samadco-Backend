using Domain.Entities.Users;
using Domain.Enums.Users;
using Domain.ValueObjects.Identity;

namespace Domain.Abstractions.Repositories;

/// <summary>
/// واجهة مستودع المستخدمين
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// البحث عن مستخدم مع ملف البائع
    /// </summary>
    Task<User?> GetWithSellerProfileAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// البحث عن مستخدم بالبريد الإلكتروني
    /// </summary>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// البحث عن مستخدم برقم الهاتف
    /// </summary>
    Task<User?> GetByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من وجود مستخدم بالبريد الإلكتروني
    /// </summary>
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من وجود مستخدم برقم الهاتف
    /// </summary>
    Task<bool> ExistsByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على المستخدمين بدور معين
    /// </summary>
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على المستخدمين بحالة معينة
    /// </summary>
    Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
}
