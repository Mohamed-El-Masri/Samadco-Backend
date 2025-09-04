namespace Domain.Enums.Users;

/// <summary>
/// صلاحيات الإداري
/// </summary>
[Flags]
public enum AdminPermissions
{
    /// <summary>
    /// بدون صلاحيات
    /// </summary>
    None = 0,
    
    /// <summary>
    /// إدارة المستخدمين
    /// </summary>
    ManageUsers = 1,
    
    /// <summary>
    /// إدارة المنتجات
    /// </summary>
    ManageProducts = 2,
    
    /// <summary>
    /// إدارة الطلبات
    /// </summary>
    ManageOrders = 4,
    
    /// <summary>
    /// إدارة التوصيل
    /// </summary>
    ManageDelivery = 8,
    
    /// <summary>
    /// إدارة المدفوعات
    /// </summary>
    ManagePayments = 16,
    
    /// <summary>
    /// عرض التقارير
    /// </summary>
    ViewReports = 32,
    
    /// <summary>
    /// إدارة النظام
    /// </summary>
    ManageSystem = 64,
    
    /// <summary>
    /// جميع الصلاحيات
    /// </summary>
    All = ManageUsers | ManageProducts | ManageOrders | ManageDelivery | ManagePayments | ViewReports | ManageSystem
}
