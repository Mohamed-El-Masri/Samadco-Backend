using Domain.Abstractions.Events;

namespace Domain.Events.Users;

/// <summary>
/// حدث حذف مستخدم (ناعم)
/// </summary>
public sealed record UserDeletedEvent(
    Guid UserId,
    string FullName,
    string Email
) : DomainEventBase;
