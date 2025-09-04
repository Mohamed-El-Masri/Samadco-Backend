using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Users;
using Domain.Events.Users;
using Domain.ValueObjects.Identity;
using Domain.ValueObjects.Location;

namespace Domain.Entities.Users;

/// <summary>
/// ملف البائع المرتبط بالمستخدم
/// </summary>
public sealed class SellerProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public string CompanyName { get; private set; } = default!;
    public string? CompanyNameAr { get; private set; }
    public TaxId TaxId { get; private set; } = default!;
    public Address? Address { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; } = VerificationStatus.Pending;
    public DateTime? VerifiedAtUtc { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? VerifiedBy { get; private set; }

    // للاستخدام مع EF Core
    private SellerProfile() { }

    internal SellerProfile(Guid userId, string companyName, string? companyNameAr, TaxId taxId, Address? address)
    {
        UserId = userId;
        CompanyName = companyName;
        CompanyNameAr = companyNameAr;
        TaxId = taxId;
        Address = address;
        VerificationStatus = VerificationStatus.Pending;
    }

    /// <summary>
    /// إنشاء ملف بائع جديد
    /// </summary>
    public static SellerProfile Create(
        Guid userId,
        string companyName,
        string? companyNameAr,
        TaxId taxId,
        Address? address = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainRuleViolationException("Company name is required");

        if (companyName.Length > 100)
            throw new DomainRuleViolationException("Company name cannot exceed 100 characters");

        if (!string.IsNullOrWhiteSpace(companyNameAr) && companyNameAr.Length > 100)
            throw new DomainRuleViolationException("Company name in Arabic cannot exceed 100 characters");

        var profile = new SellerProfile(userId, companyName.Trim(), 
            string.IsNullOrWhiteSpace(companyNameAr) ? null : companyNameAr.Trim(), 
            taxId, address);

        profile.AddDomainEvent(new SellerProfileCreatedEvent(userId, companyName));
        return profile;
    }

    /// <summary>
    /// التحقق من ملف البائع
    /// </summary>
    public void MarkVerified(string verifiedBy)
    {
        if (VerificationStatus == VerificationStatus.Verified)
            throw new DomainRuleViolationException("Seller profile is already verified");

        if (string.IsNullOrWhiteSpace(verifiedBy))
            throw new DomainRuleViolationException("Verified by is required");

        VerificationStatus = VerificationStatus.Verified;
        VerifiedAtUtc = DateTime.UtcNow;
        VerifiedBy = verifiedBy;
        RejectionReason = null;
        MarkAsModified();

        AddDomainEvent(new SellerProfileVerifiedEvent(UserId, CompanyName, TaxId.Value));
    }

    /// <summary>
    /// رفض ملف البائع
    /// </summary>
    public void Reject(string reason)
    {
        if (VerificationStatus == VerificationStatus.Verified)
            throw new DomainRuleViolationException("Cannot reject a verified seller profile");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainRuleViolationException("Rejection reason is required");

        if (reason.Length > 500)
            throw new DomainRuleViolationException("Rejection reason cannot exceed 500 characters");

        VerificationStatus = VerificationStatus.Rejected;
        RejectionReason = reason.Trim();
        VerifiedAtUtc = null;
        VerifiedBy = null;
        MarkAsModified();

        AddDomainEvent(new SellerProfileRejectedEvent(UserId, reason));
    }

    /// <summary>
    /// تحديث معلومات الشركة
    /// </summary>
    public void UpdateCompanyInfo(string companyName, string? companyNameAr, TaxId taxId, Address? address)
    {
        if (VerificationStatus == VerificationStatus.Verified)
            throw new DomainRuleViolationException("Cannot update verified seller profile");

        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainRuleViolationException("Company name is required");

        if (companyName.Length > 100)
            throw new DomainRuleViolationException("Company name cannot exceed 100 characters");

        if (!string.IsNullOrWhiteSpace(companyNameAr) && companyNameAr.Length > 100)
            throw new DomainRuleViolationException("Company name in Arabic cannot exceed 100 characters");

        CompanyName = companyName.Trim();
        CompanyNameAr = string.IsNullOrWhiteSpace(companyNameAr) ? null : companyNameAr.Trim();
        TaxId = taxId;
        Address = address;
        
        // إعادة تعيين الحالة إلى Pending إذا كانت مرفوضة
        if (VerificationStatus == VerificationStatus.Rejected)
        {
            VerificationStatus = VerificationStatus.Pending;
            RejectionReason = null;
        }

        MarkAsModified();
    }
}
