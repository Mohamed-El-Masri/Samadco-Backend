using Domain.Abstractions.Events;
using Domain.Enums;

namespace Domain.Events.Drivers;

/// <summary>
/// حدث تقديم طلب سائق جديد
/// </summary>
public sealed record DriverApplicationSubmittedEvent(
    Guid ApplicationId,
    Guid UserId,
    string VehicleInfo) : DomainEventBase;

/// <summary>
/// حدث الموافقة على طلب السائق
/// </summary>
public sealed record DriverApplicationApprovedEvent(
    Guid ApplicationId,
    Guid UserId) : DomainEventBase;

/// <summary>
/// حدث رفض طلب السائق
/// </summary>
public sealed record DriverApplicationRejectedEvent(
    Guid ApplicationId,
    Guid UserId,
    string Reason) : DomainEventBase;
