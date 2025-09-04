using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Events.Carts;
using Domain.ValueObjects.Products;

namespace Domain.Entities.Carts;

/// <summary>
/// كيان السلة - جذر التجميعة
/// </summary>
public sealed class Cart : BaseEntity, IAggregateRoot
{
    private readonly List<CartItem> _items = new();

    public Guid UserId { get; private set; }
    public string? Notes { get; private set; } // ملاحظات إضافية مثل العنوان أو تفاصيل التسليم
    public DateTime LastTouchedUtc { get; private set; }
    public bool IsLocked { get; private set; } // مقفلة عند تحويلها لطلب تسعير

    /// <summary>
    /// عناصر السلة
    /// </summary>
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    /// <summary>
    /// إجمالي عدد العناصر في السلة
    /// </summary>
    public int TotalItems => _items.Sum(item => item.Quantity);

    // للاستخدام مع EF Core
    private Cart() { }

    private Cart(Guid userId)
    {
        UserId = userId;
        LastTouchedUtc = DateTime.UtcNow;
        IsLocked = false;
    }

    /// <summary>
    /// إنشاء سلة جديدة
    /// </summary>
    public static Cart Create(Guid userId)
    {
        var cart = new Cart(userId);
        cart.AddDomainEvent(new CartCreatedEvent(cart.Id, userId));
        return cart;
    }

    /// <summary>
    /// إضافة عنصر للسلة
    /// </summary>
    public void AddItem(Guid productId, int quantity, JsonSpec? selectedSpecs = null)
    {
        if (IsLocked)
            throw new DomainRuleViolationException("Cannot modify locked cart");

        if (quantity <= 0)
            throw new DomainRuleViolationException("Quantity must be positive");

        if (quantity > 1000)
            throw new DomainRuleViolationException("Quantity cannot exceed 1000");

        if (_items.Count >= 200) // حد أقصى 200 عنصر
            throw new DomainRuleViolationException("Cannot add more than 200 items to cart");

        // البحث عن العنصر الموجود
        var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem != null)
        {
            // تحديث الكمية والمواصفات
            var newQuantity = existingItem.Quantity + quantity;
            if (newQuantity > 1000)
                throw new DomainRuleViolationException("Total quantity cannot exceed 1000");

            existingItem.UpdateQuantity(newQuantity);
            existingItem.UpdateSelectedSpecs(selectedSpecs);
            
            AddDomainEvent(new CartItemUpdatedEvent(Id, UserId, productId, newQuantity));
        }
        else
        {
            // إضافة عنصر جديد
            var item = new CartItem(Id, productId, quantity, selectedSpecs);
            _items.Add(item);
            
            AddDomainEvent(new CartItemAddedEvent(Id, UserId, productId, quantity));
        }

        Touch();
    }

    /// <summary>
    /// تحديث كمية عنصر
    /// </summary>
    public void UpdateQuantity(Guid productId, int quantity)
    {
        if (IsLocked)
            throw new DomainRuleViolationException("Cannot modify locked cart");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new DomainRuleViolationException("Product not found in cart");

        item.UpdateQuantity(quantity);
        Touch();

        AddDomainEvent(new CartItemUpdatedEvent(Id, UserId, productId, quantity));
    }

    /// <summary>
    /// إزالة عنصر من السلة
    /// </summary>
    public void RemoveItem(Guid productId)
    {
        if (IsLocked)
            throw new DomainRuleViolationException("Cannot modify locked cart");

        var removed = _items.RemoveAll(item => item.ProductId == productId) > 0;
        if (removed)
        {
            Touch();
            AddDomainEvent(new CartItemRemovedEvent(Id, UserId, productId));
        }
    }

    /// <summary>
    /// مسح جميع العناصر
    /// </summary>
    public void Clear()
    {
        if (IsLocked)
            throw new DomainRuleViolationException("Cannot modify locked cart");

        if (_items.Count > 0)
        {
            _items.Clear();
            Touch();
        }
    }

    /// <summary>
    /// تحديث الملاحظات
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        if (IsLocked)
            throw new DomainRuleViolationException("Cannot modify locked cart");

        if (!string.IsNullOrWhiteSpace(notes) && notes.Length > 1000)
            throw new DomainRuleViolationException("Notes cannot exceed 1000 characters");

        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Touch();
    }

    /// <summary>
    /// قفل السلة (عند تحويلها لطلب تسعير)
    /// </summary>
    public void Lock()
    {
        if (IsLocked)
            return; // مسبقاً مقفلة

        if (_items.Count == 0)
            throw new DomainRuleViolationException("Cannot lock empty cart");

        IsLocked = true;
        Touch();

        AddDomainEvent(new CartLockedEvent(Id, UserId));
    }

    /// <summary>
    /// إلغاء قفل السلة
    /// </summary>
    public void Unlock()
    {
        if (!IsLocked)
            return; // غير مقفلة

        IsLocked = false;
        Touch();
    }

    /// <summary>
    /// التحقق من وجود عنصر في السلة
    /// </summary>
    public bool HasItem(Guid productId) => _items.Any(item => item.ProductId == productId);

    /// <summary>
    /// الحصول على عنصر من السلة
    /// </summary>
    public CartItem? GetItem(Guid productId) => _items.FirstOrDefault(item => item.ProductId == productId);

    /// <summary>
    /// التحقق من كون السلة فارغة
    /// </summary>
    public bool IsEmpty => _items.Count == 0;

    /// <summary>
    /// تحديث الطابع الزمني للمس الأخير
    /// </summary>
    private void Touch()
    {
        LastTouchedUtc = DateTime.UtcNow;
        MarkAsModified();
    }
}
