using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.ValueObjects.Identity;
using Domain.ValueObjects.Location;

namespace Domain.Entities.Distributors;

/// <summary>
/// كيان الموزع - جذر التجميعة
/// </summary>
public sealed class Distributor : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? NameAr { get; private set; }
    public string Region { get; private set; } = default!;
    public string? RegionAr { get; private set; }
    public Email ContactEmail { get; private set; } = default!;
    public PhoneNumber ContactPhone { get; private set; } = default!;
    public Address? Address { get; private set; }
    public bool IsActive { get; private set; } = true;

    // للاستخدام مع EF Core
    private Distributor() { }

    private Distributor(
        string name,
        string? nameAr,
        string region,
        string? regionAr,
        Email contactEmail,
        PhoneNumber contactPhone,
        Address? address)
    {
        Name = name;
        NameAr = nameAr;
        Region = region;
        RegionAr = regionAr;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        Address = address;
        IsActive = true;
    }

    /// <summary>
    /// إنشاء موزع جديد
    /// </summary>
    public static Distributor Create(
        string name,
        string? nameAr,
        string region,
        string? regionAr,
        Email contactEmail,
        PhoneNumber contactPhone,
        Address? address = null)
    {
        ValidateTexts(name, nameAr, region, regionAr);

        return new Distributor(
            name.Trim(),
            string.IsNullOrWhiteSpace(nameAr) ? null : nameAr.Trim(),
            region.Trim(),
            string.IsNullOrWhiteSpace(regionAr) ? null : regionAr.Trim(),
            contactEmail,
            contactPhone,
            address);
    }

    /// <summary>
    /// تحديث معلومات الموزع
    /// </summary>
    public void UpdateInfo(
        string name,
        string? nameAr,
        string region,
        string? regionAr,
        Email contactEmail,
        PhoneNumber contactPhone,
        Address? address)
    {
        ValidateTexts(name, nameAr, region, regionAr);

        Name = name.Trim();
        NameAr = string.IsNullOrWhiteSpace(nameAr) ? null : nameAr.Trim();
        Region = region.Trim();
        RegionAr = string.IsNullOrWhiteSpace(regionAr) ? null : regionAr.Trim();
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        Address = address;

        MarkAsModified();
    }

    /// <summary>
    /// تفعيل الموزع
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return; // مسبقاً نشط

        IsActive = true;
        MarkAsModified();
    }

    /// <summary>
    /// إلغاء تفعيل الموزع
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return; // مسبقاً غير نشط

        IsActive = false;
        MarkAsModified();
    }

    /// <summary>
    /// التحقق من صحة النصوص
    /// </summary>
    private static void ValidateTexts(string name, string? nameAr, string region, string? regionAr)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleViolationException("Distributor name is required");

        if (name.Length > 100)
            throw new DomainRuleViolationException("Distributor name cannot exceed 100 characters");

        if (!string.IsNullOrWhiteSpace(nameAr) && nameAr.Length > 100)
            throw new DomainRuleViolationException("Distributor name in Arabic cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(region))
            throw new DomainRuleViolationException("Distributor region is required");

        if (region.Length > 100)
            throw new DomainRuleViolationException("Distributor region cannot exceed 100 characters");

        if (!string.IsNullOrWhiteSpace(regionAr) && regionAr.Length > 100)
            throw new DomainRuleViolationException("Distributor region in Arabic cannot exceed 100 characters");
    }
}
