namespace Domain.Enums.Orders;

/// <summary>
/// حالات العرض
/// </summary>
public enum OfferStatus
{
    /// <summary>
    /// مسودة
    /// </summary>
    Draft,
    
    /// <summary>
    /// نشط
    /// </summary>
    Active,
    
    /// <summary>
    /// منتهي الصلاحية
    /// </summary>
    Expired,
    
    /// <summary>
    /// مؤرشف
    /// </summary>
    Archived
}
