using Domain.Entities.Products;

namespace Domain.Abstractions.Repositories;

/// <summary>
/// واجهة مستودع التصنيفات
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    /// <summary>
    /// الحصول على التصنيفات الجذر
    /// </summary>
    Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على التصنيفات الفرعية
    /// </summary>
    Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على التصنيفات حسب المستوى
    /// </summary>
    Task<IEnumerable<Category>> GetByLevelAsync(int level, CancellationToken cancellationToken = default);

    /// <summary>
    /// الحصول على التصنيفات النهائية
    /// </summary>
    Task<IEnumerable<Category>> GetLeafCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// البحث عن تصنيف بالمسار
    /// </summary>
    Task<Category?> GetByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// البحث في التصنيفات بالاسم
    /// </summary>
    Task<IEnumerable<Category>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من وجود تصنيفات فرعية
    /// </summary>
    Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
