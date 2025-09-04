using Domain.Entities.Products;
using Domain.Enums.Products;

namespace Domain.Abstractions.Repositories;

/// <summary>
/// واجهة مستودع المنتجات
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// الحصول على منتجات البائع
    /// </summary>
    Task<IEnumerable<Product>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على المنتجات حسب التصنيف
    /// </summary>
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على المنتجات حسب الحالة
    /// </summary>
    Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على المنتجات المعتمدة
    /// </summary>
    Task<IEnumerable<Product>> GetApprovedProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على المنتجات المعلقة
    /// </summary>
    Task<IEnumerable<Product>> GetPendingApprovalAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// البحث في المنتجات بالاسم
    /// </summary>
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
}
