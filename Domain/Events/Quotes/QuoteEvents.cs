using Domain.Abstractions.Events;

namespace Domain.Events.Quotes;

/// <summary>
/// حدث إنشاء طلب تسعير جديد
/// </summary>
public sealed record QuoteRequestCreatedEvent(
    Guid QuoteRequestId,
    Guid UserId,
    int ItemsCount) : DomainEventBase;

/// <summary>
/// حدث تسعير طلب التسعير
/// </summary>
public sealed record QuoteRequestPricedEvent(
    Guid QuoteRequestId,
    Guid UserId) : DomainEventBase;

/// <summary>
/// حدث انتهاء صلاحية طلب التسعير
/// </summary>
public sealed record QuoteRequestExpiredEvent(
    Guid QuoteRequestId,
    Guid UserId) : DomainEventBase;

/// <summary>
/// حدث إصدار عرض سعر
/// </summary>
public sealed record QuoteIssuedEvent(
    Guid QuoteId,
    Guid QuoteRequestId,
    Guid UserId,
    decimal Total) : DomainEventBase;

/// <summary>
/// حدث انتهاء صلاحية عرض السعر
/// </summary>
public sealed record QuoteExpiredEvent(
    Guid QuoteId,
    Guid UserId) : DomainEventBase;
