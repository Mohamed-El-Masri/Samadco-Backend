using Domain.Abstractions.Events;
using Domain.Enums;

namespace Domain.Events.Products;

/// <summary>
/// حدث إنشاء منتج جديد في انتظار الموافقة
/// </summary>
public sealed record ProductCreatedPendingApprovalEvent(
    Guid ProductId,
    Guid SellerId,
    string ProductName) : DomainEventBase;

/// <summary>
/// حدث الموافقة على منتج
/// </summary>
public sealed record ProductApprovedEvent(
    Guid ProductId,
    Guid SellerId,
    string AdminId) : DomainEventBase;

/// <summary>
/// حدث رفض منتج
/// </summary>
public sealed record ProductRejectedEvent(
    Guid ProductId,
    Guid SellerId,
    string AdminId,
    string Reason) : DomainEventBase;

/// <summary>
/// حدث تحديث منتج
/// </summary>
public sealed record ProductUpdatedEvent(
    Guid ProductId,
    Guid SellerId) : DomainEventBase;
