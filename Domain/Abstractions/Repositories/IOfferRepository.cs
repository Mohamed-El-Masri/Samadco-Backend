using Domain.Entities.Offers;
using Domain.Entities.Carts;
using Domain.Entities.Quotes;

namespace Domain.Abstractions.Repositories;

/// <summary>
/// واجهة مستودع العروض
/// </summary>
public interface IOfferRepository : IRepository<Offer>
{
    /// <summary>
    /// الحصول على العروض النشطة
    /// </summary>
    Task<IEnumerable<Offer>> GetActiveOffersAsync(DateTime now, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على العروض التي انتهت صلاحيتها
    /// </summary>
    Task<IEnumerable<Offer>> GetExpiredOffersAsync(DateTime now, CancellationToken cancellationToken = default);

    /// <summary>
    /// البحث في العروض
    /// </summary>
    Task<IEnumerable<Offer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

/// <summary>
/// واجهة مستودع السلة
/// </summary>
public interface ICartRepository : IRepository<Cart>
{
    /// <summary>
    /// الحصول على سلة المستخدم
    /// </summary>
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على السلات القديمة للتنظيف
    /// </summary>
    Task<IEnumerable<Cart>> GetOldCartsAsync(DateTime olderThan, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على السلات المقفلة
    /// </summary>
    Task<IEnumerable<Cart>> GetLockedCartsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// واجهة مستودع طلبات التسعير
/// </summary>
public interface IQuoteRequestRepository : IRepository<QuoteRequest>
{
    /// <summary>
    /// الحصول على طلبات المستخدم
    /// </summary>
    Task<IEnumerable<QuoteRequest>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الطلبات المعلقة
    /// </summary>
    Task<IEnumerable<QuoteRequest>> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على الطلبات التي انتهت صلاحيتها
    /// </summary>
    Task<IEnumerable<QuoteRequest>> GetExpiredAsync(DateTime now, CancellationToken cancellationToken = default);
}

/// <summary>
/// واجهة مستودع عروض الأسعار
/// </summary>
public interface IQuoteRepository : IRepository<Quote>
{
    /// <summary>
    /// الحصول على عرض السعر النشط
    /// </summary>
    Task<Quote?> GetActiveAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على عروض أسعار المستخدم
    /// </summary>
    Task<IEnumerable<Quote>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على عرض السعر بطلب التسعير
    /// </summary>
    Task<Quote?> GetByQuoteRequestIdAsync(Guid quoteRequestId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على عروض الأسعار التي انتهت صلاحيتها
    /// </summary>
    Task<IEnumerable<Quote>> GetExpiredAsync(DateTime now, CancellationToken cancellationToken = default);
}
