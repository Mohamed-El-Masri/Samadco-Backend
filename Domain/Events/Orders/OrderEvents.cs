using Domain.Abstractions.Events;
using Domain.Enums;

namespace Domain.Events.Orders;

/// <summary>
/// حدث إنشاء طلب جديد
/// </summary>
public sealed record OrderCreatedEvent(
    Guid OrderId,
    Guid UserId,
    Guid QuoteId) : DomainEventBase;

/// <summary>
/// حدث دفع العربون
/// </summary>
public sealed record OrderDepositPaidEvent(
    Guid OrderId,
    Guid UserId,
    decimal Amount) : DomainEventBase;

/// <summary>
/// حدث تأكيد الطلب
/// </summary>
public sealed record OrderConfirmedEvent(
    Guid OrderId,
    Guid UserId,
    string NationalIdImageUrl) : DomainEventBase;

/// <summary>
/// حدث تطوير الطلب للمعالجة
/// </summary>
public sealed record OrderProcessingStartedEvent(
    Guid OrderId,
    Guid UserId) : DomainEventBase;

/// <summary>
/// حدث شحن الطلب
/// </summary>
public sealed record OrderShippedEvent(
    Guid OrderId,
    Guid UserId,
    string? TrackingNumber) : DomainEventBase;

/// <summary>
/// حدث تسليم الطلب
/// </summary>
public sealed record OrderDeliveredEvent(
    Guid OrderId,
    Guid UserId) : DomainEventBase;

/// <summary>
/// حدث إلغاء الطلب
/// </summary>
public sealed record OrderCancelledEvent(
    Guid OrderId,
    Guid UserId,
    string Reason) : DomainEventBase;
