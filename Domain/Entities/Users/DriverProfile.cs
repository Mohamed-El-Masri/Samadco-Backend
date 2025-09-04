using Domain.Abstractions;
using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Users;
using Domain.Events.Users;

namespace Domain.Entities.Users;

/// <summary>
/// ملف السائق
/// </summary>
public sealed class DriverProfile : BaseEntity, ISoftDelete
{
    public Guid UserId { get; private set; }
    public string LicenseNumber { get; private set; } = default!;
    public DateTime LicenseExpiryDate { get; private set; }
    public DriverStatus Status { get; private set; } = DriverStatus.PendingVerification;
    public DateTime? VerifiedAt { get; private set; }
    public string? VerifiedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    
    // Statistics
    public int TotalDeliveries { get; private set; }
    public decimal TotalEarnings { get; private set; }
    public decimal AverageRating { get; private set; }
    public int TotalRatings { get; private set; }
    public DateTime? LastDeliveryDate { get; private set; }
    
    // Availability
    public bool IsAvailable { get; private set; } = true;
    public DateTime? AvailabilityUpdatedAt { get; private set; }
    
    // Vehicle Information
    public string? VehicleType { get; private set; }
    public string? VehicleModel { get; private set; }
    public string? VehiclePlateNumber { get; private set; }
    public string? VehicleColor { get; private set; }
    public decimal? VehicleCapacityKg { get; private set; }
    
    // ISoftDelete implementation
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    
    // Navigation Properties
    public User User { get; private set; } = default!;

    // For EF Core
    private DriverProfile() { }

    private DriverProfile(Guid userId, string licenseNumber, DateTime licenseExpiryDate)
    {
        UserId = userId;
        LicenseNumber = licenseNumber;
        LicenseExpiryDate = licenseExpiryDate;
        Status = DriverStatus.PendingVerification;
        IsAvailable = true;
        TotalDeliveries = 0;
        TotalEarnings = 0;
        AverageRating = 0;
        TotalRatings = 0;
    }

    /// <summary>
    /// إنشاء ملف سائق جديد
    /// </summary>
    public static DriverProfile Create(Guid userId, string licenseNumber, DateTime licenseExpiryDate)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new DomainRuleViolationException("License number is required");

        if (licenseExpiryDate <= DateTime.UtcNow)
            throw new DomainRuleViolationException("License expiry date must be in the future");

        var profile = new DriverProfile(userId, licenseNumber.Trim(), licenseExpiryDate);
        profile.AddDomainEvent(new DriverProfileCreatedEvent(profile.Id, userId, licenseNumber));
        return profile;
    }

    /// <summary>
    /// التحقق من السائق
    /// </summary>
    public void MarkVerified(string verifiedBy)
    {
        if (Status == DriverStatus.Verified)
            throw new DomainRuleViolationException("Driver is already verified");

        if (string.IsNullOrWhiteSpace(verifiedBy))
            throw new DomainRuleViolationException("Verified by is required");

        Status = DriverStatus.Verified;
        VerifiedAt = DateTime.UtcNow;
        VerifiedBy = verifiedBy.Trim();
        RejectionReason = null;
        MarkAsModified();
        
        AddDomainEvent(new DriverVerifiedEvent(Id, UserId, verifiedBy));
    }

    /// <summary>
    /// رفض السائق
    /// </summary>
    public void MarkRejected(string reason, string rejectedBy)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainRuleViolationException("Rejection reason is required");

        if (string.IsNullOrWhiteSpace(rejectedBy))
            throw new DomainRuleViolationException("Rejected by is required");

        Status = DriverStatus.Rejected;
        RejectionReason = reason.Trim();
        VerifiedBy = rejectedBy.Trim();
        VerifiedAt = DateTime.UtcNow;
        MarkAsModified();
        
        AddDomainEvent(new DriverRejectedEvent(Id, UserId, reason, rejectedBy));
    }

    /// <summary>
    /// تعليق السائق
    /// </summary>
    public void Suspend(string reason, string suspendedBy)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainRuleViolationException("Suspension reason is required");

        Status = DriverStatus.Suspended;
        RejectionReason = reason.Trim();
        IsAvailable = false;
        AvailabilityUpdatedAt = DateTime.UtcNow;
        MarkAsModified();
        
        AddDomainEvent(new DriverSuspendedEvent(Id, UserId, reason, suspendedBy));
    }

    /// <summary>
    /// تحديث معلومات المركبة
    /// </summary>
    public void UpdateVehicleInfo(string vehicleType, string vehicleModel, string plateNumber, 
        string color, decimal capacityKg)
    {
        if (string.IsNullOrWhiteSpace(vehicleType))
            throw new DomainRuleViolationException("Vehicle type is required");

        if (capacityKg <= 0)
            throw new DomainRuleViolationException("Vehicle capacity must be positive");

        VehicleType = vehicleType.Trim();
        VehicleModel = vehicleModel?.Trim();
        VehiclePlateNumber = plateNumber?.Trim();
        VehicleColor = color?.Trim();
        VehicleCapacityKg = capacityKg;
        MarkAsModified();
    }

    /// <summary>
    /// تحديث حالة التوفر
    /// </summary>
    public void UpdateAvailability(bool isAvailable)
    {
        if (Status != DriverStatus.Verified)
            throw new DomainRuleViolationException("Only verified drivers can update availability");

        IsAvailable = isAvailable;
        AvailabilityUpdatedAt = DateTime.UtcNow;
        MarkAsModified();
        
        AddDomainEvent(new DriverAvailabilityChangedEvent(Id, UserId, isAvailable));
    }

    /// <summary>
    /// إضافة تقييم
    /// </summary>
    public void AddRating(decimal rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainRuleViolationException("Rating must be between 1 and 5");

        var totalRatingPoints = (AverageRating * TotalRatings) + rating;
        TotalRatings++;
        AverageRating = totalRatingPoints / TotalRatings;
        MarkAsModified();
    }

    /// <summary>
    /// تحديث إحصائيات التوصيل
    /// </summary>
    public void UpdateDeliveryStatistics(decimal earnings)
    {
        TotalDeliveries++;
        TotalEarnings += earnings;
        LastDeliveryDate = DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// التحقق من صحة الرخصة
    /// </summary>
    public bool IsLicenseValid()
    {
        return LicenseExpiryDate > DateTime.UtcNow;
    }

    /// <summary>
    /// التحقق من إمكانية قبول المهام
    /// </summary>
    public bool CanAcceptDeliveries()
    {
        return Status == DriverStatus.Verified && 
               IsAvailable && 
               IsLicenseValid();
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
