using Domain.Abstractions.Events;

namespace Domain.Events.Users;

/// <summary>
/// حدث إنشاء ملف إداري
/// </summary>
public record AdminProfileCreatedEvent(Guid AdminProfileId, Guid UserId, string Department) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث تحديث صلاحيات الإداري
/// </summary>
public record AdminPermissionsUpdatedEvent(Guid AdminProfileId, Guid UserId, object OldPermissions, object NewPermissions, string UpdatedBy) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث تحديث قسم الإداري
/// </summary>
public record AdminDepartmentUpdatedEvent(Guid AdminProfileId, Guid UserId, string OldDepartment, string NewDepartment) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث تسجيل إجراء إداري
/// </summary>
public record AdminActionPerformedEvent(Guid AdminProfileId, Guid UserId, string ActionType) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث تعطيل الإداري
/// </summary>
public record AdminDisabledEvent(Guid AdminProfileId, Guid UserId, string Reason, string DisabledBy) : DomainEventBase(DateTime.UtcNow);

/// <summary>
/// حدث تفعيل الإداري
/// </summary>
public record AdminEnabledEvent(Guid AdminProfileId, Guid UserId, string EnabledBy) : DomainEventBase(DateTime.UtcNow);
