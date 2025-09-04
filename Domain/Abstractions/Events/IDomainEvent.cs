namespace Domain.Abstractions.Events;

/// <summary>
/// الواجهة الأساسية لجميع أحداث النطاق
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// الوقت الذي حدث فيه الحدث
    /// </summary>
    DateTime OccurredOnUtc { get; }
}

/// <summary>
/// الفئة الأساسية لجميع أحداث النطاق
/// </summary>
public abstract record DomainEventBase(DateTime OccurredOnUtc) : IDomainEvent
{
    protected DomainEventBase() : this(DateTime.UtcNow) { }
}
