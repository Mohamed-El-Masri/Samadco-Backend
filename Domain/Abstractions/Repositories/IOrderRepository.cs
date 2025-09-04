using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Notifications;
using Domain.Entities.Distributors;
using Domain.Entities.Drivers;
using Domain.Enums.Orders;
using Domain.Enums.Payments;
using Domain.Enums.Drivers;

namespace Domain.Abstractions.Repositories;

/// <summary>
/// واجهة مستودع الطلبات
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// الحصول على الطلب مع المدفوعات
    /// </summary>
    Task<Order?> GetWithPaymentsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على طلبات المستخدم
    /// </summary>
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الطلبات حسب الحالة
    /// </summary>
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الطلبات بعرض السعر
    /// </summary>
    Task<Order?> GetByQuoteIdAsync(Guid quoteId, CancellationToken cancellationToken = default);
}

/// <summary>
/// واجهة مستودع المدفوعات
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>
    /// الحصول على مدفوعات الطلب
    /// </summary>
    Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الدفع بمرجع البوابة
    /// </summary>
    Task<Payment?> GetByGatewayReferenceAsync(string gatewayReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على المدفوعات حسب الحالة
    /// </summary>
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);
}

/// <summary>
/// واجهة مستودع الإشعارات
/// </summary>
public interface INotificationRepository : IRepository<Notification>
{
    /// <summary>
    /// الحصول على إشعارات المستخدم
    /// </summary>
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, bool includeRead = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الإشعارات غير المقروءة للمستخدم
    /// </summary>
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على عدد الإشعارات غير المقروءة للمستخدم
    /// </summary>
    Task<int> GetUnreadCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الإشعارات العامة
    /// </summary>
    Task<IEnumerable<Notification>> GetGlobalNotificationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الإشعارات المنتهية الصلاحية
    /// </summary>
    Task<IEnumerable<Notification>> GetExpiredNotificationsAsync(DateTime now, CancellationToken cancellationToken = default);
}

/// <summary>
/// واجهة مستودع الموزعين
/// </summary>
public interface IDistributorRepository : IRepository<Distributor>
{
    /// <summary>
    /// الحصول على الموزعين النشطين
    /// </summary>
    Task<IEnumerable<Distributor>> GetActiveDistributorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الموزعين حسب المنطقة
    /// </summary>
    Task<IEnumerable<Distributor>> GetByRegionAsync(string region, CancellationToken cancellationToken = default);

    /// <summary>
    /// البحث في الموزعين
    /// </summary>
    Task<IEnumerable<Distributor>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

/// <summary>
/// واجهة مستودع طلبات السائقين
/// </summary>
public interface IDriverApplicationRepository : IRepository<DriverApplication>
{
    /// <summary>
    /// الحصول على طلبات المستخدم
    /// </summary>
    Task<IEnumerable<DriverApplication>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الطلبات المعلقة
    /// </summary>
    Task<IEnumerable<DriverApplication>> GetPendingApplicationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الطلبات حسب الحالة
    /// </summary>
    Task<IEnumerable<DriverApplication>> GetByStatusAsync(DriverApplicationStatus status, CancellationToken cancellationToken = default);
}
