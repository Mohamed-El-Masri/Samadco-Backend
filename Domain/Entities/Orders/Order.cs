using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Orders;
using Domain.Enums.Payments;
using Domain.Events.Orders;

namespace Domain.Entities.Orders;

/// <summary>
/// كيان الطلب - جذر التجميعة
/// </summary>
public sealed class Order : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid QuoteId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.PendingDeposit;
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Initiated;
    
    /// <summary>
    /// مبلغ العربون المطلوب (10% من إجمالي العرض)
    /// </summary>
    public decimal DepositAmount { get; private set; }
    
    /// <summary>
    /// رابط صورة الهوية الوطنية
    /// </summary>
    public string? NationalIdImageUrl { get; private set; }
    
    /// <summary>
    /// رقم التتبع للشحن
    /// </summary>
    public string? TrackingNumber { get; private set; }
    
    /// <summary>
    /// تاريخ تأكيد الطلب
    /// </summary>
    public DateTime? ConfirmedAtUtc { get; private set; }
    
    /// <summary>
    /// تاريخ بدء المعالجة
    /// </summary>
    public DateTime? ProcessingStartedAtUtc { get; private set; }
    
    /// <summary>
    /// تاريخ الشحن
    /// </summary>
    public DateTime? ShippedAtUtc { get; private set; }
    
    /// <summary>
    /// تاريخ التسليم
    /// </summary>
    public DateTime? DeliveredAtUtc { get; private set; }
    
    /// <summary>
    /// تاريخ الإلغاء
    /// </summary>
    public DateTime? CancelledAtUtc { get; private set; }
    
    /// <summary>
    /// سبب الإلغاء
    /// </summary>
    public string? CancellationReason { get; private set; }

    // للاستخدام مع EF Core
    private Order() { }

    private Order(Guid userId, Guid quoteId, decimal depositAmount)
    {
        UserId = userId;
        QuoteId = quoteId;
        DepositAmount = depositAmount;
        Status = OrderStatus.PendingDeposit;
        PaymentStatus = PaymentStatus.Initiated;
    }

    /// <summary>
    /// إنشاء طلب جديد
    /// </summary>
    public static Order Create(Guid userId, Guid quoteId, decimal quoteTotal)
    {
        if (quoteTotal <= 0)
            throw new DomainRuleViolationException("Quote total must be positive");

        // حساب العربون (10%)
        var depositAmount = Math.Round(quoteTotal * 0.10m, 2);

        var order = new Order(userId, quoteId, depositAmount);
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, userId, quoteId));
        return order;
    }

    /// <summary>
    /// تسجيل دفع العربون
    /// </summary>
    public void RegisterDeposit(decimal amount)
    {
        if (Status != OrderStatus.PendingDeposit)
            throw new DomainRuleViolationException("Order is not waiting for deposit");

        if (amount < DepositAmount)
            throw new DomainRuleViolationException($"Deposit amount insufficient. Required: {DepositAmount}, Paid: {amount}");

        PaymentStatus = PaymentStatus.Succeeded;
        MarkAsModified();

        AddDomainEvent(new OrderDepositPaidEvent(Id, UserId, amount));
    }

    /// <summary>
    /// تأكيد الطلب مع صورة الهوية
    /// </summary>
    public void Confirm(string nationalIdImageUrl)
    {
        if (Status != OrderStatus.PendingDeposit)
            throw new DomainRuleViolationException("Order is not in pending deposit status");

        if (PaymentStatus != PaymentStatus.Succeeded)
            throw new DomainRuleViolationException("Cannot confirm order before successful deposit payment");

        if (string.IsNullOrWhiteSpace(nationalIdImageUrl))
            throw new DomainRuleViolationException("National ID image URL is required");

        Status = OrderStatus.Confirmed;
        NationalIdImageUrl = nationalIdImageUrl.Trim();
        ConfirmedAtUtc = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new OrderConfirmedEvent(Id, UserId, nationalIdImageUrl));
    }

    /// <summary>
    /// تطوير الطلب للمعالجة
    /// </summary>
    public void AdvanceToProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainRuleViolationException("Only confirmed orders can advance to processing");

        Status = OrderStatus.Processing;
        ProcessingStartedAtUtc = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new OrderProcessingStartedEvent(Id, UserId));
    }

    /// <summary>
    /// شحن الطلب
    /// </summary>
    public void Ship(string? trackingNumber = null)
    {
        if (Status != OrderStatus.Processing)
            throw new DomainRuleViolationException("Only processing orders can be shipped");

        Status = OrderStatus.Shipped;
        TrackingNumber = string.IsNullOrWhiteSpace(trackingNumber) ? null : trackingNumber.Trim();
        ShippedAtUtc = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new OrderShippedEvent(Id, UserId, TrackingNumber));
    }

    /// <summary>
    /// تسليم الطلب
    /// </summary>
    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainRuleViolationException("Only shipped orders can be delivered");

        Status = OrderStatus.Delivered;
        DeliveredAtUtc = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new OrderDeliveredEvent(Id, UserId));
    }

    /// <summary>
    /// إلغاء الطلب
    /// </summary>
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Cancelled)
            return; // مسبقاً ملغي

        if (Status == OrderStatus.Delivered)
            throw new DomainRuleViolationException("Cannot cancel delivered order");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainRuleViolationException("Cancellation reason is required");

        if (reason.Length > 500)
            throw new DomainRuleViolationException("Cancellation reason cannot exceed 500 characters");

        Status = OrderStatus.Cancelled;
        CancellationReason = reason.Trim();
        CancelledAtUtc = DateTime.UtcNow;
        MarkAsModified();

        AddDomainEvent(new OrderCancelledEvent(Id, UserId, reason));
    }

    /// <summary>
    /// تحديث رقم التتبع
    /// </summary>
    public void UpdateTrackingNumber(string trackingNumber)
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainRuleViolationException("Can only update tracking number for shipped orders");

        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new DomainRuleViolationException("Tracking number cannot be empty");

        TrackingNumber = trackingNumber.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// التحقق من إمكانية الإلغاء
    /// </summary>
    public bool CanBeCancelled() => Status != OrderStatus.Cancelled && Status != OrderStatus.Delivered;

    /// <summary>
    /// التحقق من إمكانية التعديل
    /// </summary>
    public bool CanBeModified() => Status == OrderStatus.PendingDeposit || Status == OrderStatus.Confirmed;
}
