using Microsoft.EntityFrameworkCore.Storage;
using NetCoreCase.Domain.Interfaces;
using NetCoreCase.Infrastructure.Data.Repositories;

namespace NetCoreCase.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IUserRepository? _users;
    private ICategoryRepository? _categories;
    private IContentRepository? _contents;
    private IContentVariantRepository? _contentVariants;
    private IUserContentVariantHistoryRepository? _userContentVariantHistories;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users 
    { 
        get 
        { 
            return _users ??= new UserRepository(_context); 
        } 
    }

    public ICategoryRepository Categories 
    { 
        get 
        { 
            return _categories ??= new CategoryRepository(_context); 
        } 
    }

    public IContentRepository Contents 
    { 
        get 
        { 
            return _contents ??= new ContentRepository(_context); 
        } 
    }

    public IContentVariantRepository ContentVariants 
    { 
        get 
        { 
            return _contentVariants ??= new ContentVariantRepository(_context); 
        } 
    }

    public IUserContentVariantHistoryRepository UserContentVariantHistories 
    { 
        get 
        { 
            return _userContentVariantHistories ??= new UserContentVariantHistoryRepository(_context); 
        } 
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already started.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction started.");
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction started.");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        await _context.DisposeAsync();
    }
} 