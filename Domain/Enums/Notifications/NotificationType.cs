namespace Domain.Enums.Notifications;

/// <summary>
/// أنواع الإشعارات
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// إشعار عام
    /// </summary>
    General,
    
    /// <summary>
    /// إشعار طلب
    /// </summary>
    Order,
    
    /// <summary>
    /// إشعار دفع
    /// </summary>
    Payment,
    
    /// <summary>
    /// إشعار منتج
    /// </summary>
    Product,
    
    /// <summary>
    /// إشعار عرض
    /// </summary>
    Offer,
    
    /// <summary>
    /// إشعار تسعير
    /// </summary>
    Quote
}
