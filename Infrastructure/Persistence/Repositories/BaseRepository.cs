using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using Domain.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// المستودع الأساسي لجميع الكيانات
/// </summary>
public abstract class BaseRepository<TAggregate> : IRepository<TAggregate>
    where TAggregate : class, IAggregateRoot
{
    protected readonly SemadcoDbContext Context;
    protected readonly DbSet<TAggregate> DbSet;

    protected BaseRepository(SemadcoDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TAggregate>();
    }

    public virtual async Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task AddAsync(TAggregate entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TAggregate entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Remove(TAggregate entity)
    {
        if (entity is ISoftDelete softDeleteEntity)
        {
            softDeleteEntity.Delete();
            Update(entity);
        }
        else
        {
            DbSet.Remove(entity);
        }
    }
}
