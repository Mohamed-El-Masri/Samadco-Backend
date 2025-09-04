using Domain.Abstractions.Events;

namespace Domain.Events.Products;

/// <summary>
/// حدث استعادة منتج محذوف
/// </summary>
public sealed record ProductRestoredEvent(
    Guid ProductId,
    string ProductName,
    Guid SellerId
) : DomainEventBase;
