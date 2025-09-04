using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Drivers;
using Domain.Events.Drivers;
using Domain.ValueObjects.Media;
using Domain.ValueObjects.Vehicles;

namespace Domain.Entities.Drivers;

/// <summary>
/// كيان طلب السائق - جذر التجميعة
/// </summary>
public sealed class DriverApplication : BaseEntity, IAggregateRoot
{
    private readonly List<ImageRef> _licenseImages = new();

    public Guid UserId { get; private set; }
    public VehicleInfo VehicleInfo { get; private set; } = default!;
    
    /// <summary>
    /// صور رخصة القيادة
    /// </summary>
    public IReadOnlyList<ImageRef> LicenseImages => _licenseImages.AsReadOnly();
    
    public DriverApplicationStatus Status { get; private set; } = DriverApplicationStatus.Pending;
    public DateTime? DecisionAtUtc { get; private set; }
    public string? DecisionReason { get; private set; }
    public string? DecisionBy { get; private set; }

    // للاستخدام مع EF Core
    private DriverApplication() { }

    private DriverApplication(Guid userId, VehicleInfo vehicleInfo, List<ImageRef> licenseImages)
    {
        UserId = userId;
        VehicleInfo = vehicleInfo;
        _licenseImages.AddRange(licenseImages);
        Status = DriverApplicationStatus.Pending;
    }

    /// <summary>
    /// إنشاء طلب سائق جديد
    /// </summary>
    public static DriverApplication Create(Guid userId, VehicleInfo vehicleInfo, List<ImageRef> licenseImages)
    {
        if (licenseImages == null || licenseImages.Count == 0)
            throw new DomainRuleViolationException("At least one license image is required");

        if (licenseImages.Count > 5)
            throw new DomainRuleViolationException("Cannot upload more than 5 license images");

        var application = new DriverApplication(userId, vehicleInfo, licenseImages);
        application.AddDomainEvent(new DriverApplicationSubmittedEvent(
            application.Id, 
            userId, 
            vehicleInfo.ToString()));
        
        return application;
    }

    /// <summary>
    /// الموافقة على الطلب
    /// </summary>
    public void Approve(string approvedBy)
    {
        if (Status != DriverApplicationStatus.Pending)
            throw new DomainRuleViolationException("Only pending applications can be approved");

        if (string.IsNullOrWhiteSpace(approvedBy))
            throw new DomainRuleViolationException("Approved by is required");

        Status = DriverApplicationStatus.Approved;
        DecisionAtUtc = DateTime.UtcNow;
        DecisionBy = approvedBy.Trim();
        DecisionReason = null;
        MarkAsModified();

        AddDomainEvent(new DriverApplicationApprovedEvent(Id, UserId));
    }

    /// <summary>
    /// رفض الطلب
    /// </summary>
    public void Reject(string rejectedBy, string reason)
    {
        if (Status != DriverApplicationStatus.Pending)
            throw new DomainRuleViolationException("Only pending applications can be rejected");

        if (string.IsNullOrWhiteSpace(rejectedBy))
            throw new DomainRuleViolationException("Rejected by is required");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainRuleViolationException("Rejection reason is required");

        if (reason.Length > 500)
            throw new DomainRuleViolationException("Rejection reason cannot exceed 500 characters");

        Status = DriverApplicationStatus.Rejected;
        DecisionAtUtc = DateTime.UtcNow;
        DecisionBy = rejectedBy.Trim();
        DecisionReason = reason.Trim();
        MarkAsModified();

        AddDomainEvent(new DriverApplicationRejectedEvent(Id, UserId, reason));
    }

    /// <summary>
    /// تحديث معلومات المركبة
    /// </summary>
    public void UpdateVehicleInfo(VehicleInfo vehicleInfo)
    {
        if (Status != DriverApplicationStatus.Pending)
            throw new DomainRuleViolationException("Cannot update approved or rejected application");

        VehicleInfo = vehicleInfo;
        MarkAsModified();
    }

    /// <summary>
    /// إضافة صورة رخصة
    /// </summary>
    public void AddLicenseImage(ImageRef licenseImage)
    {
        if (Status != DriverApplicationStatus.Pending)
            throw new DomainRuleViolationException("Cannot update approved or rejected application");

        if (_licenseImages.Count >= 5)
            throw new DomainRuleViolationException("Cannot add more than 5 license images");

        if (_licenseImages.Any(img => img.Url == licenseImage.Url))
            return; // الصورة موجودة بالفعل

        _licenseImages.Add(licenseImage);
        MarkAsModified();
    }

    /// <summary>
    /// إزالة صورة رخصة
    /// </summary>
    public void RemoveLicenseImage(string imageUrl)
    {
        if (Status != DriverApplicationStatus.Pending)
            throw new DomainRuleViolationException("Cannot update approved or rejected application");

        var removed = _licenseImages.RemoveAll(img => img.Url == imageUrl) > 0;
        
        if (removed)
        {
            if (_licenseImages.Count == 0)
                throw new DomainRuleViolationException("At least one license image is required");

            MarkAsModified();
        }
    }

    /// <summary>
    /// التحقق من إمكانية التعديل
    /// </summary>
    public bool CanBeModified() => Status == DriverApplicationStatus.Pending;
}
