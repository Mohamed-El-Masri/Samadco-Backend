using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// تكوين حقن التبعيات للطبقة Infrastructure
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// إضافة خدمات Infrastructure إلى حاوي حقن التبعيات
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // إضافة DbContext
        AddDatabase(services, configuration);

        // إضافة Unit of Work
        AddUnitOfWork(services);

        // إضافة Repositories
        AddRepositories(services);

        return services;
    }

    /// <summary>
    /// تكوين قاعدة البيانات
    /// </summary>
    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<SemadcoDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                
                sqlOptions.CommandTimeout(30);
            });

            // تكوين إضافي للتطوير
            options.EnableSensitiveDataLogging(false);
            options.EnableServiceProviderCaching();
            options.EnableDetailedErrors(false);
        });
    }

    /// <summary>
    /// إضافة Unit of Work
    /// </summary>
    private static void AddUnitOfWork(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    /// <summary>
    /// إضافة جميع Repositories
    /// </summary>
    private static void AddRepositories(IServiceCollection services)
    {
        // User repositories
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Product repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        
        // يمكن إضافة repositories أخرى هنا عند الحاجة
        // services.AddScoped<IOrderRepository, OrderRepository>();
        // services.AddScoped<IQuoteRepository, QuoteRepository>();
    }
}