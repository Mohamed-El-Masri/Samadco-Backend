using Domain.Abstractions.Events;

namespace Domain.Events.Products;

/// <summary>
/// حدث حذف منتج (ناعم)
/// </summary>
public sealed record ProductDeletedEvent(
    Guid ProductId,
    string ProductName,
    Guid SellerId
) : DomainEventBase;
