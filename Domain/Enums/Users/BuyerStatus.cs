namespace Domain.Enums.Users;

/// <summary>
/// حالة المشتري
/// </summary>
public enum BuyerStatus
{
    /// <summary>
    /// نشط
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// معلق
    /// </summary>
    Suspended = 2,
    
    /// <summary>
    /// محظور
    /// </summary>
    Banned = 3
}
