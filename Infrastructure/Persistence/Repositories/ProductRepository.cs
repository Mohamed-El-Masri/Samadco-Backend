using Domain.Abstractions.Repositories;
using Domain.Entities.Products;
using Domain.Enums.Products;
using Infrastructure.Data;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// مستودع المنتجات
/// </summary>
public sealed class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(SemadcoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.SellerId == sellerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetApprovedProductsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.Status == ProductStatus.Approved)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetPendingApprovalAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.Status == ProductStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.Name.Contains(searchTerm) || 
                       (p.NameAr != null && p.NameAr.Contains(searchTerm)))
            .ToListAsync(cancellationToken);
    }
}
