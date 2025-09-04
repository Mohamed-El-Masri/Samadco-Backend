using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.ValueObjects.Products;

namespace Domain.Entities.Carts;

/// <summary>
/// عنصر السلة - جزء من تجميعة السلة
/// </summary>
public sealed class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public JsonSpec? SelectedSpecs { get; private set; } // المواصفات المختارة للمنتج
    public DateTime AddedAtUtc { get; private set; }

    // للاستخدام مع EF Core
    private CartItem() { }

    internal CartItem(Guid cartId, Guid productId, int quantity, JsonSpec? selectedSpecs = null)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        SelectedSpecs = selectedSpecs;
        AddedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// تحديث الكمية
    /// </summary>
    internal void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainRuleViolationException("Quantity must be positive");

        if (quantity > 1000)
            throw new DomainRuleViolationException("Quantity cannot exceed 1000");

        Quantity = quantity;
        MarkAsModified();
    }

    /// <summary>
    /// تحديث المواصفات المختارة
    /// </summary>
    internal void UpdateSelectedSpecs(JsonSpec? selectedSpecs)
    {
        SelectedSpecs = selectedSpecs;
        MarkAsModified();
    }
}
