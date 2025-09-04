using Domain.Abstractions.Events;

namespace Domain.Events.Users;

/// <summary>
/// حدث إنشاء ملف سائق
/// </summary>
public record DriverProfileCreatedEvent(Guid DriverProfileId, Guid UserId, string LicenseNumber) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث التحقق من السائق
/// </summary>
public record DriverVerifiedEvent(Guid DriverProfileId, Guid UserId, string VerifiedBy) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث رفض السائق
/// </summary>
public record DriverRejectedEvent(Guid DriverProfileId, Guid UserId, string Reason, string RejectedBy) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث تعليق السائق
/// </summary>
public record DriverSuspendedEvent(Guid DriverProfileId, Guid UserId, string Reason, string SuspendedBy) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث تغيير حالة توفر السائق
/// </summary>
public record DriverAvailabilityChangedEvent(Guid DriverProfileId, Guid UserId, bool IsAvailable) : DomainEventBase(DateTime.UtcNow);
