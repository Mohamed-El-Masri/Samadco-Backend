using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Orders;
using Domain.Events.Offers;

namespace Domain.Entities.Offers;

/// <summary>
/// كيان العرض - جذر التجميعة
/// </summary>
public sealed class Offer : BaseEntity, IAggregateRoot
{
    private readonly List<OfferItem> _items = new();

    // حقول متعددة اللغات
    public string Title { get; private set; } = default!;
    public string? TitleAr { get; private set; }
    public string? Description { get; private set; }
    public string? DescriptionAr { get; private set; }
    
    public DateTime ActiveFromUtc { get; private set; }
    public DateTime ActiveToUtc { get; private set; }
    public OfferStatus Status { get; private set; } = OfferStatus.Draft;
    
    /// <summary>
    /// عناصر العرض
    /// </summary>
    public IReadOnlyList<OfferItem> Items => _items.AsReadOnly();

    // للاستخدام مع EF Core
    private Offer() { }

    private Offer(
        string title,
        string? titleAr,
        string? description,
        string? descriptionAr,
        DateTime activeFromUtc,
        DateTime activeToUtc)
    {
        Title = title;
        TitleAr = titleAr;
        Description = description;
        DescriptionAr = descriptionAr;
        ActiveFromUtc = activeFromUtc;
        ActiveToUtc = activeToUtc;
        Status = OfferStatus.Draft;
    }

    /// <summary>
    /// إنشاء عرض جديد
    /// </summary>
    public static Offer Create(
        string title,
        string? titleAr,
        string? description,
        string? descriptionAr,
        DateTime activeFromUtc,
        DateTime activeToUtc)
    {
        ValidateTexts(title, titleAr, description, descriptionAr);
        ValidateDates(activeFromUtc, activeToUtc);

        var offer = new Offer(
            title.Trim(),
            string.IsNullOrWhiteSpace(titleAr) ? null : titleAr.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim(),
            activeFromUtc,
            activeToUtc);

        offer.AddDomainEvent(new OfferCreatedEvent(offer.Id, title));
        return offer;
    }

    /// <summary>
    /// تفعيل العرض
    /// </summary>
    public void Activate(DateTime now)
    {
        if (Status != OfferStatus.Draft)
            throw new DomainRuleViolationException("Only draft offers can be activated");

        if (_items.Count == 0)
            throw new DomainRuleViolationException("Cannot activate offer without items");

        if (now >= ActiveToUtc)
            throw new DomainRuleViolationException("Cannot activate expired offer");

        Status = OfferStatus.Active;
        MarkAsModified();

        AddDomainEvent(new OfferActivatedEvent(Id, ActiveFromUtc, ActiveToUtc));
    }

    /// <summary>
    /// إنهاء صلاحية العرض
    /// </summary>
    public void Expire(DateTime now)
    {
        if (Status == OfferStatus.Expired || Status == OfferStatus.Archived)
            return; // مسبقاً منتهي الصلاحية أو مؤرشف

        Status = OfferStatus.Expired;
        MarkAsModified();

        AddDomainEvent(new OfferExpiredEvent(Id));
    }

    /// <summary>
    /// أرشفة العرض
    /// </summary>
    public void Archive()
    {
        if (Status == OfferStatus.Archived)
            return; // مسبقاً مؤرشف

        Status = OfferStatus.Archived;
        MarkAsModified();

        AddDomainEvent(new OfferArchivedEvent(Id));
    }

    /// <summary>
    /// إضافة عنصر للعرض
    /// </summary>
    public void AddItem(Guid productId, int quantity, string? productSnapshotName = null)
    {
        if (Status != OfferStatus.Draft)
            throw new DomainRuleViolationException("Cannot modify non-draft offer");

        if (quantity <= 0)
            throw new DomainRuleViolationException("Quantity must be positive");

        if (quantity > 10000)
            throw new DomainRuleViolationException("Quantity cannot exceed 10000");

        if (_items.Count >= 50) // حد أقصى 50 عنصر
            throw new DomainRuleViolationException("Cannot add more than 50 items to offer");

        // التحقق من عدم وجود المنتج مسبقاً
        if (_items.Any(item => item.ProductId == productId))
            throw new DomainRuleViolationException("Product already exists in offer");

        var item = new OfferItem(Id, productId, quantity, productSnapshotName);
        _items.Add(item);
        MarkAsModified();
    }

