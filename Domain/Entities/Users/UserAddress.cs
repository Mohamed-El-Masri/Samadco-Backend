using Domain.Abstractions;
using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Users;
using Domain.ValueObjects.Location;

namespace Domain.Entities.Users;

/// <summary>
/// عنوان المستخدم
/// </summary>
public sealed class UserAddress : BaseEntity, ISoftDelete
{
    public Guid UserId { get; private set; }
    public string Label { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; } = true;
    public AddressType Type { get; private set; }
    public string? DeliveryInstructions { get; private set; }
    public string? ContactPersonName { get; private set; }
    public string? ContactPersonPhone { get; private set; }

    // ISoftDelete implementation
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    // Navigation Properties
    public User User { get; private set; } = default!;

    // For EF Core
    private UserAddress() { }

    private UserAddress(Guid userId, string label, Address address, AddressType type, bool isDefault = false)
    {
        UserId = userId;
        Label = label;
        Address = address;
        Type = type;
        IsDefault = isDefault;
        IsActive = true;
    }

    /// <summary>
    /// إنشاء عنوان جديد
    /// </summary>
    public static UserAddress Create(Guid userId, string label, Address address, AddressType type, bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainRuleViolationException("Address label is required");

        if (label.Length > 100)
            throw new DomainRuleViolationException("Address label cannot exceed 100 characters");

        return new UserAddress(userId, label.Trim(), address, type, isDefault);
    }

    /// <summary>
    /// تحديث تسمية العنوان
    /// </summary>
    public void UpdateLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainRuleViolationException("Address label is required");

        if (label.Length > 100)
            throw new DomainRuleViolationException("Address label cannot exceed 100 characters");

        Label = label.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// تحديث العنوان
    /// </summary>
    public void UpdateAddress(Address address)
    {
        Address = address ?? throw new DomainRuleViolationException("Address is required");
        MarkAsModified();
    }

    /// <summary>
    /// تحديد كعنوان افتراضي
    /// </summary>
    public void SetAsDefault()
    {
        IsDefault = true;
        MarkAsModified();
    }

    /// <summary>
    /// إلغاء تحديد كعنوان افتراضي
    /// </summary>
    public void UnsetAsDefault()
    {
        IsDefault = false;
        MarkAsModified();
    }

    /// <summary>
    /// تفعيل العنوان
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        MarkAsModified();
    }

    /// <summary>
    /// إلغاء تفعيل العنوان
    /// </summary>
    public void Deactivate()
    {
        if (IsDefault)
            throw new DomainRuleViolationException("Cannot deactivate default address");

        IsActive = false;
        MarkAsModified();
    }

    /// <summary>
    /// تحديث تعليمات التوصيل
    /// </summary>
    public void UpdateDeliveryInstructions(string? instructions)
    {
        DeliveryInstructions = instructions?.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// تحديث بيانات الاتصال
    /// </summary>
    public void UpdateContactPerson(string? name, string? phone)
    {
        ContactPersonName = name?.Trim();
        ContactPersonPhone = phone?.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// تحديث نوع العنوان
    /// </summary>
    public void UpdateType(AddressType type)
    {
        Type = type;
        MarkAsModified();
    }

    /// <summary>
    /// حذف ناعم للعنوان
    /// </summary>
    public void Delete()
    {
        if (IsDeleted)
            return;

        if (IsDefault)
            throw new DomainRuleViolationException("Cannot delete default address");

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// استعادة العنوان المحذوف
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
