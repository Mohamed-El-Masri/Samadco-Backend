using Domain.Abstractions.Events;
using Domain.Enums.Payments;

namespace Domain.Events.Payments;

/// <summary>
/// حدث بدء دفعة جديدة
/// </summary>
public sealed record PaymentInitiatedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    PaymentMethod Method) : DomainEventBase;

/// <summary>
/// حدث نجاح الدفع
/// </summary>
public sealed record PaymentSucceededEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string GatewayReference) : DomainEventBase;

/// <summary>
/// حدث فشل الدفع
/// </summary>
public sealed record PaymentFailedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string ErrorCode,
    string? ErrorMessage) : DomainEventBase;
