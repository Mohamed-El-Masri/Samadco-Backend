using Domain.Abstractions.Events;

namespace Domain.Common;

/// <summary>
/// الكيان الأساسي الذي ترث منه جميع الكيانات في الـ Domain
/// يحتوي على المعرف والطوابع الزمنية وإدارة أحداث النطاق
/// </summary>
public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; protected set; }
    public string? ConcurrencyStamp { get; protected set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// قائمة أحداث النطاق التي يجب نشرها بعد حفظ التغييرات
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// إضافة حدث نطاق جديد لقائمة الأحداث
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// مسح جميع أحداث النطاق (يتم استدعاؤها بعد نشر الأحداث)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// تحديث الطابع الزمني للتعديل
    /// </summary>
    protected void MarkAsModified()
    {
        UpdatedAtUtc = DateTime.UtcNow;
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }
}
