using Domain.Abstractions.Errors;
using Domain.Common;

namespace Domain.Entities.Products;

/// <summary>
/// كيان التصنيف - جذر التجميعة
/// </summary>
public sealed class Category : BaseEntity, IAggregateRoot
{
    // حقول متعددة اللغات
    public string Name { get; private set; } = default!;
    public string? NameAr { get; private set; }
    public string? Description { get; private set; }
    public string? DescriptionAr { get; private set; }
    
    /// <summary>
    /// معرف التصنيف الأب (null للتصنيف الجذر)
    /// </summary>
    public Guid? ParentId { get; private set; }
    
    /// <summary>
    /// مستوى التصنيف في الهرم (0 للجذر)
    /// </summary>
    public int Level { get; private set; }
    
    /// <summary>
    /// مسار التصنيف في الهرم (مثل: "1/5/23")
    /// </summary>
    public string Path { get; private set; } = default!;
    
    /// <summary>
    /// هل هذا تصنيف نهائي (ليس له أطفال)
    /// </summary>
    public bool IsLeaf { get; private set; } = true;
    
    /// <summary>
    /// ترتيب العرض
    /// </summary>
    public int DisplayOrder { get; private set; }

    // للاستخدام مع EF Core
    private Category() { }

    private Category(
        string name,
        string? nameAr,
        string? description,
        string? descriptionAr,
        Guid? parentId,
        int level,
        string path,
        int displayOrder)
    {
        Name = name;
        NameAr = nameAr;
        Description = description;
        DescriptionAr = descriptionAr;
        ParentId = parentId;
        Level = level;
        Path = path;
        DisplayOrder = displayOrder;
        IsLeaf = true;
    }

    /// <summary>
    /// إنشاء تصنيف جذر جديد
    /// </summary>
    public static Category CreateRoot(
        string name,
        string? nameAr = null,
        string? description = null,
        string? descriptionAr = null,
        int displayOrder = 0)
    {
        ValidateTexts(name, nameAr, description, descriptionAr);

        var category = new Category(
            name.Trim(),
            string.IsNullOrWhiteSpace(nameAr) ? null : nameAr.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim(),
            null,
            0,
            string.Empty, // سيتم تحديثه بعد الحفظ
            displayOrder);

        return category;
    }

    /// <summary>
    /// إنشاء تصنيف فرعي
    /// </summary>
    public static Category CreateChild(
        string name,
        string? nameAr,
        string? description,
        string? descriptionAr,
        Guid parentId,
        int parentLevel,
        string parentPath,
        int displayOrder = 0)
    {
        ValidateTexts(name, nameAr, description, descriptionAr);

        if (parentLevel >= 5) // حد أقصى 6 مستويات (0-5)
            throw new DomainRuleViolationException("Maximum category depth exceeded");

        var category = new Category(
            name.Trim(),
            string.IsNullOrWhiteSpace(nameAr) ? null : nameAr.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim(),
            parentId,
            parentLevel + 1,
            string.Empty, // سيتم تحديثه بعد الحفظ
            displayOrder);

        return category;
    }

    /// <summary>
    /// تحديث المسار بعد الحفظ (يستدعى من Infrastructure)
    /// </summary>
    public void UpdatePath(string parentPath = "")
    {
        if (ParentId == null)
        {
            Path = Id.ToString();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(parentPath))
                throw new DomainRuleViolationException("Parent path is required for child categories");

            Path = $"{parentPath}/{Id}";
        }
        
        MarkAsModified();
    }

    /// <summary>
    /// تحديث معلومات التصنيف
    /// </summary>
    public void UpdateInfo(
        string name,
        string? nameAr,
        string? description,
        string? descriptionAr,
        int displayOrder)
    {
        ValidateTexts(name, nameAr, description, descriptionAr);

        Name = name.Trim();
        NameAr = string.IsNullOrWhiteSpace(nameAr) ? null : nameAr.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim();
        DisplayOrder = displayOrder;
        
        MarkAsModified();
    }

    /// <summary>
    /// تحديد ما إذا كان التصنيف نهائياً
    /// </summary>
    public void SetLeafStatus(bool isLeaf)
    {
        if (IsLeaf != isLeaf)
        {
            IsLeaf = isLeaf;
            MarkAsModified();
        }
    }

    /// <summary>
    /// نقل التصنيف إلى أب جديد
    /// </summary>
    public void MoveTo(Guid? newParentId, int newParentLevel, string newParentPath)
    {
        if (newParentId == Id)
            throw new DomainRuleViolationException("Category cannot be its own parent");

        if (newParentLevel >= 5)
            throw new DomainRuleViolationException("Maximum category depth exceeded");

        ParentId = newParentId;
        Level = newParentId == null ? 0 : newParentLevel + 1;
        
        // تحديث المسار
        if (newParentId == null)
        {
            Path = Id.ToString();
        }
        else
        {
            Path = $"{newParentPath}/{Id}";
        }
        
        MarkAsModified();
    }

    /// <summary>
    /// التحقق من صحة النصوص
    /// </summary>
    private static void ValidateTexts(string name, string? nameAr, string? description, string? descriptionAr)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleViolationException("Category name is required");

        if (name.Length > 100)
            throw new DomainRuleViolationException("Category name cannot exceed 100 characters");

        if (!string.IsNullOrWhiteSpace(nameAr) && nameAr.Length > 100)
            throw new DomainRuleViolationException("Category name in Arabic cannot exceed 100 characters");

        if (!string.IsNullOrWhiteSpace(description) && description.Length > 500)
            throw new DomainRuleViolationException("Category description cannot exceed 500 characters");

        if (!string.IsNullOrWhiteSpace(descriptionAr) && descriptionAr.Length > 500)
            throw new DomainRuleViolationException("Category description in Arabic cannot exceed 500 characters");
    }
}
