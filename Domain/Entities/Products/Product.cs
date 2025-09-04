using Domain.Abstractions;
using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Products;
using Domain.Events.Products;
using Domain.ValueObjects.Media;
using Domain.ValueObjects.Products;

namespace Domain.Entities.Products;

/// <summary>
/// كيان المنتج - جذر التجميعة
/// </summary>
public sealed class Product : BaseEntity, IAggregateRoot, ISoftDelete
{
    private readonly List<ImageRef> _images = new();

    public Guid SellerId { get; private set; }
    public Guid CategoryId { get; private set; }
    
    // حقول متعددة اللغات
    public string Name { get; private set; } = default!;
    public string? NameAr { get; private set; }
    public string Description { get; private set; } = default!;
    public string? DescriptionAr { get; private set; }
    
    /// <summary>
    /// صور المنتج
    /// </summary>
    public IReadOnlyList<ImageRef> Images => _images.AsReadOnly();
    
    /// <summary>
    /// مواصفات المنتج بصيغة JSON
    /// </summary>
    public JsonSpec? Specs { get; private set; }
    
    public ProductStatus Status { get; private set; } = ProductStatus.Pending;
    public DateTime? ApprovedAtUtc { get; private set; }
    public string? RejectedReason { get; private set; }
    public string? ApprovedBy { get; private set; }

    // للاستخدام مع EF Core
    private Product() { }

    private Product(
        Guid sellerId,
        Guid categoryId,
        string name,
        string? nameAr,
        string description,
        string? descriptionAr,
        JsonSpec? specs)
    {
        SellerId = sellerId;
        CategoryId = categoryId;
        Name = name;
        NameAr = nameAr;
        Description = description;
        DescriptionAr = descriptionAr;
        Specs = specs;
        Status = ProductStatus.Pending;
    }

    /// <summary>
    /// إنشاء منتج جديد
    /// </summary>
    public static Product Create(
        Guid sellerId,
        Guid categoryId,
        string name,
        string? nameAr,
        string description,
        string? descriptionAr,
        JsonSpec? specs = null)
    {
        ValidateTexts(name, nameAr, description, descriptionAr);

        var product = new Product(sellerId, categoryId, name.Trim(), 
            string.IsNullOrWhiteSpace(nameAr) ? null : nameAr.Trim(),
            description.Trim(),
            string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim(),
            specs);

        product.AddDomainEvent(new ProductCreatedPendingApprovalEvent(product.Id, sellerId, name));
        return product;
    }

    /// <summary>
    /// الموافقة على المنتج
    /// </summary>
    public void Approve(string adminId)
    {
        if (Status != ProductStatus.Pending)
            throw new DomainRuleViolationException("Only pending products can be approved");

        if (string.IsNullOrWhiteSpace(adminId))
            throw new DomainRuleViolationException("Admin ID is required for approval");

        Status = ProductStatus.Approved;
        ApprovedAtUtc = DateTime.UtcNow;
        ApprovedBy = adminId;
        RejectedReason = null;
        MarkAsModified();

        AddDomainEvent(new ProductApprovedEvent(Id, SellerId, adminId));
    }

    /// <summary>
    /// رفض المنتج
    /// </summary>
    public void Reject(string adminId, string reason)
    {
        if (Status != ProductStatus.Pending)
            throw new DomainRuleViolationException("Only pending products can be rejected");

        if (string.IsNullOrWhiteSpace(adminId))
            throw new DomainRuleViolationException("Admin ID is required for rejection");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainRuleViolationException("Rejection reason is required");

        if (reason.Length > 500)
            throw new DomainRuleViolationException("Rejection reason cannot exceed 500 characters");

        Status = ProductStatus.Rejected;
        RejectedReason = reason.Trim();
        ApprovedAtUtc = null;
        ApprovedBy = null;
        MarkAsModified();

        AddDomainEvent(new ProductRejectedEvent(Id, SellerId, adminId, reason));
    }

