using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Orders;
using Domain.Events.Quotes;
using Domain.ValueObjects.Commerce;

namespace Domain.Entities.Quotes;

/// <summary>
/// كيان طلب التسعير - جذر التجميعة
/// </summary>
public sealed class QuoteRequest : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public CartSnapshot CartSnapshot { get; private set; } = default!;
    public QuoteRequestStatus Status { get; private set; } = QuoteRequestStatus.Pending;
    public DateTime RequestedAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }

    // للاستخدام مع EF Core
    private QuoteRequest() { }

    private QuoteRequest(Guid userId, CartSnapshot cartSnapshot)
    {
        UserId = userId;
        CartSnapshot = cartSnapshot;
        Status = QuoteRequestStatus.Pending;
        RequestedAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = DateTime.UtcNow.AddDays(7); // انتهاء الصلاحية بعد 7 أيام افتراضياً
    }

    /// <summary>
    /// إنشاء طلب تسعير جديد
    /// </summary>
    public static QuoteRequest Create(Guid userId, CartSnapshot cartSnapshot)
    {
        if (cartSnapshot.ItemsCount == 0)
            throw new DomainRuleViolationException("Cannot create quote request for empty cart");

        var quoteRequest = new QuoteRequest(userId, cartSnapshot);
        quoteRequest.AddDomainEvent(new QuoteRequestCreatedEvent(quoteRequest.Id, userId, cartSnapshot.ItemsCount));
        return quoteRequest;
    }

    /// <summary>
    /// تحديد حالة الطلب كمُسعَّر
    /// </summary>
    public void MarkPriced()
    {
        if (Status != QuoteRequestStatus.Pending)
            throw new DomainRuleViolationException("Only pending quote requests can be marked as priced");

        if (IsExpired(DateTime.UtcNow))
            throw new DomainRuleViolationException("Cannot price expired quote request");

        Status = QuoteRequestStatus.Priced;
        MarkAsModified();

        AddDomainEvent(new QuoteRequestPricedEvent(Id, UserId));
    }

    /// <summary>
    /// إنهاء صلاحية الطلب
    /// </summary>
    public void Expire()
    {
        if (Status == QuoteRequestStatus.Expired)
            return; // مسبقاً منتهي الصلاحية

        Status = QuoteRequestStatus.Expired;
        MarkAsModified();

        AddDomainEvent(new QuoteRequestExpiredEvent(Id, UserId));
    }

    /// <summary>
    /// التحقق من انتهاء صلاحية الطلب
    /// </summary>
    public bool IsExpired(DateTime now) => ExpiresAtUtc.HasValue && now >= ExpiresAtUtc.Value;

    /// <summary>
    /// تحديد موعد انتهاء الصلاحية
    /// </summary>
    public void SetExpiryDate(DateTime expiresAtUtc)
    {
        if (expiresAtUtc <= DateTime.UtcNow)
            throw new DomainRuleViolationException("Expiry date must be in the future");

        ExpiresAtUtc = expiresAtUtc;
        MarkAsModified();
    }
}
