using Domain.Abstractions.Events;

namespace Domain.Events.Users;

/// <summary>
/// حدث إنشاء ملف مشتري
/// </summary>
public record BuyerProfileCreatedEvent(Guid BuyerProfileId, Guid UserId) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث إضافة رصيد للمشتري
/// </summary>
public record BuyerCreditAddedEvent(Guid BuyerProfileId, Guid UserId, decimal Amount, string Reason) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث خصم رصيد من المشتري
/// </summary>
public record BuyerCreditDeductedEvent(Guid BuyerProfileId, Guid UserId, decimal Amount, string Reason) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث تغيير حالة المشتري
/// </summary>
public record BuyerStatusChangedEvent(Guid BuyerProfileId, Guid UserId, object OldStatus, object NewStatus, string? Reason) : DomainEventBase(DateTime.UtcNow);
