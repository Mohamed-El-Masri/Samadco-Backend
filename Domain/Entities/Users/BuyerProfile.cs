using Domain.Abstractions;
using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Users;
using Domain.Events.Users;

namespace Domain.Entities.Users;

/// <summary>
/// ملف المشتري
/// </summary>
public sealed class BuyerProfile : BaseEntity, ISoftDelete
{
    public Guid UserId { get; private set; }
    public string? PreferredLanguage { get; private set; }
    public string? PreferredCurrency { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public decimal CurrentCredit { get; private set; }
    public BuyerStatus Status { get; private set; } = BuyerStatus.Active;
    public DateTime? LastOrderDate { get; private set; }
    public int TotalOrdersCount { get; private set; }
    public decimal TotalOrdersValue { get; private set; }
    public string? Notes { get; private set; }
    
    // ISoftDelete implementation
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    // Navigation Properties
    public User User { get; private set; } = default!;

    // For EF Core
    private BuyerProfile() { }

    private BuyerProfile(Guid userId)
    {
        UserId = userId;
        Status = BuyerStatus.Active;
        CurrentCredit = 0;
        TotalOrdersCount = 0;
        TotalOrdersValue = 0;
    }

    /// <summary>
    /// إنشاء ملف مشتري جديد
    /// </summary>
    public static BuyerProfile Create(Guid userId)
    {
        var profile = new BuyerProfile(userId);
        profile.AddDomainEvent(new BuyerProfileCreatedEvent(profile.Id, userId));
        return profile;
    }

    /// <summary>
    /// تحديث تفضيلات المشتري
    /// </summary>
    public void UpdatePreferences(string? language, string? currency)
    {
        PreferredLanguage = language?.Trim();
        PreferredCurrency = currency?.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// تحديد حد الائتمان
    /// </summary>
    public void SetCreditLimit(decimal creditLimit)
    {
        if (creditLimit < 0)
            throw new DomainRuleViolationException("Credit limit cannot be negative");

        CreditLimit = creditLimit;
        MarkAsModified();
    }

    /// <summary>
    /// إضافة رصيد
    /// </summary>
    public void AddCredit(decimal amount, string reason)
    {
        if (amount <= 0)
            throw new DomainRuleViolationException("Credit amount must be positive");

        CurrentCredit += amount;
        MarkAsModified();
        AddDomainEvent(new BuyerCreditAddedEvent(Id, UserId, amount, reason));
    }

    /// <summary>
    /// خصم رصيد
    /// </summary>
    public void DeductCredit(decimal amount, string reason)
    {
        if (amount <= 0)
            throw new DomainRuleViolationException("Deduct amount must be positive");

        if (CurrentCredit < amount)
            throw new DomainRuleViolationException("Insufficient credit balance");

        CurrentCredit -= amount;
        MarkAsModified();
        AddDomainEvent(new BuyerCreditDeductedEvent(Id, UserId, amount, reason));
    }

    /// <summary>
    /// تحديث إحصائيات الطلبات
    /// </summary>
    public void UpdateOrderStatistics(decimal orderValue)
    {
        TotalOrdersCount++;
        TotalOrdersValue += orderValue;
        LastOrderDate = DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// تحديث حالة المشتري
    /// </summary>
    public void UpdateStatus(BuyerStatus status, string? reason = null)
    {
        if (Status == status)
            return;

        var oldStatus = Status;
        Status = status;
        Notes = reason;
        MarkAsModified();
        
        AddDomainEvent(new BuyerStatusChangedEvent(Id, UserId, oldStatus, status, reason));
    }

    /// <summary>
    /// التحقق من إمكانية الطلب
    /// </summary>
    public bool CanPlaceOrder(decimal orderValue)
    {
        if (Status != BuyerStatus.Active)
            return false;

        if (CreditLimit.HasValue && orderValue > CreditLimit.Value)
            return false;

        return true;
    }

    /// <summary>
    /// إضافة ملاحظة
    /// </summary>
    public void AddNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
            return;

        Notes = string.IsNullOrEmpty(Notes) 
            ? note.Trim() 
            : $"{Notes}\n{DateTime.UtcNow:yyyy-MM-dd}: {note.Trim()}";
        
        MarkAsModified();
    }

    /// <summary>
    /// حذف ناعم للملف
    /// </summary>
    public void Delete()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// استعادة الملف المحذوف
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAtUtc = null;
        MarkAsModified();
    }
}
