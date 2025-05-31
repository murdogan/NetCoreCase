using NetCoreCase.Domain.Entities;

namespace NetCoreCase.Domain.Interfaces;

public interface IUserContentVariantHistoryRepository : IBaseRepository<UserContentVariantHistory>
{
    Task<UserContentVariantHistory?> GetUserContentHistoryAsync(Guid userId, Guid contentId, CancellationToken cancellationToken = default);
    Task<UserContentVariantHistory> CreateOrUpdateHistoryAsync(Guid userId, Guid contentId, Guid variantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserContentVariantHistory>> GetUserHistoryAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserContentVariantHistory>> GetContentHistoryAsync(Guid contentId, CancellationToken cancellationToken = default);
    Task<bool> HasUserViewedContentAsync(Guid userId, Guid contentId, CancellationToken cancellationToken = default);
} 