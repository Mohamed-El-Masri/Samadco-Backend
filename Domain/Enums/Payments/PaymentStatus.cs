namespace Domain.Enums.Payments;

/// <summary>
/// حالات الدفع
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// تم البدء
    /// </summary>
    Initiated,
    
    /// <summary>
    /// في انتظار المعالجة
    /// </summary>
    Pending,
    
    /// <summary>
    /// نجح
    /// </summary>
    Succeeded,
    
    /// <summary>
    /// فشل
    /// </summary>
    Failed
}
