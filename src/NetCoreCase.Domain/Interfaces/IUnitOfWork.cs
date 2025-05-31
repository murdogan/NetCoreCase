namespace NetCoreCase.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IContentRepository Contents { get; }
    ICategoryRepository Categories { get; }
    IContentVariantRepository ContentVariants { get; }
    IUserContentVariantHistoryRepository UserContentVariantHistories { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
} 