    /// <summary>
    /// إزالة عنصر من العرض
    /// </summary>
    public void RemoveItem(Guid productId)
    {
        if (Status != OfferStatus.Draft)
            throw new DomainRuleViolationException("Cannot modify non-draft offer");

        var removed = _items.RemoveAll(item => item.ProductId == productId) > 0;
        if (removed)
        {
            MarkAsModified();
        }
    }

    /// <summary>
    /// تحديث كمية عنصر
    /// </summary>
    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        if (Status != OfferStatus.Draft)
            throw new DomainRuleViolationException("Cannot modify non-draft offer");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new DomainRuleViolationException("Product not found in offer");

        item.UpdateQuantity(quantity);
        MarkAsModified();
    }

    /// <summary>
    /// تحديث معلومات العرض
    /// </summary>
    public void UpdateInfo(
        string title,
        string? titleAr,
        string? description,
        string? descriptionAr,
        DateTime activeFromUtc,
        DateTime activeToUtc)
    {
        if (Status != OfferStatus.Draft)
            throw new DomainRuleViolationException("Cannot modify non-draft offer");

        ValidateTexts(title, titleAr, description, descriptionAr);
        ValidateDates(activeFromUtc, activeToUtc);

        Title = title.Trim();
        TitleAr = string.IsNullOrWhiteSpace(titleAr) ? null : titleAr.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim();
        ActiveFromUtc = activeFromUtc;
        ActiveToUtc = activeToUtc;
        
        MarkAsModified();
    }

    /// <summary>
    /// التحقق من انتهاء صلاحية العرض
    /// </summary>
    public bool IsExpired(DateTime now) => now >= ActiveToUtc;

    /// <summary>
    /// التحقق من كون العرض نشطاً
    /// </summary>
    public bool IsActiveNow(DateTime now) => 
        Status == OfferStatus.Active && 
        now >= ActiveFromUtc && 
        now < ActiveToUtc;

    /// <summary>
    /// التحقق من صحة النصوص
    /// </summary>
    private static void ValidateTexts(string title, string? titleAr, string? description, string? descriptionAr)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainRuleViolationException("Offer title is required");

        if (title.Length > 200)
            throw new DomainRuleViolationException("Offer title cannot exceed 200 characters");

        if (!string.IsNullOrWhiteSpace(titleAr) && titleAr.Length > 200)
            throw new DomainRuleViolationException("Offer title in Arabic cannot exceed 200 characters");

        if (!string.IsNullOrWhiteSpace(description) && description.Length > 1000)
            throw new DomainRuleViolationException("Offer description cannot exceed 1000 characters");

        if (!string.IsNullOrWhiteSpace(descriptionAr) && descriptionAr.Length > 1000)
            throw new DomainRuleViolationException("Offer description in Arabic cannot exceed 1000 characters");
    }

    /// <summary>
    /// التحقق من صحة التواريخ
    /// </summary>
    private static void ValidateDates(DateTime activeFromUtc, DateTime activeToUtc)
    {
        if (activeFromUtc >= activeToUtc)
            throw new DomainRuleViolationException("Active from date must be before active to date");

        if (activeToUtc <= DateTime.UtcNow)
            throw new DomainRuleViolationException("Active to date must be in the future");

        var duration = activeToUtc - activeFromUtc;
        if (duration.TotalDays > 365) // حد أقصى سنة واحدة
            throw new DomainRuleViolationException("Offer duration cannot exceed 365 days");
    }
}
