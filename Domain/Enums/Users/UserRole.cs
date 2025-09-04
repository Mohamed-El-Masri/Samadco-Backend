namespace Domain.Enums.Users;

/// <summary>
/// أدوار المستخدمين في النظام
/// </summary>
public enum UserRole
{
    /// <summary>
    /// مدير النظام - يمكنه الوصول لجميع الوظائف
    /// </summary>
    Admin = 1,
    
    /// <summary>
    /// مشتري - يمكنه تصفح المنتجات وإجراء الطلبات
    /// </summary>
    Buyer = 2,
    
    /// <summary>
    /// بائع - يمكنه إدارة المنتجات والطلبات
    /// </summary>
    Seller = 3,
    
    /// <summary>
    /// سائق - يمكنه استلام طلبات التوصيل
    /// </summary>
    Driver = 4
}
