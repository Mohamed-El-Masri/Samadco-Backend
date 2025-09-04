using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Events.Quotes;

namespace Domain.Entities.Quotes;

/// <summary>
/// كيان عرض السعر - جذر التجميعة
/// </summary>
public sealed class Quote : BaseEntity, IAggregateRoot
{
    private readonly List<QuoteLine> _lines = new();

    public Guid QuoteRequestId { get; private set; }
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// أسطر عرض السعر
    /// </summary>
    public IReadOnlyList<QuoteLine> Lines => _lines.AsReadOnly();
    
    public decimal TotalBeforeTax { get; private set; }
    public decimal Tax { get; private set; }
    public decimal Shipping { get; private set; }
    public decimal Total { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public string? Notes { get; private set; }

    /// <summary>
    /// التحقق من انتهاء الصلاحية
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    // للاستخدام مع EF Core
    private Quote() { }

    {
        QuoteRequestId = quoteRequestId;
        UserId = userId;
        ExpiresAtUtc = expiresAtUtc;
        Notes = notes;
        TotalBeforeTax = 0;
        Tax = 0;
        Shipping = 0;
        Total = 0;
    }

    /// <summary>
    /// إنشاء عرض سعر جديد
    /// </summary>
    {
        var expiry = expiresAtUtc ?? DateTime.UtcNow.AddDays(14); // صالح لمدة 14 يوم افتراضياً
        
        if (expiry <= DateTime.UtcNow)
            throw new DomainRuleViolationException("Quote expiry date must be in the future");

        if (!string.IsNullOrWhiteSpace(notes) && notes.Length > 1000)
            throw new DomainRuleViolationException("Quote notes cannot exceed 1000 characters");

            string.IsNullOrWhiteSpace(notes) ? null : notes.Trim());

        return quote;
    }

    /// <summary>
    /// إضافة سطر لعرض السعر
    /// </summary>
    public void AddLine(Guid productId, int quantity, decimal unitPrice, string? productSnapshot = null)
    {
        if (IsExpired)
            throw new DomainRuleViolationException("Cannot modify expired quote");

        if (quantity <= 0)
            throw new DomainRuleViolationException("Quantity must be positive");

        if (unitPrice < 0)
            throw new DomainRuleViolationException("Unit price cannot be negative");

        // التحقق من عدم وجود المنتج مسبقاً
        if (_lines.Any(line => line.ProductId == productId))
            throw new DomainRuleViolationException("Product already exists in quote");

        var line = new QuoteLine(Id, productId, quantity, unitPrice, productSnapshot);
        _lines.Add(line);
        
        RecalculateTotals();
    }

    /// <summary>
    /// إزالة سطر من عرض السعر
    /// </summary>
    public void RemoveLine(Guid productId)
    {
        if (IsExpired)
            throw new DomainRuleViolationException("Cannot modify expired quote");

        var removed = _lines.RemoveAll(line => line.ProductId == productId) > 0;
        if (removed)
        {
            RecalculateTotals();
        }
    }

    /// <summary>
    /// تحديث سعر منتج
    /// </summary>
    public void UpdateLinePrice(Guid productId, decimal unitPrice)
    {
        if (IsExpired)
            throw new DomainRuleViolationException("Cannot modify expired quote");

        var line = _lines.FirstOrDefault(l => l.ProductId == productId);
        if (line == null)
            throw new DomainRuleViolationException("Product not found in quote");

        line.UpdatePrice(unitPrice);
        RecalculateTotals();
    }

    /// <summary>
    /// تحديث كمية منتج
    /// </summary>
    public void UpdateLineQuantity(Guid productId, int quantity)
    {
        if (IsExpired)
            throw new DomainRuleViolationException("Cannot modify expired quote");

        var line = _lines.FirstOrDefault(l => l.ProductId == productId);
        if (line == null)
            throw new DomainRuleViolationException("Product not found in quote");

        line.UpdateQuantity(quantity);
        RecalculateTotals();
    }

    /// <summary>
    /// تحديث الضريبة والشحن
    /// </summary>
    public void UpdateTaxAndShipping(decimal tax, decimal shipping)
    {
        if (IsExpired)
            throw new DomainRuleViolationException("Cannot modify expired quote");

        if (tax < 0)
            throw new DomainRuleViolationException("Tax cannot be negative");

        if (shipping < 0)
            throw new DomainRuleViolationException("Shipping cannot be negative");

        Tax = tax;
        Shipping = shipping;
        RecalculateTotals();
    }

    /// <summary>
    /// إنهاء صلاحية عرض السعر
    /// </summary>
    public void Expire()
    {
        if (IsExpired)
            return; // مسبقاً منتهي الصلاحية

        ExpiresAtUtc = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new QuoteExpiredEvent(Id, UserId));
    }

    /// <summary>
    /// إصدار عرض السعر (إضافة الحدث)
    /// </summary>
    public void Issue()
    {
        if (_lines.Count == 0)
            throw new DomainRuleViolationException("Cannot issue quote without lines");

        if (IsExpired)
            throw new DomainRuleViolationException("Cannot issue expired quote");

        AddDomainEvent(new QuoteIssuedEvent(Id, QuoteRequestId, UserId, Total));
    }

    /// <summary>
    /// تحديث ملاحظات عرض السعر
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        if (IsExpired)
            throw new DomainRuleViolationException("Cannot modify expired quote");

        if (!string.IsNullOrWhiteSpace(notes) && notes.Length > 1000)
            throw new DomainRuleViolationException("Quote notes cannot exceed 1000 characters");

        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// إعادة حساب الإجماليات
    /// </summary>
    private void RecalculateTotals()
    {
        TotalBeforeTax = _lines.Sum(line => line.Subtotal);
        Total = TotalBeforeTax + Tax + Shipping;
        MarkAsModified();
    }
}
