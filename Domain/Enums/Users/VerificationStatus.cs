namespace Domain.Enums.Users;

/// <summary>
/// حالات التحقق
/// </summary>
public enum VerificationStatus
{
    /// <summary>
    /// في انتظار التحقق
    /// </summary>
    Pending,
    
    /// <summary>
    /// تم التحقق
    /// </summary>
    Verified,
    
    /// <summary>
    /// مرفوض
    /// </summary>
    Rejected
}
