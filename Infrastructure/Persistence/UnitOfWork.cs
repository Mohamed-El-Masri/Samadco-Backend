using Domain.Abstractions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

/// <summary>
/// تنفيذ وحدة العمل باستخدام Entity Framework
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SemadcoDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(SemadcoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return new EfTransaction(_currentTransaction, () => _currentTransaction = null);
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}

/// <summary>
/// تنفيذ المعاملة باستخدام Entity Framework
/// </summary>
internal sealed class EfTransaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;
    private readonly Action _onDispose;
    private bool _disposed;

    public EfTransaction(IDbContextTransaction transaction, Action onDispose)
    {
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        _onDispose = onDispose ?? throw new ArgumentNullException(nameof(onDispose));
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        catch
        {
            // Log the rollback exception but don't throw to prevent masking the original exception
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction.Dispose();
            _onDispose();
            _disposed = true;
        }
    }
}
