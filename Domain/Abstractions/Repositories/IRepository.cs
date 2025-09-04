using Domain.Common;

namespace Domain.Abstractions.Repositories;

/// <summary>
/// المستودع العام لجميع جذور التجميعات
/// </summary>
public interface IRepository<TAggregate> where TAggregate : class, IAggregateRoot
{
    /// <summary>
    /// البحث عن كيان بالمعرف
    /// </summary>
    Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// إضافة كيان جديد
    /// </summary>
    Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// تحديث كيان موجود
    /// </summary>
    void Update(TAggregate entity);

    /// <summary>
    /// حذف كيان
    /// </summary>
    void Remove(TAggregate entity);
}
