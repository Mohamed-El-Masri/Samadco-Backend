using Domain.Abstractions;

namespace Domain.Abstractions;

/// <summary>
/// واجهة وحدة العمل لإدارة المعاملات وحفظ التغييرات
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// حفظ جميع التغييرات في قاعدة البيانات
    /// </summary>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>عدد الصفوف المتأثرة</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// بدء معاملة جديدة
    /// </summary>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>معاملة قاعدة البيانات</returns>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// واجهة المعاملة
/// </summary>
public interface ITransaction : IDisposable
{
    /// <summary>
    /// تأكيد المعاملة
    /// </summary>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// إلغاء المعاملة
    /// </summary>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
