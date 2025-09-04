namespace Domain.Enums.Drivers;

/// <summary>
/// حالات طلب السائق
/// </summary>
public enum DriverApplicationStatus
{
    /// <summary>
    /// في انتظار المراجعة
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
