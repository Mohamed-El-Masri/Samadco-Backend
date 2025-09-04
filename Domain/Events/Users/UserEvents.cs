using Domain.Abstractions.Events;
using Domain.Enums;

namespace Domain.Events.Users;

/// <summary>
/// حدث إنشاء مستخدم جديد
/// </summary>
public sealed record UserCreatedEvent(
    Guid UserId,
    string Email,
    string FullName) : DomainEventBase;

/// <summary>
/// حدث تفعيل مستخدم
/// </summary>
public sealed record UserActivatedEvent(
    Guid UserId) : DomainEventBase;

/// <summary>
/// حدث إلغاء تفعيل مستخدم
/// </summary>
public sealed record UserDeactivatedEvent(
    Guid UserId,
    string Reason) : DomainEventBase;

/// <summary>
/// حدث إضافة دور للمستخدم
/// </summary>
public sealed record UserRoleAddedEvent(
    Guid UserId,
    string RoleName) : DomainEventBase;

/// <summary>
/// حدث إزالة دور من المستخدم
/// </summary>
public sealed record UserRoleRemovedEvent(
    Guid UserId,
    string RoleName) : DomainEventBase;

/// <summary>
/// حدث إنشاء ملف بائع
/// </summary>
public sealed record SellerProfileCreatedEvent(
    Guid UserId,
    string CompanyName) : DomainEventBase;

/// <summary>
/// حدث التحقق من ملف البائع
/// </summary>
public sealed record SellerProfileVerifiedEvent(
    Guid UserId,
    string CompanyName,
    string TaxId) : DomainEventBase;

/// <summary>
/// حدث رفض ملف البائع
/// </summary>
public sealed record SellerProfileRejectedEvent(
    Guid UserId,
    string Reason) : DomainEventBase;
