namespace Domain.Enums.Users;

/// <summary>
/// حالات المستخدم
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// في انتظار التفعيل
    /// </summary>
    PendingActivation,
    
    /// <summary>
    /// نشط
    /// </summary>
    Active,
    
    /// <summary>
    /// معلق
    /// </summary>
    Suspended,
    
    /// <summary>
    /// محذوف
    /// </summary>
    Deleted
}
