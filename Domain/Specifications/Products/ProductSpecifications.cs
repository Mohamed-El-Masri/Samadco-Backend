using Domain.Entities.Products;
using Domain.Enums.Products;
using System.Linq.Expressions;

namespace Domain.Specifications.Products;

/// <summary>
/// مواصفة المنتجات المعتمدة
/// </summary>
public sealed class ApprovedProductsSpecification : Specification<Product>
{
    public override Expression<Func<Product, bool>> Criteria =>
        product => product.Status == ProductStatus.Approved;
}

/// <summary>
/// مواصفة المنتجات المعلقة للمراجعة
/// </summary>
public sealed class PendingProductsSpecification : Specification<Product>
{
    public override Expression<Func<Product, bool>> Criteria =>
        product => product.Status == ProductStatus.Pending;
}

/// <summary>
/// مواصفة المنتجات المرفوضة
/// </summary>
public sealed class RejectedProductsSpecification : Specification<Product>
{
    public override Expression<Func<Product, bool>> Criteria =>
        product => product.Status == ProductStatus.Rejected;
}

/// <summary>
/// مواصفة منتجات بائع محدد
/// </summary>
public sealed class ProductsBySellerSpecification : Specification<Product>
{
    private readonly Guid _sellerId;

    public ProductsBySellerSpecification(Guid sellerId)
    {
        _sellerId = sellerId;
    }

    public override Expression<Func<Product, bool>> Criteria =>
        product => product.SellerId == _sellerId;
}

/// <summary>
/// مواصفة منتجات تصنيف محدد
/// </summary>
public sealed class ProductsByCategorySpecification : Specification<Product>
{
    private readonly Guid _categoryId;

    public ProductsByCategorySpecification(Guid categoryId)
    {
        _categoryId = categoryId;
    }

    public override Expression<Func<Product, bool>> Criteria =>
        product => product.CategoryId == _categoryId;
}

/// <summary>
/// مواصفة البحث في أسماء المنتجات
/// </summary>
public sealed class ProductNameSearchSpecification : Specification<Product>
{
    private readonly string _searchTerm;

    public ProductNameSearchSpecification(string searchTerm)
    {
        _searchTerm = searchTerm?.ToLowerInvariant() ?? string.Empty;
    }

    public override Expression<Func<Product, bool>> Criteria =>
        product => product.Name.ToLower().Contains(_searchTerm) ||
                   (product.NameAr != null && product.NameAr.ToLower().Contains(_searchTerm));
}

/// <summary>
/// مواصفة المنتجات التي تحتوي على صور
/// </summary>
public sealed class ProductsWithImagesSpecification : Specification<Product>
{
    public override Expression<Func<Product, bool>> Criteria =>
        product => product.Images.Any();
}

/// <summary>
/// مواصفة المنتجات المعتمدة لبائع محدد
/// </summary>
public sealed class ApprovedProductsBySellerSpecification : Specification<Product>
{
    private readonly Guid _sellerId;

    public ApprovedProductsBySellerSpecification(Guid sellerId)
    {
        _sellerId = sellerId;
    }

    public override Expression<Func<Product, bool>> Criteria =>
        product => product.SellerId == _sellerId && product.Status == ProductStatus.Approved;
}
