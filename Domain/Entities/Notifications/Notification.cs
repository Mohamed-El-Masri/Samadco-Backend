using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Notifications;
using Domain.ValueObjects.Products;

namespace Domain.Entities.Notifications;

/// <summary>
/// كيان الإشعار - جذر التجميعة
/// </summary>
public sealed class Notification : BaseEntity, IAggregateRoot
{
    public Guid? UserId { get; private set; } // null للإشعارات العامة
    public string Title { get; private set; } = default!;
    public string? TitleAr { get; private set; }
    public string Body { get; private set; } = default!;
    public string? BodyAr { get; private set; }
    public NotificationType Type { get; private set; }
    public JsonSpec? Data { get; private set; } // بيانات إضافية بصيغة JSON
    public DateTime? ReadAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }

    /// <summary>
    /// التحقق من قراءة الإشعار
    /// </summary>
    public bool IsRead => ReadAtUtc.HasValue;

    /// <summary>
    /// التحقق من انتهاء صلاحية الإشعار
    /// </summary>
    public bool IsExpired => ExpiresAtUtc.HasValue && DateTime.UtcNow >= ExpiresAtUtc.Value;

    // للاستخدام مع EF Core
    private Notification() { }

    private Notification(
        Guid? userId,
        string title,
        string? titleAr,
        string body,
        string? bodyAr,
        NotificationType type,
        JsonSpec? data,
        DateTime? expiresAtUtc)
    {
        UserId = userId;
        Title = title;
        TitleAr = titleAr;
        Body = body;
        BodyAr = bodyAr;
        Type = type;
        Data = data;
        ExpiresAtUtc = expiresAtUtc;
    }

    /// <summary>
    /// إنشاء إشعار جديد
    /// </summary>
    public static Notification Create(
        Guid? userId,
        string title,
        string? titleAr,
        string body,
        string? bodyAr,
        NotificationType type,
        JsonSpec? data = null,
        DateTime? expiresAtUtc = null)
    {
        ValidateTexts(title, titleAr, body, bodyAr);

        if (expiresAtUtc.HasValue && expiresAtUtc.Value <= DateTime.UtcNow)
            throw new DomainRuleViolationException("Notification expiry date must be in the future");

        return new Notification(
            userId,
            title.Trim(),
            string.IsNullOrWhiteSpace(titleAr) ? null : titleAr.Trim(),
            body.Trim(),
            string.IsNullOrWhiteSpace(bodyAr) ? null : bodyAr.Trim(),
            type,
            data,
            expiresAtUtc);
    }

    /// <summary>
    /// تحديد الإشعار كمقروء
    /// </summary>
    public void MarkRead(DateTime? readAtUtc = null)
    {
        if (IsRead)
            return; // مسبقاً مقروء

        ReadAtUtc = readAtUtc ?? DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// تحديد الإشعار كغير مقروء
    /// </summary>
    public void MarkUnread()
    {
        if (!IsRead)
            return; // مسبقاً غير مقروء

        ReadAtUtc = null;
        MarkAsModified();
    }

    /// <summary>
    /// تحديث محتوى الإشعار
    /// </summary>
    public void UpdateContent(
        string title,
        string? titleAr,
        string body,
        string? bodyAr,
        JsonSpec? data = null)
    {
        ValidateTexts(title, titleAr, body, bodyAr);

        Title = title.Trim();
        TitleAr = string.IsNullOrWhiteSpace(titleAr) ? null : titleAr.Trim();
        Body = body.Trim();
        BodyAr = string.IsNullOrWhiteSpace(bodyAr) ? null : bodyAr.Trim();
        Data = data;

        MarkAsModified();
    }

    /// <summary>
    /// تحديث تاريخ انتهاء الصلاحية
    /// </summary>
    public void UpdateExpiryDate(DateTime? expiresAtUtc)
    {
        if (expiresAtUtc.HasValue && expiresAtUtc.Value <= DateTime.UtcNow)
            throw new DomainRuleViolationException("Notification expiry date must be in the future");

        ExpiresAtUtc = expiresAtUtc;
        MarkAsModified();
    }

    /// <summary>
    /// انتهاء صلاحية الإشعار
    /// </summary>
    public void Expire()
    {
        if (IsExpired)
            return; // مسبقاً منتهي الصلاحية

        ExpiresAtUtc = DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// التحقق من صحة النصوص
    /// </summary>
    private static void ValidateTexts(string title, string? titleAr, string body, string? bodyAr)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainRuleViolationException("Notification title is required");

        if (title.Length > 200)
            throw new DomainRuleViolationException("Notification title cannot exceed 200 characters");

        if (!string.IsNullOrWhiteSpace(titleAr) && titleAr.Length > 200)
            throw new DomainRuleViolationException("Notification title in Arabic cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(body))
            throw new DomainRuleViolationException("Notification body is required");

        if (body.Length > 1000)
            throw new DomainRuleViolationException("Notification body cannot exceed 1000 characters");

        if (!string.IsNullOrWhiteSpace(bodyAr) && bodyAr.Length > 1000)
            throw new DomainRuleViolationException("Notification body in Arabic cannot exceed 1000 characters");
    }
}
