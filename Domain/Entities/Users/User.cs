using Domain.Abstractions;
using Domain.Abstractions.Errors;
using Domain.Common;
using Domain.Enums.Users;
using Domain.Events.Users;
using Domain.ValueObjects.Identity;
using Domain.ValueObjects.Location;

namespace Domain.Entities.Users;

/// <summary>
/// المستخدم - جذر التجميعة للمستخدمين والأدوار المتعددة
/// </summary>
public sealed class User : BaseEntity, IAggregateRoot, ISoftDelete
{
    private readonly List<UserRole> _roles = new();
    private readonly List<Guid> _addressIds = new();
    private readonly List<Guid> _driverApplicationIds = new();

    public string FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public PhoneNumber? Phone { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.PendingActivation;
    public DateTime? LastLoginUtc { get; private set; }
    public string? ProfileImageUrl { get; private set; }
    public DateTime? EmailVerifiedAt { get; private set; }
    public DateTime? PhoneVerifiedAt { get; private set; }
    
    /// <summary>
    /// أدوار المستخدم المتعددة
    /// </summary>
    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();
    
    /// <summary>
    /// معرفات العناوين المرتبطة بالمستخدم
    /// </summary>
    public IReadOnlyList<Guid> AddressIds => _addressIds.AsReadOnly();

    /// <summary>
    /// ملف المشتري (إذا كان له دور مشتري)
    /// </summary>
    public BuyerProfile? BuyerProfile { get; private set; }
    
    /// <summary>
    /// ملف البائع (إذا كان له دور بائع)
    /// </summary>
    public SellerProfile? SellerProfile { get; private set; }
    
    /// <summary>
    /// ملف السائق (إذا كان له دور سائق)
    /// </summary>
    public DriverProfile? DriverProfile { get; private set; }
    
    /// <summary>
    /// ملف الإداري (إذا كان له دور إداري)
    /// </summary>
    public AdminProfile? AdminProfile { get; private set; }

    // للاستخدام مع EF Core
    private User() { }

    private User(string fullName, Email email, PhoneNumber? phone)
    {
        FullName = fullName;
        Email = email;
        Phone = phone;
        Status = UserStatus.PendingActivation;
    }

    /// <summary>
    /// إنشاء مستخدم جديد
    /// </summary>
    public static User Create(string fullName, Email email, PhoneNumber? phone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainRuleViolationException("Full name is required");

        if (fullName.Length > 100)
            throw new DomainRuleViolationException("Full name cannot exceed 100 characters");

        var user = new User(fullName.Trim(), email, phone);
        user.AddDomainEvent(new UserCreatedEvent(user.Id, email.Value, fullName));
        return user;
    }

    /// <summary>
    /// تفعيل المستخدم
    /// </summary>
    public void Activate()
    {
        if (Status == UserStatus.Active)
            throw new DomainRuleViolationException("User is already active");

        if (Status == UserStatus.Deleted)
            throw new DomainRuleViolationException("Cannot activate a deleted user");

        Status = UserStatus.Active;
        MarkAsModified();
        AddDomainEvent(new UserActivatedEvent(Id));
    }

    /// <summary>
    /// إلغاء تفعيل المستخدم
    /// </summary>
    public void Deactivate(string reason)
    {
        if (Status == UserStatus.Deleted)
            throw new DomainRuleViolationException("User is already deleted");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainRuleViolationException("Deactivation reason is required");

        Status = UserStatus.Suspended;
        MarkAsModified();
        AddDomainEvent(new UserDeactivatedEvent(Id, reason));
    }

    /// <summary>
    /// إضافة دور للمستخدم
    /// </summary>
    public void AddRole(UserRole role)
    {
        if (_roles.Contains(role))
            return; // الدور موجود بالفعل

        _roles.Add(role);
        MarkAsModified();
        AddDomainEvent(new UserRoleAddedEvent(Id, role.ToString()));
    }

    /// <summary>
    /// إزالة دور من المستخدم
    /// </summary>
    public void RemoveRole(UserRole role)
    {
        var removed = _roles.Remove(role);
        if (removed)
        {
            MarkAsModified();
            AddDomainEvent(new UserRoleRemovedEvent(Id, role.ToString()));
        }
    }

    /// <summary>
    /// التحقق من وجود دور معين
    /// </summary>
    public bool HasRole(UserRole role)
    {
        return _roles.Contains(role);
    }

    /// <summary>
    /// التحقق من وجود أي من الأدوار المحددة
    /// </summary>
    public bool HasAnyRole(params UserRole[] roles)
    {
        return roles.Any(role => _roles.Contains(role));
    }

    /// <summary>
    /// إضافة عنوان جديد
    /// </summary>
    public void AddAddress(Guid addressId)
    {
        if (!_addressIds.Contains(addressId))
        {
            _addressIds.Add(addressId);
            MarkAsModified();
        }
    }

    /// <summary>
    /// إزالة عنوان
    /// </summary>
    public void RemoveAddress(Guid addressId)
    {
        if (_addressIds.Remove(addressId))
        {
            MarkAsModified();
        }
    }

    /// <summary>
    /// إرفاق ملف المشتري
    /// </summary>
    public void AttachBuyerProfile()
    {
        if (BuyerProfile != null)
            throw new DomainRuleViolationException("User already has a buyer profile");

        if (!HasRole(UserRole.Buyer))
            throw new DomainRuleViolationException("User must have Buyer role to attach buyer profile");

        BuyerProfile = BuyerProfile.Create(Id);
        MarkAsModified();
    }

    /// <summary>
    /// إرفاق ملف البائع
    /// </summary>
    public void AttachSellerProfile(string companyName, string? companyNameAr, TaxId taxId, Address? address = null)
    {
        if (SellerProfile != null)
            throw new DomainRuleViolationException("User already has a seller profile");

        if (!HasRole(UserRole.Seller))
            throw new DomainRuleViolationException("User must have Seller role to attach seller profile");

        SellerProfile = SellerProfile.Create(Id, companyName, companyNameAr, taxId, address);
        MarkAsModified();
    }

    /// <summary>
    /// إرفاق ملف السائق
    /// </summary>
    public void AttachDriverProfile(string licenseNumber, DateTime licenseExpiryDate)
    {
        if (DriverProfile != null)
            throw new DomainRuleViolationException("User already has a driver profile");

        if (!HasRole(UserRole.Driver))
            throw new DomainRuleViolationException("User must have Driver role to attach driver profile");

        DriverProfile = DriverProfile.Create(Id, licenseNumber, licenseExpiryDate);
        MarkAsModified();
    }

    /// <summary>
    /// إرفاق ملف الإداري
    /// </summary>
    public void AttachAdminProfile(string department, AdminPermissions permissions)
    {
        if (AdminProfile != null)
            throw new DomainRuleViolationException("User already has an admin profile");

        if (!HasRole(UserRole.Admin))
            throw new DomainRuleViolationException("User must have Admin role to attach admin profile");

        AdminProfile = AdminProfile.Create(Id, department, permissions);
        MarkAsModified();
    }

    /// <summary>
    /// التحقق من ملف البائع
    /// </summary>
    public void VerifySellerProfile(string verifiedBy)
    {
        if (SellerProfile == null)
            throw new DomainRuleViolationException("User does not have a seller profile");

        SellerProfile.MarkVerified(verifiedBy);
        MarkAsModified();
    }

    /// <summary>
    /// تحديث رقم الهاتف
    /// </summary>
    public void UpdatePhone(PhoneNumber? phone)
    {
        Phone = phone;
        MarkAsModified();
    }

    /// <summary>
    /// تحديث الاسم الكامل
    /// </summary>
    public void UpdateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainRuleViolationException("Full name is required");

        if (fullName.Length > 100)
            throw new DomainRuleViolationException("Full name cannot exceed 100 characters");

        FullName = fullName.Trim();
        MarkAsModified();
    }

