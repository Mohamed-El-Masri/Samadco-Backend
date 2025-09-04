using Domain.Abstractions.Events;

namespace Domain.Events.Carts;

/// <summary>
/// حدث إنشاء سلة جديدة
/// </summary>
public sealed record CartCreatedEvent(
    Guid CartId,
    Guid UserId) : DomainEventBase;

/// <summary>
/// حدث إضافة عنصر للسلة
/// </summary>
public sealed record CartItemAddedEvent(
    Guid CartId,
    Guid UserId,
    Guid ProductId,
    int Quantity) : DomainEventBase;

/// <summary>
/// حدث تحديث عنصر في السلة
/// </summary>
public sealed record CartItemUpdatedEvent(
    Guid CartId,
    Guid UserId,
    Guid ProductId,
    int NewQuantity) : DomainEventBase;

/// <summary>
/// حدث إزالة عنصر من السلة
/// </summary>
public sealed record CartItemRemovedEvent(
    Guid CartId,
    Guid UserId,
    Guid ProductId) : DomainEventBase;

/// <summary>
/// حدث قفل السلة (تحويلها لطلب تسعير)
/// </summary>
public sealed record CartLockedEvent(
    Guid CartId,
    Guid UserId) : DomainEventBase;
