using Domain.Abstractions;
using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Users;
using Domain.Events.Users;

namespace Domain.Entities.Users;

/// <summary>
/// ملف الإداري
/// </summary>
public sealed class AdminProfile : BaseEntity, ISoftDelete
{
    public Guid UserId { get; private set; }
    public string Department { get; private set; } = default!;
    public AdminPermissions Permissions { get; private set; }
    public AdminStatus Status { get; private set; } = AdminStatus.Active;
    public DateTime? LastLoginAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? PermissionsUpdatedAt { get; private set; }
    public string? PermissionsUpdatedBy { get; private set; }
    
    // Activity Statistics
    public int TotalActionsPerformed { get; private set; }
    public DateTime? LastActionAt { get; private set; }
    public string? LastActionType { get; private set; }
    
    // ISoftDelete implementation
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    
    // Navigation Properties
    public User User { get; private set; } = default!;

    // For EF Core
    private AdminProfile() { }

    private AdminProfile(Guid userId, string department, AdminPermissions permissions)
    {
        UserId = userId;
        Department = department;
        Permissions = permissions;
        Status = AdminStatus.Active;
        TotalActionsPerformed = 0;
    }

    /// <summary>
    /// إنشاء ملف إداري جديد
    /// </summary>
    public static AdminProfile Create(Guid userId, string department, AdminPermissions permissions)
    {
        if (string.IsNullOrWhiteSpace(department))
            throw new DomainRuleViolationException("Department is required");

        var profile = new AdminProfile(userId, department.Trim(), permissions);
        profile.AddDomainEvent(new AdminProfileCreatedEvent(profile.Id, userId, department));
        return profile;
    }

    /// <summary>
    /// تحديث الصلاحيات
    /// </summary>
    public void UpdatePermissions(AdminPermissions permissions, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new DomainRuleViolationException("Updated by is required");

        var oldPermissions = Permissions;
        Permissions = permissions;
        PermissionsUpdatedAt = DateTime.UtcNow;
        PermissionsUpdatedBy = updatedBy.Trim();
        MarkAsModified();
        
        AddDomainEvent(new AdminPermissionsUpdatedEvent(Id, UserId, oldPermissions, permissions, updatedBy));
    }

    /// <summary>
    /// تحديث القسم
    /// </summary>
    public void UpdateDepartment(string department)
    {
        if (string.IsNullOrWhiteSpace(department))
            throw new DomainRuleViolationException("Department is required");

        var oldDepartment = Department;
        Department = department.Trim();
        MarkAsModified();
        
        AddDomainEvent(new AdminDepartmentUpdatedEvent(Id, UserId, oldDepartment, department));
    }

    /// <summary>
    /// تسجيل تسجيل الدخول
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// تسجيل إجراء إداري
    /// </summary>
    public void RecordAction(string actionType)
    {
        if (string.IsNullOrWhiteSpace(actionType))
            return;

        TotalActionsPerformed++;
        LastActionAt = DateTime.UtcNow;
        LastActionType = actionType.Trim();
        MarkAsModified();
        
        AddDomainEvent(new AdminActionPerformedEvent(Id, UserId, actionType));
    }

    /// <summary>
    /// تعطيل الإداري
    /// </summary>
    public void Disable(string reason, string disabledBy)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainRuleViolationException("Disable reason is required");

        Status = AdminStatus.Disabled;
        MarkAsModified();
        
        AddDomainEvent(new AdminDisabledEvent(Id, UserId, reason, disabledBy));
    }

    /// <summary>
    /// تفعيل الإداري
    /// </summary>
    public void Enable(string enabledBy)
    {
        Status = AdminStatus.Active;
        MarkAsModified();
        
        AddDomainEvent(new AdminEnabledEvent(Id, UserId, enabledBy));
    }

    /// <summary>
    /// التحقق من الصلاحية
    /// </summary>
    public bool HasPermission(AdminPermissions permission)
    {
        if (Status != AdminStatus.Active)
            return false;

        return Permissions.HasFlag(permission);
    }

    /// <summary>
    /// التحقق من صلاحية إدارة المستخدمين
    /// </summary>
    public bool CanManageUsers()
    {
        return HasPermission(AdminPermissions.ManageUsers);
    }

    /// <summary>
    /// التحقق من صلاحية إدارة المنتجات
    /// </summary>
    public bool CanManageProducts()
    {
        return HasPermission(AdminPermissions.ManageProducts);
    }

    /// <summary>
    /// التحقق من صلاحية إدارة الطلبات
    /// </summary>
    public bool CanManageOrders()
    {
        return HasPermission(AdminPermissions.ManageOrders);
    }

    /// <summary>
    /// التحقق من صلاحية عرض التقارير
    /// </summary>
    public bool CanViewReports()
    {
        return HasPermission(AdminPermissions.ViewReports);
    }

    /// <summary>
    /// التحقق من صلاحية إدارة النظام
    /// </summary>
    public bool CanManageSystem()
    {
        return HasPermission(AdminPermissions.ManageSystem);
    }

    /// <summary>
    /// حذف ناعم للملف
    /// </summary>
    public void Delete()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// استعادة الملف المحذوف
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAtUtc = null;
        MarkAsModified();
    }
}
