using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Payments;
using Domain.Events.Payments;

namespace Domain.Entities.Payments;

/// <summary>
/// كيان الدفع - جذر التجميعة
/// </summary>
public sealed class Payment : BaseEntity, IAggregateRoot
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Initiated;
    
    /// <summary>
    /// مرجع البوابة الخارجية للدفع
    /// </summary>
    public string? GatewayReference { get; private set; }
    
    /// <summary>
    /// رمز الخطأ في حالة فشل الدفع
    /// </summary>
    public string? ErrorCode { get; private set; }
    
    /// <summary>
    /// رسالة الخطأ في حالة فشل الدفع
    /// </summary>
    public string? ErrorMessage { get; private set; }
    
    /// <summary>
    /// تاريخ نجاح الدفع
    /// </summary>
    public DateTime? SucceededAtUtc { get; private set; }
    
    /// <summary>
    /// تاريخ فشل الدفع
    /// </summary>
    public DateTime? FailedAtUtc { get; private set; }

    // للاستخدام مع EF Core
    private Payment() { }

    private Payment(Guid orderId, decimal amount, PaymentMethod method)
    {
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Initiated;
    }

    /// <summary>
    /// إنشاء دفعة جديدة
    /// </summary>
    public static Payment Create(Guid orderId, decimal amount, PaymentMethod method)
    {
        if (amount <= 0)
            throw new DomainRuleViolationException("Payment amount must be positive");

        if (amount > 1_000_000) // حد أقصى مليون
            throw new DomainRuleViolationException("Payment amount cannot exceed 1,000,000");

        var payment = new Payment(orderId, amount, method);
        payment.AddDomainEvent(new PaymentInitiatedEvent(payment.Id, orderId, amount, method));
        return payment;
    }

    /// <summary>
    /// تحديد حالة الدفع كناجح
    /// </summary>
    public void MarkSucceeded(string gatewayReference)
    {
        if (Status == PaymentStatus.Succeeded)
            return; // مسبقاً ناجح

        if (Status == PaymentStatus.Failed)
            throw new DomainRuleViolationException("Cannot mark failed payment as succeeded");

        if (string.IsNullOrWhiteSpace(gatewayReference))
            throw new DomainRuleViolationException("Gateway reference is required for successful payment");

        Status = PaymentStatus.Succeeded;
        GatewayReference = gatewayReference.Trim();
        SucceededAtUtc = DateTime.UtcNow;
        ErrorCode = null;
        ErrorMessage = null;
        FailedAtUtc = null;
        MarkAsModified();

        AddDomainEvent(new PaymentSucceededEvent(Id, OrderId, Amount, gatewayReference));
    }

    /// <summary>
    /// تحديد حالة الدفع كفاشل
    /// </summary>
    public void MarkFailed(string errorCode, string? errorMessage = null)
    {
        if (Status == PaymentStatus.Failed)
            return; // مسبقاً فاشل

        if (Status == PaymentStatus.Succeeded)
            throw new DomainRuleViolationException("Cannot mark succeeded payment as failed");

        if (string.IsNullOrWhiteSpace(errorCode))
            throw new DomainRuleViolationException("Error code is required for failed payment");

        if (errorCode.Length > 50)
            throw new DomainRuleViolationException("Error code cannot exceed 50 characters");

        if (!string.IsNullOrWhiteSpace(errorMessage) && errorMessage.Length > 500)
            throw new DomainRuleViolationException("Error message cannot exceed 500 characters");

        Status = PaymentStatus.Failed;
        ErrorCode = errorCode.Trim();
        ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? null : errorMessage.Trim();
        FailedAtUtc = DateTime.UtcNow;
        GatewayReference = null;
        SucceededAtUtc = null;
        MarkAsModified();

        AddDomainEvent(new PaymentFailedEvent(Id, OrderId, Amount, errorCode, errorMessage));
    }

    /// <summary>
    /// تحديد حالة الدفع كمعلق
    /// </summary>
    public void MarkPending()
    {
        if (Status == PaymentStatus.Succeeded || Status == PaymentStatus.Failed)
            throw new DomainRuleViolationException("Cannot mark completed payment as pending");

        Status = PaymentStatus.Pending;
        MarkAsModified();
    }

    /// <summary>
    /// التحقق من كون الدفع مكتملاً
    /// </summary>
    public bool IsCompleted() => Status == PaymentStatus.Succeeded || Status == PaymentStatus.Failed;

    /// <summary>
    /// التحقق من نجاح الدفع
    /// </summary>
    public bool IsSuccessful() => Status == PaymentStatus.Succeeded;

    /// <summary>
    /// التحقق من فشل الدفع
    /// </summary>
    public bool IsFailed() => Status == PaymentStatus.Failed;
}
