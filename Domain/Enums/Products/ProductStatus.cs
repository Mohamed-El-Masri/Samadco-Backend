namespace Domain.Enums.Products;

/// <summary>
/// حالات المنتج
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// في انتظار الموافقة
    /// </summary>
    Pending,
    
    /// <summary>
    /// موافق عليه
    /// </summary>
    Approved,
    
    /// <summary>
    /// مرفوض
    /// </summary>
    Rejected
}
