using Domain.Abstractions.Events;

namespace Domain.Events.Users;

/// <summary>
/// حدث استعادة مستخدم محذوف
/// </summary>
public sealed record UserRestoredEvent(
    Guid UserId,
    string FullName,
    string Email
) : DomainEventBase;
