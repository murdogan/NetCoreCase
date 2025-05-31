using NetCoreCase.Domain.Entities;

namespace NetCoreCase.Domain.Interfaces;

public interface IContentRepository : IBaseRepository<Content>
{
    Task<IEnumerable<Content>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Content>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Content>> GetByLanguageAsync(string language, CancellationToken cancellationToken = default);
    Task<IEnumerable<Content>> GetByCategoryAndLanguageAsync(Guid categoryId, string language, CancellationToken cancellationToken = default);
    Task<Content?> GetWithVariantsAsync(Guid contentId, CancellationToken cancellationToken = default);
    Task<Content?> GetWithAllRelationsAsync(Guid contentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Content>> GetContentsWithVariantsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Content>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
} 