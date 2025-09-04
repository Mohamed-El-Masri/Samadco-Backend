using Domain.Abstractions.Errors;
using Domain.Common;

namespace Domain.Entities.Offers;

/// <summary>
/// عنصر العرض - جزء من تجميعة العرض
/// </summary>
public sealed class OfferItem : BaseEntity
{
    public Guid OfferId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public string? SnapshotName { get; private set; } // لقطة من اسم المنتج عند الإنشاء

    // للاستخدام مع EF Core
    private OfferItem() { }

    internal OfferItem(Guid offerId, Guid productId, int quantity, string? snapshotName = null)
    {
        OfferId = offerId;
        ProductId = productId;
        Quantity = quantity;
        SnapshotName = snapshotName;
    }

    /// <summary>
    /// تحديث الكمية
    /// </summary>
    internal void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainRuleViolationException("Quantity must be positive");

        if (quantity > 10000)
            throw new DomainRuleViolationException("Quantity cannot exceed 10000");

        Quantity = quantity;
        MarkAsModified();
    }
}
