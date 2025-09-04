using Domain.Abstractions.Errors;
using Domain.Common;

namespace Domain.Entities.Quotes;

/// <summary>
/// سطر عرض السعر - جزء من تجميعة عرض السعر
/// </summary>
public sealed class QuoteLine : BaseEntity
{
    public Guid QuoteId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }
    public string? ProductSnapshot { get; private set; } // لقطة من بيانات المنتج

    // للاستخدام مع EF Core
    private QuoteLine() { }

    internal QuoteLine(Guid quoteId, Guid productId, int quantity, decimal unitPrice, string? productSnapshot = null)
    {
        QuoteId = quoteId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Subtotal = quantity * unitPrice;
        ProductSnapshot = productSnapshot;
    }

    /// <summary>
    /// تحديث السعر
    /// </summary>
    internal void UpdatePrice(decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new DomainRuleViolationException("Unit price cannot be negative");

        UnitPrice = unitPrice;
        Subtotal = Quantity * unitPrice;
        MarkAsModified();
    }

    /// <summary>
    /// تحديث الكمية
    /// </summary>
    internal void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainRuleViolationException("Quantity must be positive");

        Quantity = quantity;
        Subtotal = quantity * UnitPrice;
        MarkAsModified();
    }
}
