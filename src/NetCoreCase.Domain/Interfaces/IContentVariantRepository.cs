using NetCoreCase.Domain.Entities;

namespace NetCoreCase.Domain.Interfaces;

public interface IContentVariantRepository : IBaseRepository<ContentVariant>
{
    Task<IEnumerable<ContentVariant>> GetByContentIdAsync(Guid contentId, CancellationToken cancellationToken = default);
    Task<ContentVariant?> GetDefaultVariantAsync(Guid contentId, CancellationToken cancellationToken = default);
    Task<ContentVariant?> GetUserVariantAsync(Guid contentId, Guid userId, CancellationToken cancellationToken = default);
    Task SetDefaultVariantAsync(Guid contentId, Guid variantId, CancellationToken cancellationToken = default);
    Task<bool> ContentHasVariantsAsync(Guid contentId, CancellationToken cancellationToken = default);
} 