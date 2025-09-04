namespace Domain.Enums.Orders;

/// <summary>
/// حالات طلب التسعير
/// </summary>
public enum QuoteRequestStatus
{
    /// <summary>
    /// في انتظار التسعير
    /// </summary>
    Pending,
    
    /// <summary>
    /// تم التسعير
    /// </summary>
    Priced,
    
    /// <summary>
    /// منتهي الصلاحية
    /// </summary>
    Expired
}
