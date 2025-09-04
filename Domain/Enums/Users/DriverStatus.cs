namespace Domain.Enums.Users;

/// <summary>
/// حالة السائق
/// </summary>
public enum DriverStatus
{
    /// <summary>
    /// في انتظار التحقق
    /// </summary>
    PendingVerification = 1,
    
    /// <summary>
    /// محقق
    /// </summary>
    Verified = 2,
    
    /// <summary>
    /// مرفوض
    /// </summary>
    Rejected = 3,
    
    /// <summary>
    /// معلق
    /// </summary>
    Suspended = 4
}