    /// <summary>
    /// تحديث النصوص
    /// </summary>
    public void UpdateTexts(string name, string? nameAr, string description, string? descriptionAr)
    {
        if (Status == ProductStatus.Approved)
            throw new DomainRuleViolationException("Cannot update approved product");

        ValidateTexts(name, nameAr, description, descriptionAr);

        Name = name.Trim();
        NameAr = string.IsNullOrWhiteSpace(nameAr) ? null : nameAr.Trim();
        Description = description.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim();

        // إعادة تعيين الحالة إلى Pending إذا كانت مرفوضة
        if (Status == ProductStatus.Rejected)
        {
            Status = ProductStatus.Pending;
            RejectedReason = null;
        }

        MarkAsModified();
        AddDomainEvent(new ProductUpdatedEvent(Id, SellerId));
    }

    /// <summary>
    /// تحديث المواصفات
    /// </summary>
    public void UpdateSpecs(JsonSpec? specs)
    {
        if (Status == ProductStatus.Approved)
            throw new DomainRuleViolationException("Cannot update approved product specifications");

        Specs = specs;

        // إعادة تعيين الحالة إلى Pending إذا كانت مرفوضة
        if (Status == ProductStatus.Rejected)
        {
            Status = ProductStatus.Pending;
            RejectedReason = null;
        }

        MarkAsModified();
        AddDomainEvent(new ProductUpdatedEvent(Id, SellerId));
    }

    /// <summary>
    /// تحديث التصنيف
    /// </summary>
    public void UpdateCategory(Guid categoryId)
    {
        if (Status == ProductStatus.Approved)
            throw new DomainRuleViolationException("Cannot update approved product category");

        CategoryId = categoryId;

        // إعادة تعيين الحالة إلى Pending إذا كانت مرفوضة
        if (Status == ProductStatus.Rejected)
        {
            Status = ProductStatus.Pending;
            RejectedReason = null;
        }

        MarkAsModified();
        AddDomainEvent(new ProductUpdatedEvent(Id, SellerId));
    }

    /// <summary>
    /// إضافة صورة
    /// </summary>
    public void AddImage(ImageRef image)
    {
        if (_images.Count >= 10) // حد أقصى 10 صور
            throw new DomainRuleViolationException("Cannot add more than 10 images");

        if (_images.Any(img => img.Url == image.Url))
            return; // الصورة موجودة بالفعل

        _images.Add(image);
        MarkAsModified();
    }

    /// <summary>
    /// إزالة صورة
    /// </summary>
    public void RemoveImage(string imageUrl)
    {
        var removed = _images.RemoveAll(img => img.Url == imageUrl) > 0;
        if (removed)
        {
            MarkAsModified();
        }
    }

    /// <summary>
    /// مسح جميع الصور
    /// </summary>
    public void ClearImages()
    {
        if (_images.Count > 0)
        {
            _images.Clear();
            MarkAsModified();
        }
    }

    /// <summary>
    /// التحقق من صحة النصوص
    /// </summary>
    private static void ValidateTexts(string name, string? nameAr, string description, string? descriptionAr)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleViolationException("Product name is required");

        if (name.Length > 150)
            throw new DomainRuleViolationException("Product name cannot exceed 150 characters");

        if (!string.IsNullOrWhiteSpace(nameAr) && nameAr.Length > 150)
            throw new DomainRuleViolationException("Product name in Arabic cannot exceed 150 characters");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainRuleViolationException("Product description is required");

        if (description.Length > 2000)
            throw new DomainRuleViolationException("Product description cannot exceed 2000 characters");

        if (!string.IsNullOrWhiteSpace(descriptionAr) && descriptionAr.Length > 2000)
            throw new DomainRuleViolationException("Product description in Arabic cannot exceed 2000 characters");
    }

    // ISoftDelete Implementation
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }

    /// <summary>
    /// حذف المنتج (ناعم)
    /// </summary>
    public void Delete()
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = null; // لا نحفظ من قام بالحذف في هذا التطبيق
        MarkAsModified();

        AddDomainEvent(new ProductDeletedEvent(Id, Name, SellerId));
    }

    /// <summary>
    /// استعادة المنتج المحذوف
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedAtUtc = null;
        DeletedBy = null;
        MarkAsModified();

        AddDomainEvent(new ProductRestoredEvent(Id, Name, SellerId));
    }
}
