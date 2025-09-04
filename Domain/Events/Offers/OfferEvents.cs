using Domain.Abstractions.Events;

namespace Domain.Events.Offers;

/// <summary>
/// حدث إنشاء عرض جديد
/// </summary>
public sealed record OfferCreatedEvent(
    Guid OfferId,
    string Title) : DomainEventBase;

/// <summary>
/// حدث تفعيل عرض
/// </summary>
public sealed record OfferActivatedEvent(
    Guid OfferId,
    DateTime ActiveFromUtc,
    DateTime ActiveToUtc) : DomainEventBase;

/// <summary>
/// حدث انتهاء صلاحية عرض
/// </summary>
public sealed record OfferExpiredEvent(
    Guid OfferId) : DomainEventBase;

/// <summary>
/// حدث أرشفة عرض
/// </summary>
public sealed record OfferArchivedEvent(
    Guid OfferId) : DomainEventBase;
