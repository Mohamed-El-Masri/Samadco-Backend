using Domain.Abstractions.Events;
using Domain.Common;
using Domain.Entities.Carts;
using Domain.Entities.Distributors;
using Domain.Entities.Drivers;
using Domain.Entities.Notifications;
using Domain.Entities.Offers;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Products;
using Domain.Entities.Quotes;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Data;

/// <summary>
/// سياق قاعدة البيانات الرئيسي لنظام سمادكو
/// </summary>
public class SemadcoDbContext : DbContext
{
    public SemadcoDbContext(DbContextOptions<SemadcoDbContext> options) : base(options)
    {
    }

    // User Aggregates
    public DbSet<User> Users => Set<User>();
    public DbSet<SellerProfile> SellerProfiles => Set<SellerProfile>();
    public DbSet<BuyerProfile> BuyerProfiles => Set<BuyerProfile>();
    public DbSet<DriverProfile> DriverProfiles => Set<DriverProfile>();
    public DbSet<AdminProfile> AdminProfiles => Set<AdminProfile>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();

    // Product Aggregates
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    // Offer Aggregates
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<OfferItem> OfferItems => Set<OfferItem>();

    // Cart Aggregates
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    // Quote Aggregates
    public DbSet<QuoteRequest> QuoteRequests => Set<QuoteRequest>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteLine> QuoteLines => Set<QuoteLine>();

    // Order Aggregates
    public DbSet<Order> Orders => Set<Order>();

    // Payment Aggregates
    public DbSet<Payment> Payments => Set<Payment>();

    // Notification Aggregates
    public DbSet<Notification> Notifications => Set<Notification>();

    // Distributor Aggregates
    public DbSet<Distributor> Distributors => Set<Distributor>();

    // Driver Aggregates
    public DbSet<DriverApplication> DriverApplications => Set<DriverApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // تطبيق جميع التكوينات من Assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // تطبيق قواعد عامة
        ApplyGlobalQueryFilters(modelBuilder);
        ApplyNamingConventions(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // سيتم إضافة Interceptors لاحقاً
    }

    /// <summary>
    /// تطبيق فلاتر عامة مثل Soft Delete
    /// </summary>
    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // تطبيق Global Query Filter للكيانات التي تدعم ISoftDelete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var property = entityType.FindProperty("IsDeleted");
            if (property is not null && property.ClrType == typeof(bool))
            {
                var parameter = Expression.Parameter(entityType.ClrType);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property.PropertyInfo!);
                var condition = Expression.Equal(propertyAccess, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    /// <summary>
    /// تطبيق قواعد التسمية
    /// </summary>
    private void ApplyNamingConventions(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // تحويل أسماء الجداول إلى snake_case (اختياري)
            // entity.SetTableName(entity.GetTableName()?.ToSnakeCase());

            foreach (var property in entity.GetProperties())
            {
                // تحويل أسماء الأعمدة إلى snake_case (اختياري)
                // property.SetColumnName(property.GetColumnName().ToSnakeCase());
            }
        }
    }

    /// <summary>
    /// حفظ التغييرات مع نشر أحداث النطاق
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // جمع أحداث النطاق قبل الحفظ
        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // حفظ التغييرات
        var result = await base.SaveChangesAsync(cancellationToken);

        // مسح أحداث النطاق بعد الحفظ
        foreach (var entity in ChangeTracker.Entries<BaseEntity>().Select(e => e.Entity))
        {
            entity.ClearDomainEvents();
        }

        // يمكن هنا إضافة نشر الأحداث عبر MediatR أو أي نظام messaging آخر
        // await PublishDomainEventsAsync(domainEvents, cancellationToken);

        return result;
    }
}