    /// <summary>
    /// تسجيل تسجيل دخول
    /// </summary>
    public void RecordLogin()
    {
        LastLoginUtc = DateTime.UtcNow;
        MarkAsModified();
    }

    /// <summary>
    /// إضافة معرف طلب السائق (مرجعية معرفية فقط)
    /// </summary>
    internal void AddDriverApplicationId(Guid driverApplicationId)
    {
        if (!_driverApplicationIds.Contains(driverApplicationId))
        {
            _driverApplicationIds.Add(driverApplicationId);
            MarkAsModified();
        }
    }

    /// <summary>
    /// إزالة معرف طلب السائق
    /// </summary>
    internal void RemoveDriverApplicationId(Guid driverApplicationId)
    {
        if (_driverApplicationIds.Remove(driverApplicationId))
        {
            MarkAsModified();
        }
    }

    // ISoftDelete Implementation
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }

    /// <summary>
    /// حذف المستخدم (ناعم)
    /// </summary>
    public void Delete()
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = null; // لا نحفظ من قام بالحذف في هذا التطبيق
        Status = UserStatus.Deleted;
        MarkAsModified();

        AddDomainEvent(new UserDeletedEvent(Id, FullName, Email.Value));
    }

    /// <summary>
    /// استعادة المستخدم المحذوف
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedAtUtc = null;
        DeletedBy = null;
        Status = UserStatus.PendingActivation;
        MarkAsModified();

        AddDomainEvent(new UserRestoredEvent(Id, FullName, Email.Value));
    }
}
