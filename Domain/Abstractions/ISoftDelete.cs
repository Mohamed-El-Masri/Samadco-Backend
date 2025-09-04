namespace Domain.Abstractions;

/// <summary>
/// واجهة للكيانات القابلة للحذف الناعم
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// تحديد ما إذا كان الكيان محذوفاً نعماً
    /// </summary>
    bool IsDeleted { get; }
    
    /// <summary>
    /// تاريخ الحذف
    /// </summary>
    DateTime? DeletedAtUtc { get; }
    
    /// <summary>
    /// حذف الكيان نعماً
    /// </summary>
    void Delete();
    
    /// <summary>
    /// استعادة الكيان المحذوف
    /// </summary>
    void Restore();
}
