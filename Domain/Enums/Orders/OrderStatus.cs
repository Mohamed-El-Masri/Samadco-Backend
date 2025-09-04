namespace Domain.Enums.Orders;

/// <summary>
/// حالات الطلب
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// في انتظار العربون
    /// </summary>
    PendingDeposit,
    
    /// <summary>
    /// مؤكد
    /// </summary>
    Confirmed,
    
    /// <summary>
    /// قيد المعالجة
    /// </summary>
    Processing,
    
    /// <summary>
    /// تم الشحن
    /// </summary>
    Shipped,
    
    /// <summary>
    /// تم التسليم
    /// </summary>
    Delivered,
    
    /// <summary>
    /// ملغي
    /// </summary>
    Cancelled
}
