using Microsoft.EntityFrameworkCore.Storage;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
namespace LoadlyAggregator.Infrastructure.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    public async Task CommitTransactionAsync()
    {
        await (_transaction != null ? CommitTransactionInternal() : Task.CompletedTask);
    }

    private async Task CommitTransactionInternal()
    {
        await _transaction!.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        await (_transaction != null ? RollbackTransactionInternal() : Task.CompletedTask);
    }

    private async Task RollbackTransactionInternal()
    {
        await _transaction!.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
